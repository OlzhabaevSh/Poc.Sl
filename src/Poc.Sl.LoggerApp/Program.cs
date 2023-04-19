using Microsoft.Diagnostics.NETCore.Client;
using Microsoft.Diagnostics.Tracing;
using Poc.Sl.LoggerApp;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Text.Json;

// get process by name
var process = ProcessHelper.GetProcessId(ProcessHelper.ProcessName);

if (!process.HasValue) 
{
    Console.WriteLine($"Process '{ProcessHelper.ProcessName}' is not started");
    return;
}

// prepare providers
var providers = EvenPipeProvidersHelper.GetProviders();

// configure event source
var client = new DiagnosticsClient(process.Value);
using EventPipeSession session = client.StartEventPipeSession(providers, false);
using var source = new EventPipeEventSource(session.EventStream);

source.Dynamic.All += TraceEventParserHelper.Parse;

// run
try 
{
    source.Process();
}
catch (Exception ex) 
{
    Console.Error.WriteLine(ex.ToString());
}

Console.ReadKey();