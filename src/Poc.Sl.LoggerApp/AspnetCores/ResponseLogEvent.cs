using Poc.Sl.LoggerApp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.Sl.LoggerApp.AspnetCores
{
    internal class ResponseLogEvent : BaseEvent
    {
        public override string CategoryName => "AspnetCore";

        public override string EventName => nameof(ResponseLogEvent);

        public int StatusCode { get; set; }

        public string? ContentType { get; set; }

        public Dictionary<string, string> Metadata { get; set; }
    }
}
