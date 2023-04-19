using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;

namespace Poc.Sl.LoggerApp
{
    internal static class HttpClientEventParserHelper
    {
        /// <summary>
        /// System.Net.Http.HttpClient
        /// </summary>
        public static string LoggerNameBeggin => "System.Net.Http.HttpClient";

        /// <summary>
        /// LogicalHandler
        /// </summary>
        public static string LoggerNameEnd => "LogicalHandler";

        /// <summary>
        /// MessageJson
        /// </summary>
        public static string EventName => "MessageJson";

        public static void Parse(TraceEvent traceEvent) 
        {
            var eventId = Convert.ToInt32(traceEvent.PayloadByName("EventId"));
            var eventName = traceEvent.PayloadByName("EventName")?.ToString();

            // ActivityId : Guid? 
            // traceEvent.ActivityID - is correlation id.
            // IMPORTANT:
            // in debug mode, ActivityID will be not correct for eventId 101 and 103
            Console.WriteLine($"{traceEvent.ActivityID} | {eventName}");
            if (eventId == 100)
                HttpClientEventParserHelper.ParseRequestPipelineStart(traceEvent);
            else if (eventId == 101)
                HttpClientEventParserHelper.ParseRequestPipelineEnd(traceEvent);
            else if (eventId == 102)
                HttpClientEventParserHelper.ParseRequestPipelineRequestHeader(traceEvent);
            else if (eventId == 103)
                HttpClientEventParserHelper.ParseRequestPipelineResponseHeader(traceEvent);
        }

        /// <summary>
        /// eventId 100 - eventName RequestPipelineStart
        /// </summary>
        /// <param name="traceEvent"></param>
        public static void ParseRequestPipelineStart(TraceEvent traceEvent)
        {
            var argumentsAsJson = traceEvent.PayloadByName("ArgumentsJson")?.ToString();
            var arguments = JsonSerializer.Deserialize<Dictionary<string, string>>(argumentsAsJson);
            var httpMethod = arguments["HttpMethod"];
            var uri = arguments["Uri"];
        }

        /// <summary>
        /// eventId 102 - eventName RequestPipelineRequestHeader
        /// </summary>
        /// <param name="traceEvent"></param>
        public static void ParseRequestPipelineRequestHeader(TraceEvent traceEvent)
        {
            var headersAsText = traceEvent.PayloadByName("FormattedMessage")?.ToString();

            var result = ParseHeaders(headersAsText);
        }

        /// <summary>
        /// eventId 101 - eventName RequestPipelineEnd
        /// </summary>
        /// <param name="traceEvent"></param>
        public static void ParseRequestPipelineEnd(TraceEvent traceEvent)
        {
            var argumentsAsJson = traceEvent.PayloadByName("ArgumentsJson")?.ToString();
            var arguments = JsonSerializer.Deserialize<Dictionary<string, string>>(argumentsAsJson);
            var elapsedMilliseconds = arguments["ElapsedMilliseconds"];
            var statusCode = arguments["StatusCode"];
        }

        /// <summary>
        /// eventId 103 - eventName RequestPipelineResponseHeader
        /// </summary>
        /// <param name="traceEvent"></param>
        public static void ParseRequestPipelineResponseHeader(TraceEvent traceEvent)
        {
            var headersAsText = traceEvent.PayloadByName("FormattedMessage")?.ToString();
            
            var result = ParseHeaders(headersAsText);
        }

        private static string ParseHeaders(string headersAsText)
        {
            var headers = headersAsText.Split('\n').Skip(1);
            var headersDictionary = new Dictionary<string, string>();
            foreach (var header in headers)
            {
                var deviderIndex = header.IndexOf(':');
                if (deviderIndex == -1)
                {
                    continue;
                }
                var key = header.Substring(0, deviderIndex);
                var value = header.Substring(deviderIndex + 1);
                headersDictionary.Add(key, value);
            }
            return JsonSerializer.Serialize(headersDictionary);
        }
    }
}
