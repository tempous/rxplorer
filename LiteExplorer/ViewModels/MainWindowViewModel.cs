using LiteExplorer.Commands.Base;
using LiteExplorer.Models;
using LiteExplorer.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LiteExplorer.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Properties

        #region Title
        private string title = "LiteExplorer";

        public string Title
        {
            get => title;
            set => SetValue(ref title, value);
        }
        #endregion

        #region CurrentTabItem
        private TabItemViewModel currentTabItem;

        public TabItemViewModel CurrentTabItem
        {
            get => currentTabItem;
            set => SetValue(ref currentTabItem, value);
        }
        #endregion

        public ObservableCollection<TabItemViewModel> TabItems { get; set; } = new();

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
            var newTab = new TabItemViewModel();
            TabItems.Add(newTab);
            CurrentTabItem = newTab;
        }
        #endregion

        #region CloseTab
        public ICommand CloseTabCmd { get; }
        private bool CanCloseTabCmdExecute(object p) => TabItems.Count > 1;
        private void OnCloseTabCmdExecuted(object p)
        {
            if (p is TabItemViewModel closeTab)
            {
                TabItems.Remove(closeTab);
                CurrentTabItem = TabItems.LastOrDefault();
            }
        }
        #endregion

        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            TabItems.Add(new TabItemViewModel());
            CurrentTabItem = TabItems.FirstOrDefault();

            CloseAppCmd = new ActionCommand(OnCloseAppCmdExecuted, CanCloseAppCmdExecute);
            AddTabCmd = new ActionCommand(OnAddTabCmdExecuted, CanAddTabCmdExecute);
            CloseTabCmd = new ActionCommand(OnCloseTabCmdExecuted, CanCloseTabCmdExecute);
        }

        #endregion
    }
}
