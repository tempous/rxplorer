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

        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            TabItems.Add(new TabItemViewModel());
            TabItems.Add(new TabItemViewModel());
            TabItems.Add(new TabItemViewModel());

            CurrentTabItem = TabItems.FirstOrDefault();

            CloseAppCmd = new ActionCommand(OnCloseAppCmdExecuted, CanCloseAppCmdExecute);
        }

        #endregion
    }
}
