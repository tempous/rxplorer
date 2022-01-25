using LiteExplorer.ViewModels.Base;

namespace LiteExplorer.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Title
        private string title = "LiteExplorer";

        public string Title
        {
            get => title;
            set => SetValue(ref title, value);
        }
        #endregion
    }
}
