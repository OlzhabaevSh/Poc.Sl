using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.Sl.LoggerApp.Core
{
    internal abstract class BaseEvent
    {
        public string EventKey { get; set; }
    }
}
