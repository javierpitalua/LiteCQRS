using System.Threading.Tasks;

namespace IgniteCQRS
{
    public interface IEvent
    {
        //one change
    }

    public interface IEventHandler<in T> where T : IEvent
    {
        Task Handle(T e);
    }
}
