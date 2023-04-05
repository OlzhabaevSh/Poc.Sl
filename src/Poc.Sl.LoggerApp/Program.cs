using Microsoft.Diagnostics.NETCore.Client;
using Microsoft.Diagnostics.Tracing;
using Poc.Sl.LoggerApp;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Diagnostics.Tracing;
using System.Text.Json;
using System.Text.Json.Serialization;

var processName = "Poc.Sl.HttpSenderApp";

// get process by name
var process = Process
    .GetProcessesByName(processName)
    .FirstOrDefault();

if (process == null) 
{
    Console.WriteLine($"Process '{processName}' is not started");
    return;
}

var processId = process.Id;

var providers = new List<EventPipeProvider>()
{
    new EventPipeProvider(
        "Microsoft-Extensions-Logging",
        EventLevel.LogAlways,
        long.MaxValue),
    new EventPipeProvider(
        "System.Net.Http",
        EventLevel.LogAlways,
        long.MaxValue),
    new EventPipeProvider(
        "Microsoft.AspNetCore.HttpLogging",
        EventLevel.LogAlways,
        long.MaxValue),
};

var client = new DiagnosticsClient(processId);
using EventPipeSession session = client.StartEventPipeSession(providers, false);
using var source = new EventPipeEventSource(session.EventStream);

source.Dynamic.All += (TraceEvent obj) =>
{
    var loggerName = obj.PayloadByName("LoggerName")?.ToString();

    // this check convers only default httpClient. 
    // named http clients will have "System.Net.Http.HttpClient.[http-client-name].LogicalHandler"
    // to fix it we need to:
    // check if loggerName contains "System.Net.Http.HttpClient" and "LogicalHandler"
    // another way is create a regex
    if (loggerName == "System.Net.Http.HttpClient.Default.LogicalHandler")
    {
        if (obj.EventName != "MessageJson") 
        {
            return;
        }

        var eventId = Convert.ToInt32(obj.PayloadByName("EventId"));
        var eventName = obj.PayloadByName("EventName")?.ToString();
        var argumentsAsJson = obj.PayloadByName("ArgumentsJson")?.ToString();

        // ActivityId : Guid? 
        // if null - ignore
        // if not null -> need to parse and use as correlation id
        Console.WriteLine();
        Console.WriteLine(eventName);
        Console.WriteLine($"{obj.ThreadID} | ThreadID");
        Console.WriteLine($"{obj.ActivityID} | ActivityID");
        Console.WriteLine("- - - - - - - - - - - - - -");

        // eventId 100 - eventName RequestPipelineStart
        if (eventId == 100)
        {
            var arguments = JsonSerializer.Deserialize<Dictionary<string, string>>(argumentsAsJson);
            var httpMethod = arguments["HttpMethod"];
            var uri = arguments["Uri"];
        }
        // eventId 101 - eventName RequestPipelineEnd
        else if (eventId == 101)
        {
            var arguments = JsonSerializer.Deserialize<Dictionary<string, string>>(argumentsAsJson);
            var elapsedMilliseconds = arguments["ElapsedMilliseconds"];
            var statusCode = arguments["StatusCode"];
        }
        // eventId 102 - eventName RequestPipelineRequestHeader
        else if (eventId == 102)
        {
            var headersAsText = obj.PayloadByName("FormattedMessage")?.ToString();
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
        }
        // eventId 103 - eventName RequestPipelineResponseHeader
        else if (eventId == 103) 
        {
            var headersAsText = obj.PayloadByName("FormattedMessage")?.ToString();
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
        }

        var payload = new Dictionary<string, object>();

        foreach (var name in obj.PayloadNames) 
        {
            payload.Add(name, obj.PayloadByName(name));
        }
    }
    else if (loggerName == "Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware") 
    {
        // process asp.net core request/response logs
    }
};

try 
{
    source.Process();
}
catch (Exception ex) 
{
    Console.Error.WriteLine(ex.ToString());
}

Console.ReadKey();