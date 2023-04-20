using Poc.Sl.LoggerApp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.Sl.LoggerApp.HttpClients
{
    internal class RequestPipelineRequestHeaderEvent : BaseEvent
    {
        public Dictionary<string, string> Headers { get; set; }
    }
}
