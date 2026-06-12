using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PCSC;
using PCSC.Exceptions;
using PCSC.Monitoring;

namespace Frontend.Services;

public interface ISmartCardService : IDisposable
{
    bool HasAvailableReader { get; }

    event Action<bool>? ReadersAvailableChanged;

    void SetCardDetectedCallback(Action<byte[]> callback);

    void Start();
    void Stop();
}

/// <summary>
///     A reactive, hot-pluggable smart card service that monitors all connected PC/SC
///     readers simultaneously and invokes a user-supplied callback with the card UID
///     whenever a card is presented to any reader.
///     NuGet dependencies:
///     PCSC            >= 6.x   (dotnet add package PCSC)
///     PCSC.Reactive   >= 6.x   (dotnet add package PCSC.Reactive)  [optional – not used here]
///     Usage:
///     var svc = new SmartCardService();
///     svc.SetCardDetectedCallback(uid =>
///     Console.WriteLine("Card UID: " + BitConverter.ToString(uid)));
///     svc.Start();
///     // ... later ...
///     svc.Stop();
///     svc.Dispose();
/// </summary>
public sealed class SmartCardService : ISmartCardService
{
    // -------------------------------------------------------------------------
    // APDU that asks the reader for the card UID (Get Data – UID)
    // -------------------------------------------------------------------------
    private static readonly byte[] GetUidApdu = [0xFF, 0xCA, 0x00, 0x00, 0x00];

    private readonly ISCardContext _context;
    private readonly IContextFactory _contextFactory;
    private readonly IMonitorFactory _monitorFactory;

    /// <summary>One monitor per reader name so we can add/remove individually.</summary>
    private readonly ConcurrentDictionary<string, ISCardMonitor> _monitors = new();

    // -------------------------------------------------------------------------
    // Private state
    // -------------------------------------------------------------------------
    private Action<byte[]>? _cardDetectedCallback;

    private CancellationTokenSource? _cts;
    private bool _disposed;

    // -------------------------------------------------------------------------
    // Constructor
    // -------------------------------------------------------------------------
    public SmartCardService()
    {
        _contextFactory = ContextFactory.Instance;
        _context = _contextFactory.Establish(SCardScope.System);
        _monitorFactory = MonitorFactory.Instance;
        Start();
    }

    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------

    /// <summary>
    ///     Returns <c>true</c> when at least one reader is currently active and
    ///     monitored; <c>false</c> when no readers are connected.
    ///     This is a point-in-time snapshot — subscribe to
    ///     <see cref="ReadersAvailableChanged" /> for reactive updates.
    /// </summary>
    public bool HasAvailableReader => !_monitors.IsEmpty;

    // -------------------------------------------------------------------------
    // IDisposable
    // -------------------------------------------------------------------------

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        Stop();

