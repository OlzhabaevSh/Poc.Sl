using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;
using Poc.Sl.LoggerApp.Core;

namespace Poc.Sl.LoggerApp.AspnetCores
{
    internal static class AspnetCoreEventParserHelper
    {
        public static string LoggerName => "Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware";

        /// <summary>
        /// keyward 263882790666248
        /// </summary>
        public static string EventName => "MessageJson";

        public static void Parse(TraceEvent traceEvent)
        {
            // if you run the app via VS -> for responce's activity ids will be incorrect.
            var eventId = Convert.ToInt32(traceEvent.PayloadByName("EventId"));
            var eventName = traceEvent.PayloadByName("EventName")?.ToString();

            Debug.WriteLine($"{traceEvent.ActivityID} | {traceEvent.RelatedActivityID} | {eventName} | {eventId}");

            BaseEvent be;

            if (eventId == 1)
                be = ParseRequestLog(traceEvent);
            else if (eventId == 2)
                be = ParseResponseLog(traceEvent);
            else if (eventId == 3)
                be = ParseRequestBody(traceEvent);
            else if (eventId == 4)
                be = ParseRequestBody(traceEvent);

            // send notification
        }

        /// <summary>
        /// eventId 1 - eventName RequestLog
        /// </summary>
        /// <param name="traceEvent"></param>
        public static RequestLogEvent ParseRequestLog(TraceEvent traceEvent)
        {
            var argumentsAsJson = traceEvent.PayloadByName("ArgumentsJson")?.ToString();
            var arguments = JsonSerializer.Deserialize<Dictionary<string, string>>(argumentsAsJson);

            // protocol: http/2
            arguments.Remove("Protocol", out var protocol);
            // method: GET
            arguments.Remove("Method", out var method);
            // scheme: https
            arguments.Remove("Scheme", out var scheme);
            // host: localhost:5001
            arguments.Remove("Host", out var host);
            // path: /WeatherForecast
            arguments.Remove("Path", out var path);

            // all other is headers or metadata

            // create event
            return new RequestLogEvent
            {
                EventKey = traceEvent.ActivityID.ToString(),
                Protocol = protocol,
                Method = method,
                Scheme = scheme,
                Host = host,
                Path = path,
                Metadata = arguments
            };
        }

        /// <summary>
        /// eventId 2 - eventName ResponseLog
        /// </summary>
        /// <param name="traceEvent"></param>
        public static ResponseLogEvent ParseResponseLog(TraceEvent traceEvent)
        {
            var argumentsAsJson = traceEvent.PayloadByName("ArgumentsJson")?.ToString();
            var arguments = JsonSerializer.Deserialize<Dictionary<string, string>>(argumentsAsJson);

            // statusCode: 200
            arguments.Remove("StatusCode", out var statusCode);

            // contentType: application/json; charset=utf-8
            // can be null
            arguments.Remove("ContentType", out var contentType);

            // all other is headers or metadata

            // create event
            return new ResponseLogEvent
            {
                EventKey = traceEvent.ActivityID.ToString(),
                StatusCode = Convert.ToInt32(statusCode),
                ContentType = contentType,
                Metadata = arguments
            };
        }

        /// <summary>
        /// eventId 3 - eventName RequestBody
        /// </summary>
        /// <param name="traceEvent"></param>
        public static RequestBodyEvent ParseRequestBody(TraceEvent traceEvent)
        {
            var argumentsAsJson = traceEvent.PayloadByName("ArgumentsJson")?.ToString();
            var arguments = JsonSerializer.Deserialize<Dictionary<string, string>>(argumentsAsJson);

            // remove "{OriginalFormat}", because it is used for formatting
            arguments.Remove("{OriginalFormat}");

            // body
            arguments.Remove("Body", out var body);

            // all other is headers or metadata

            // create event
            return new RequestBodyEvent
            {
                EventKey = traceEvent.ActivityID.ToString(),
                BodyAsString = body,
                Metadata = arguments
            };
        }

        /// <summary>
        /// eventId 4 - eventName ResponseBody
        /// </summary>
        /// <param name="traceEvent"></param>
        public static ResponseBodyEvent ParseResponseBody(TraceEvent traceEvent)
        {
            var argumentsAsJson = traceEvent.PayloadByName("ArgumentsJson")?.ToString();
            var arguments = JsonSerializer.Deserialize<Dictionary<string, string>>(argumentsAsJson);

            // remove "{OriginalFormat}", because it is used for formatting
            arguments.Remove("{OriginalFormat}");

            // body
            arguments.Remove("Body", out var body);

            // all other is headers or metadata

            // create event
            return new ResponseBodyEvent
            {
                EventKey = traceEvent.ActivityID.ToString(),
                BodyAsString = body,
                Metadata = arguments
            };
        }
    }
}
