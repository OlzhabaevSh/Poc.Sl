using Poc.Sl.LoggerApp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.Sl.LoggerApp.AspnetCores
{
    internal class RequestLogEvent : BaseEvent
    {
        public string Protocol { get; set; }

        public string Method { get; set; }

        public string Scheme { get; set; }

        public string Host { get; set; }

        public string Path { get; set; }

        public Dictionary<string, string> Metadata { get; set; }
    }
}
