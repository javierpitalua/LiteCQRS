﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCQRS
{
    public interface ICommand
    {

    }

    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        CommandResult Execute(TCommand command);
    }
}
