using LiteExplorer.Infrastructure.Commands.Base;
using System;

namespace LiteExplorer.Infrastructure.Commands;

internal class ActionCommand : Command
{
    private readonly Action<object> execute;
    private readonly Predicate<object> canExecute;

    public ActionCommand(Action<object> execute, Predicate<object> canExecute = null)
    {
        this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
        this.canExecute = canExecute;
    }

    public override bool CanExecute(object parameter) => canExecute == null || canExecute(parameter);

    public override void Execute(object parameter) => execute(parameter);
}