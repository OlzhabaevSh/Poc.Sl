using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;
using Poc.Sl.LoggerApp.AspnetCores;
using Poc.Sl.LoggerApp.HttpClients;

namespace Poc.Sl.LoggerApp.Core
{
    internal static class TraceEventParserHelper
    {
        public static void Parse(TraceEvent traceEvent)
        {
            var loggerName = traceEvent.PayloadByName("LoggerName")?.ToString();

            if (loggerName == null)
                return;

            // logger name can be:
            // 1. System.Net.Http.HttpClient.Default.ClientHandler
            // 2. System.Net.Http.HttpClient.[http-client-name].LogicalHandler
            // Default - is default http client
            // [http-client-name] - is named http client
            if (FeatureFlags.UseHttpClient
                && loggerName.Contains(HttpClientEventParserHelper.LoggerNameBeggin)
                && loggerName.Contains(HttpClientEventParserHelper.LoggerNameEnd)
                && traceEvent.EventName == HttpClientEventParserHelper.EventName)
            {
                Console.WriteLine($"{traceEvent.Keywords} | {loggerName}");
                HttpClientEventParserHelper.Parse(traceEvent);
            }
            else if (FeatureFlags.UseAspnetCore
                && loggerName == AspnetCoreEventParserHelper.LoggerName
                && traceEvent.EventName == AspnetCoreEventParserHelper.EventName)
            {
                Console.WriteLine($"{traceEvent.Keywords} | {loggerName}");
                AspnetCoreEventParserHelper.Parse(traceEvent);
            }
        }
    }
}
