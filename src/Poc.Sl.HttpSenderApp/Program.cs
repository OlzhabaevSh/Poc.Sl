using Microsoft.AspNetCore.HttpLogging;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add http client and httplogger
builder.Services.AddHttpClient();
builder.Services.AddHttpLogging(options => 
{
    options.LoggingFields = HttpLoggingFields.All;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpLogging();

app.UseHttpsRedirection();

app.MapGet("/bing", async (HttpClient httpClient) =>
{
    httpClient.DefaultRequestHeaders.Add("my-custom-header", "my-value");

    // make http get request to bing
    // get string response content
    // return response status code
    var response = await httpClient
        .GetAsync("https://www.bing.com")
        .ConfigureAwait(false);
    
    var content = await response.Content
        .ReadAsStringAsync()
        .ConfigureAwait(false);

    return response.StatusCode;
}).WithName("Http");

app.MapGet("/id", () =>
{
    // return current process id
    return Process.GetCurrentProcess().Id;
}).WithName("Process");

app.Run();