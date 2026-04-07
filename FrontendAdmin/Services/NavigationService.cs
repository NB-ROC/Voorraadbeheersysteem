using System.Windows.Input;
using FrontendAdmin.ViewModels;
using FrontendAdmin.ViewModels.Dashboard;
using ReactiveUI;

namespace FrontendAdmin.Services;

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