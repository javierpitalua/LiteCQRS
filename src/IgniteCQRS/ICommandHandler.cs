using System.Threading.Tasks;

namespace IgniteCQRS
{
    public interface ICommand
    {

    }

    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task<CommandResult> ExecuteAsync(TCommand command);
    }
}
