using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCQRS
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
