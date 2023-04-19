using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;

namespace Poc.Sl.LoggerApp
{
    internal static class AspnetCoreEventParserHelper
    {
        public static string LoggerName => "Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware";

        public static string EventName => "MessageJson";

        public static void Parse(TraceEvent traceEvent)
        {
            // if you run the app via VS -> for responce's activity ids will be incorrect.
            var eventId = Convert.ToInt32(traceEvent.PayloadByName("EventId"));
            var eventName = traceEvent.PayloadByName("EventName")?.ToString();

            Console.WriteLine($"{traceEvent.ActivityID} | {traceEvent.RelatedActivityID} | {eventName} | {eventId}");
        }
    }
}
