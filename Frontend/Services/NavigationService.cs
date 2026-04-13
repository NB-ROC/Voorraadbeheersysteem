using Frontend.ViewModels;
using ReactiveUI;

namespace Frontend.Services;

public class NavigationService : ReactiveObject
{
    private readonly MainWindowViewModel _mainWindow;

    public NavigationService(MainWindowViewModel mainWindow)
    {
        _mainWindow = mainWindow;
    }

    public void NavigateTo(ViewModelBase vm)
    {
        _mainWindow.CurrentPage = vm;
    }
}