        _context.Dispose();
        _cts?.Dispose();
    }

    /// <summary>
    ///     Raised whenever the set of monitored readers changes (reader plugged in
    ///     or removed).  The <c>bool</c> argument reflects the new value of
    ///     <see cref="HasAvailableReader" /> after the change.
    ///     The event is fired on a background thread — marshal to the UI thread
    ///     before touching bound properties, e.g.:
    ///     <code>
    /// _service.ReadersAvailableChanged += isAvailable =>
    ///     Dispatcher.UIThread.Post(() => IsReaderConnected = isAvailable);
    /// </code>
    /// </summary>
    public event Action<bool>? ReadersAvailableChanged;

    /// <summary>
    ///     Assigns (or replaces) the callback that is invoked whenever a card is
    ///     detected on any reader.  The <paramref name="callback" /> receives the raw
    ///     UID bytes returned by the card.
    /// </summary>
    public void SetCardDetectedCallback(Action<byte[]> callback)
    {
        _cardDetectedCallback = callback ?? throw new ArgumentNullException(nameof(callback));
    }

    /// <summary>
    ///     Starts monitoring.  Initializes all readers that are currently connected
    ///     and also watches for future plug/unplug events.
    /// </summary>
    public void Start()
    {
        ThrowIfDisposed();

        _cts = new CancellationTokenSource();

        // 1. Spin up monitors for every reader that is already connected.
        IEnumerable<string> readers = GetCurrentReaders();
        foreach (string reader in readers)
            AddReaderMonitor(reader);

        // 2. Start the plug/unplug watcher in a background task.
        Task.Run(() => WatchPlugEvents(_cts.Token), _cts.Token);
    }

    /// <summary>
    ///     Stops all monitoring activity gracefully.
    /// </summary>
    public void Stop()
    {
        _cts?.Cancel();
        TearDownAllMonitors();
    }

    // -------------------------------------------------------------------------
    // Hot-plug watcher
    // -------------------------------------------------------------------------

    /// <summary>
    ///     Polls the PC/SC subsystem for reader list changes.  When a new reader
    ///     appears it is initialized immediately; when one disappears its monitor is
    ///     cleaned up.
    /// </summary>
    private async Task WatchPlugEvents(CancellationToken ct)
    {
        HashSet<string> known = new(GetCurrentReaders());

        while (!ct.IsCancellationRequested)
            try
            {
                await Task.Delay(1_000, ct); // poll every second

                HashSet<string> current = new(GetCurrentReaders());

                // Newly plugged readers
                foreach (string r in current.Except(known))
                {
                    Console.WriteLine($"[SmartCardService] Reader connected: {r}");
                    AddReaderMonitor(r);
                }

                // Removed readers
                foreach (string r in known.Except(current))
                {
                    Console.WriteLine($"[SmartCardService] Reader disconnected: {r}");
                    RemoveReaderMonitor(r);
                }

                known = current;
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                // Never crash the watcher — log and continue.
                Console.WriteLine($"[SmartCardService] Plug-watcher error: {ex.Message}");
            }
    }

    // -------------------------------------------------------------------------
    // Per-reader monitor management
    // -------------------------------------------------------------------------

    private void AddReaderMonitor(string readerName)
    {
        if (_monitors.ContainsKey(readerName))
            return; // already monitored

        try
        {
            ISCardMonitor? monitor = _monitorFactory.Create(SCardScope.System);

            monitor.CardInserted += OnCardInserted;
            monitor.MonitorException += OnMonitorException;

            monitor.Start(readerName);

            if (_monitors.TryAdd(readerName, monitor))
            {
                Console.WriteLine($"[SmartCardService] Now monitoring: {readerName}");
                ReadersAvailableChanged?.Invoke(HasAvailableReader);
            }
            else
            {
                // Race – another thread already added it.
                monitor.Cancel();
                monitor.Dispose();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SmartCardService] Failed to monitor '{readerName}': {ex.Message}");
        }
    }

    private void RemoveReaderMonitor(string readerName)
    {
        if (!_monitors.TryRemove(readerName, out ISCardMonitor? monitor))
            return;

        try
        {
            monitor.CardInserted -= OnCardInserted;
            monitor.MonitorException -= OnMonitorException;
            monitor.Cancel();
            monitor.Dispose();
            Console.WriteLine($"[SmartCardService] Stopped monitoring: {readerName}");
            ReadersAvailableChanged?.Invoke(HasAvailableReader);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SmartCardService] Error removing monitor for '{readerName}': {ex.Message}");
        }
    }

    private void TearDownAllMonitors()
    {
        foreach (string key in _monitors.Keys.ToList())
            RemoveReaderMonitor(key);
    }

    // -------------------------------------------------------------------------
    // Card-inserted event handler
    // -------------------------------------------------------------------------

    private void OnCardInserted(object sender, CardStatusEventArgs e)
    {
        string? readerName = e.ReaderName;
        Console.WriteLine("[SmartCardService] Card inserted: " + readerName);

        Task.Run(() =>
        {
            try
            {
                byte[]? uid = ReadUid(readerName);
                if (uid == null || _cardDetectedCallback == null) return;
                _cardDetectedCallback(uid);
                _cardDetectedCallback = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SmartCardService] Error reading UID from '{readerName}': {ex.Message}");
            }
        });
    }

    // -------------------------------------------------------------------------
    // APDU – read UID
    // -------------------------------------------------------------------------

    /// <summary>
    ///     Opens a short-lived connection to the reader, transmits the Get-UID APDU
    ///     and returns the raw UID bytes (stripping the trailing 0x90 0x00 status word).
    ///     Returns <c>null</c> if the card could not be read.
    /// </summary>
    private byte[]? ReadUid(string readerName)
    {
        try
        {
            using ISCardContext? ctx = _contextFactory.Establish(SCardScope.System);
            using ICardReader? reader = ctx.ConnectReader(readerName, SCardShareMode.Shared, SCardProtocol.Any);

            IntPtr sendPci = SCardPCI.GetPci(reader.Protocol);
            byte[] receiveBuffer = new byte[258]; // max APDU response

            int bytesReceived = reader.Transmit(sendPci, GetUidApdu, receiveBuffer);

            if (bytesReceived < 2)
            {
                Console.WriteLine($"[SmartCardService] Short APDU response from '{readerName}'.");
                return null;
            }

            // Last two bytes are the status word (SW1 SW2).
            // Success == 0x90 0x00
            byte sw1 = receiveBuffer[bytesReceived - 2];
            byte sw2 = receiveBuffer[bytesReceived - 1];

            if (sw1 != 0x90 || sw2 != 0x00)
            {
                Console.WriteLine(
                    $"[SmartCardService] APDU error from '{readerName}': SW={sw1:X2}{sw2:X2}");
                return null;
            }

            // Strip the status word – the rest is the UID.
            byte[] uid = new byte[bytesReceived - 2];
            Array.Copy(receiveBuffer, uid, uid.Length);
            return uid;
        }
        catch (NoServiceException)
        {
            Console.WriteLine($"[SmartCardService] PC/SC service unavailable while reading '{readerName}'.");
            return null;
        }
        catch (RemovedCardException)
        {
            Console.WriteLine($"[SmartCardService] Card removed too quickly from '{readerName}'.");
            return null;
        }
        catch (ReaderUnavailableException)
        {
            Console.WriteLine($"[SmartCardService] Reader '{readerName}' became unavailable.");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SmartCardService] Unexpected error reading '{readerName}': {ex.Message}");
            return null;
        }
    }

    // -------------------------------------------------------------------------
    // Monitor exception handler
    // -------------------------------------------------------------------------

    private void OnMonitorException(object sender, PCSCException ex)
    {
        // A monitor exception usually means the reader was yanked out.
        // The plug-watcher will clean it up on the next poll cycle.
        Console.WriteLine($"[SmartCardService] Monitor exception ({ex.SCardError}): {ex.Message}");
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private IEnumerable<string> GetCurrentReaders()
    {
        try
        {
            string[]? readers = _context.GetReaders();
            return readers ?? Enumerable.Empty<string>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SmartCardService] Could not enumerate readers: {ex.Message}");
            return Enumerable.Empty<string>();
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(SmartCardService));
    }
}