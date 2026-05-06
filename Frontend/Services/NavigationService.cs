using Frontend.ViewModels;

namespace Frontend.Services;

public class NavigationService
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