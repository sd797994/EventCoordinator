using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCoordinator.Common
{
    public class EventCoordinatorException : Exception
    {
        public EventCoordinatorException(string message) : base(message) { }
    }
}
