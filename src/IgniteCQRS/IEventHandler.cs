namespace IgniteCQRS
{
    public interface IEvent
    {
        //one change
    }

    public interface IEventHandler<in T> where T : IEvent
    {
        void Handle(T e);
    }
}
