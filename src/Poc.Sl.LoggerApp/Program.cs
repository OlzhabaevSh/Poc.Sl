using Microsoft.Diagnostics.NETCore.Client;
using Microsoft.Diagnostics.Tracing;
using Poc.Sl.LoggerApp;
using Poc.Sl.LoggerApp.Core;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Text.Json;

// get process by name
var processId = ProcessHelper.GetProcessId(ProcessHelper.ProcessName);

if (!processId.HasValue) 
{
    Console.WriteLine($"Process '{ProcessHelper.ProcessName}' is not started");
    return;
}

// configure event source
var eventObserver = EventObserver.Instance;
var consoleConsumer = new ConsoleConsumer();
using var consoleSubscribeDisposable = eventObserver.Subscribe(consoleConsumer);

// prepare providers and configure event source
var providers = EvenPipeProvidersHelper.GetProviders();
var client = new DiagnosticsClient(processId.Value);
using EventPipeSession session = client.StartEventPipeSession(providers, false);
using var source = new EventPipeEventSource(session.EventStream);
source.Dynamic.All += TraceEventHelper.Parse;

// run
try 
{
    Console.WriteLine($"Started: {processId.Value}");
    source.Process();
}
catch (Exception ex) 
{
    Console.Error.WriteLine(ex.ToString());
}

Console.ReadKey();