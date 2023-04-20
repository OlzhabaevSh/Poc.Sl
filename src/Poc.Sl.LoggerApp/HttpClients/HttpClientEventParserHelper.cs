using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;
using Poc.Sl.LoggerApp.Core;

namespace Poc.Sl.LoggerApp.HttpClients
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
        /// MessageJson. keyward 263882790666248
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
            Debug.WriteLine($"{traceEvent.ActivityID} | {eventName}");

            BaseEvent be;

            if (eventId == 100)
                be = ParseRequestPipelineStart(traceEvent);
            else if (eventId == 101)
                be = ParseRequestPipelineEnd(traceEvent);
            else if (eventId == 102)
                be = ParseRequestPipelineRequestHeader(traceEvent);
            else if (eventId == 103)
                be = ParseRequestPipelineResponseHeader(traceEvent);

            // send notification
        }

        /// <summary>
        /// eventId 100 - eventName RequestPipelineStart
        /// </summary>
        /// <param name="traceEvent"></param>
        public static RequestPipelineStartEvent ParseRequestPipelineStart(TraceEvent traceEvent)
        {
            var argumentsAsJson = traceEvent.PayloadByName("ArgumentsJson")?.ToString();
            var arguments = JsonSerializer.Deserialize<Dictionary<string, string>>(argumentsAsJson);
            var httpMethod = arguments["HttpMethod"];
            var uri = arguments["Uri"];

            return new RequestPipelineStartEvent
            {
                EventKey = traceEvent.ActivityID.ToString(),
                HttpMethod = httpMethod,
                Uri = uri
            };
        }

        /// <summary>
        /// eventId 102 - eventName RequestPipelineRequestHeader
        /// </summary>
        /// <param name="traceEvent"></param>
        public static RequestPipelineRequestHeaderEvent ParseRequestPipelineRequestHeader(TraceEvent traceEvent)
        {
            var headersAsText = traceEvent.PayloadByName("FormattedMessage")?.ToString();

            var result = ParseHeaders(headersAsText);

            return new RequestPipelineRequestHeaderEvent
            {
                EventKey = traceEvent.ActivityID.ToString(),
                Headers = result
            };
        }

        /// <summary>
        /// eventId 101 - eventName RequestPipelineEnd
        /// </summary>
        /// <param name="traceEvent"></param>
        public static RequestPipelineEndEvent ParseRequestPipelineEnd(TraceEvent traceEvent)
        {
            var argumentsAsJson = traceEvent.PayloadByName("ArgumentsJson")?.ToString();
            var arguments = JsonSerializer.Deserialize<Dictionary<string, string>>(argumentsAsJson);
            var elapsedMilliseconds = arguments["ElapsedMilliseconds"];
            var statusCode = arguments["StatusCode"];

            // parse from milisecond to TimeSpan
            return new RequestPipelineEndEvent
            {
                EventKey = traceEvent.ActivityID.ToString(),
                ElapsedMilliseconds = TimeSpan.FromMilliseconds(double.Parse(elapsedMilliseconds.ToString())),
                StatusCode = int.Parse(statusCode)
            };
        }

        /// <summary>
        /// eventId 103 - eventName RequestPipelineResponseHeader
        /// </summary>
        /// <param name="traceEvent"></param>
        public static RequestPipelineResponseHeaderEvent ParseRequestPipelineResponseHeader(TraceEvent traceEvent)
        {
            var headersAsText = traceEvent.PayloadByName("FormattedMessage")?.ToString();

            var result = ParseHeaders(headersAsText);

            return new RequestPipelineResponseHeaderEvent
            {
                EventKey = traceEvent.ActivityID.ToString(),
                Headers = result
            };
        }

        private static Dictionary<string, string> ParseHeaders(string headersAsText)
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
            return headersDictionary;
        }
    }
}
