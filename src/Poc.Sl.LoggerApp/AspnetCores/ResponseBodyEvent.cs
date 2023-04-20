using Poc.Sl.LoggerApp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.Sl.LoggerApp.AspnetCores
{
    internal class ResponseBodyEvent : BaseEvent
    {
        public override string CategoryName => "AspnetCore";

        public override string EventName => nameof(ResponseBodyEvent);

        public string BodyAsString { get; set; }

        public Dictionary<string, string> Metadata { get; set; }
    }
}
