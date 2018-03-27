namespace IgniteCQRS
{
    public interface ICommand
    {

    }

    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        CommandResult Execute(TCommand command);
    }
}
