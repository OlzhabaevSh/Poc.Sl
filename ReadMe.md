# Poc. Smart Logger

## Description

This application should be used to receive and visualize HTTP related logs from external dotnet process.

## How it works

Dotnet has built-in mechanism for logging http related activities and send it. 

For this moment it works for asp.net core. 

### How to enable logs sending

1. Use AddHttpClient extension and inject HTTP client into your class

```cs
// PLACE: startup.cs or Pragram.cs

/* some services registration */

// 1. Add HTTP client
builder.Services.AddHttpClient();

// 2. Add logging for asp.net core endpoints.
builder.Services.AddHttpLogging(options => 
{
    // include all fields: body, headers, method, path and etc.
    options.LoggingFields = HttpLoggingFields.All;
});

var app = builder.Build();

// 3. Use logging for asp.net core endpoints.
app.UseHttpLogging();

```

2. Configure log levels in appsettings.json file

```json
{
  "Logging": {
	"LogLevel": {
	  /* provide what ever you want */
	},
	"EventSource": {
            "LogLevel": {
                "Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware": "Trace",
                "System.Net.Http.HttpClient": "Trace"
      }
    }
  }
}
```

* `EventSource` is log provider which send logs out of the process. 
* `System.Net.Http.HttpClient` is key for HttpClient. We need to have a `Trace` level to see headers
* `Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware` is key for asp.net core endpoints.

We are done! Now your asp.net core application will send logs and you can receive it and process out side of this process.

As an example you can look at [HttpSenderApp](src/Poc.Sl.HttpSenderApp)

### How to recieve logs

1. Create new console application
2. Install next libriaries 
    a. `Microsoft.Diagnostics.NETCore.Client`
    b. `Microsoft.Diagnostics.Tracing.TraceEvent`
3. Get process id of your asp.net core application (the app should be in nun mode)

```cs
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
```

4. Prepare a list of providers 
    a. `Microsoft-Extensions-Logging`
    b. `System.Net.Http`
    c. `Microsoft.AspNetCore.HttpLogging`

```cs
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
```

5. Configure a source

```cs
var client = new DiagnosticsClient(processId);
using EventPipeSession session = client.StartEventPipeSession(providers, false);
using var source = new EventPipeEventSource(session.EventStream);

source.Dynamic.All += (TraceEvent obj) =>
{
    var loggerName = obj.PayloadByName("LoggerName")?.ToString();

    if (loggerName == "System.Net.Http.HttpClient.Default.ClientHandler")
    {
        // process http client request/response logs
    }
    else if (loggerName == "Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware") 
    {
        // process asp.net core request/response logs
    }
};
```

6. Start listening

```cs
try 
{
    // will work until a extern process if finished.
    source.Process();
}
catch (Exception ex) 
{
    Console.Error.WriteLine(ex.ToString());
}
```

So we are good! Our app will recieve required logs.
You can find more information in this example [LoggerApp](src/Poc.Sl.LoggerApp)

## What is next?

1. Parse logs
2. Prepare unit tests and integration tests
3. Create an extension