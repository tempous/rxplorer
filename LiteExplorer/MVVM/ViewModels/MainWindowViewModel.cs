using LiteExplorer.Infrastructure.Commands;
using LiteExplorer.MVVM.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LiteExplorer.MVVM.ViewModels;

internal class MainWindowViewModel : ViewModel
{
    #region Properties

    #region MainWindowTitle

    private string mainWindowTitle = "LiteExplorer";

    public string MainWindowTitle
    {
        get => mainWindowTitle;
        set => SetValue(ref mainWindowTitle, value);
    }

    #endregion

    #region CurrentTab

    private TabContentViewModel currentTab;

    public TabContentViewModel CurrentTab
    {
        get => currentTab;
        set => SetValue(ref currentTab, value);
    }

    #endregion

    public ObservableCollection<TabContentViewModel> Tabs { get; set; } = new();

    #endregion

    #region Commands

    #region CloseApp

    public ICommand CloseAppCmd { get; }

    private bool CanCloseAppCmdExecute(object p) => true;

    private void OnCloseAppCmdExecuted(object p) => Application.Current.Shutdown();

    #endregion

    #region AddTab

    public ICommand AddTabCmd { get; }

    private bool CanAddTabCmdExecute(object p) => true;

    private void OnAddTabCmdExecuted(object p)
    {
        var newTab = new TabContentViewModel();
        Tabs.Add(newTab);
        CurrentTab = newTab;
    }

    #endregion

    #region CloseTab

    public ICommand CloseTabCmd { get; }

    private bool CanCloseTabCmdExecute(object p) => Tabs.Count > 1;

    private void OnCloseTabCmdExecuted(object p)
    {
        if (p is TabContentViewModel closeTab)
        {
            closeTab.Dispose();
            Tabs.Remove(closeTab);
            CurrentTab = Tabs.Last();
        }
    }

    #endregion

    #endregion

    #region Constructor

    public MainWindowViewModel()
    {
        Tabs.Add(new TabContentViewModel());
        CurrentTab = Tabs.First();

        CloseAppCmd = new ActionCommand(OnCloseAppCmdExecuted, CanCloseAppCmdExecute);
        AddTabCmd = new ActionCommand(OnAddTabCmdExecuted, CanAddTabCmdExecute);
        CloseTabCmd = new ActionCommand(OnCloseTabCmdExecuted, CanCloseTabCmdExecute);
    }

    #endregion
}
