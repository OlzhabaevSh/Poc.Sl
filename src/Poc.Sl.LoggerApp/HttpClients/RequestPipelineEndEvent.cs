using Poc.Sl.LoggerApp.AspnetCores;
using Poc.Sl.LoggerApp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.Sl.LoggerApp.HttpClients
{
    internal class RequestPipelineEndEvent : BaseEvent
    {
        public override string CategoryName => "HttpClient";

        public override string EventName => nameof(RequestPipelineEndEvent);

        public int StatusCode { get; set; }

        public TimeSpan ElapsedMilliseconds { get; set; }
    }
}
