using System;

namespace LiteExplorer.Commands.Base
{
    internal class ActionCommand : Command
    {
        private readonly Action<object> execute;
        private readonly Func<object, bool> canExecute;

        public ActionCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public override bool CanExecute(object parameter) => canExecute == null || canExecute(parameter);

        public override void Execute(object parameter) => execute(parameter);
    }
}