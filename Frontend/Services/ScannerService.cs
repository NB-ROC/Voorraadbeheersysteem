using System;
using System.Linq;
using System.Reactive.Linq;
using PCSC;
using PCSC.Iso7816;
using PCSC.Monitoring;
using PCSC.Reactive;
using PCSC.Reactive.Events;
using PCSC.Utils;

namespace Frontend.Services;

public class ScannerService
{
    private static readonly byte[] UidApdu = [0xFF, 0xCA, 0x00, 0x00, 0x00]; // Command for getting the uid
    
    private Action<byte[]>? _callback;
    private IDisposable? _subscription;
    private ISCardContext? _context;
    
    public void SetCallback(Action<byte[]> callback) => _callback = callback;

    public ScannerService()
    {
        StartService();
    }

    public void StartService()
    {
        
        _context = ContextFactory.Instance.Establish(SCardScope.System);
        var readers = _context.GetReaders();

        if (readers.Length == 0) return;
        var readerName = readers[0];

        var observable = MonitorFactory.Instance.CreateObservable(SCardScope.System, readerName);

        _subscription = observable
            .OfType<CardInserted>()
            .Subscribe(HandleScan);
    }

    public void StopService()
    {
        _subscription?.Dispose();
    }

    private void HandleScan(CardInserted card)
    {
        if (_callback == null || _context == null)
            return;

        byte[]? uid = TryGetUid(_context, card.ReaderName);

        if (uid == null) return;
        _callback(uid);
        _callback = null;
    }

    
    private static byte[]? TryGetUid(ISCardContext context, string readerName)
    {
        using var reader = new SCardReader(context);

        if (reader.Connect(
                readerName,
                SCardShareMode.Shared,
                SCardProtocol.Any
            ) != SCardError.Success)
        {
            return null;
        }

        try
        {
            var buffer = new byte[256];
            var receivePci = new SCardPCI();

            if (reader.Transmit(
                    SCardPCI.GetPci(reader.ActiveProtocol),
                    UidApdu,
                    receivePci,
                    ref buffer
                ) != SCardError.Success)
            {
                return null;
            }

            var response = new ResponseApdu(
                buffer,
                IsoCase.Case2Short,
                reader.ActiveProtocol
            );

            return response.GetData();
        }
        catch (ArgumentNullException)
        {
            return null;
        }
        finally
        {
            reader.Disconnect(SCardReaderDisposition.Leave);
        }
    }
}