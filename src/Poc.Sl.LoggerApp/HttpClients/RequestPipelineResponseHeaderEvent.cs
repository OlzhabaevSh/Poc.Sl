using Poc.Sl.LoggerApp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.Sl.LoggerApp.HttpClients
{
    internal class RequestPipelineResponseHeaderEvent : BaseEvent
    {
        public override string CategoryName => "HttpClient";

        public override string EventName => nameof(RequestPipelineResponseHeaderEvent);

        public Dictionary<string, string> Headers { get; set; }
    }
}
