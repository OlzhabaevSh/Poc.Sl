using Microsoft.Diagnostics.NETCore.Client;
using Microsoft.Diagnostics.Tracing;
using System.Diagnostics;
using System.Diagnostics.Tracing;

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

var logs = new List<TraceEvent>();

source.Dynamic.All += (TraceEvent obj) =>
{
    var loggerName = obj.PayloadByName("LoggerName")?.ToString();

    if (loggerName == "System.Net.Http.HttpClient.Default.ClientHandler")
    {
        // get request url
        var requestUrl = obj.PayloadByName("RequestUrl")?.ToString();

        // get request headers
        var requestHeaders = obj.PayloadByName("RequestHeaders")?.ToString();

        logs.Add(obj);
    }
    else if (loggerName == "Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware") 
    {
        logs.Add(obj);
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

var res = logs.ToArray();

Console.ReadKey();