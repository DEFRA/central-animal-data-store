using CAS.Api.Bootstrap;
using CAS.Core;
using FluentValidation;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddCustomTrustStore();
builder.Services.AddHttpContextAccessor();
builder.Host.UseSerilog(CdpLogging.Configuration);
builder.Services.AddHttpClient("DefaultClient").AddHeaderPropagation();
builder.Services.AddTransient<ProxyHttpMessageHandler>();
builder.Services.AddHttpClient("proxy").ConfigurePrimaryHttpMessageHandler<ProxyHttpMessageHandler>();
builder.Services.AddHeaderPropagation(options =>
{
    var traceHeader = builder.Configuration.GetValue<string>("TraceHeader");
    if (!string.IsNullOrWhiteSpace(traceHeader))
    {
        options.Headers.Add(traceHeader);
    }
});
builder.Services.AddHealthChecks();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddControllers();
builder.Services.AddCasCore();

var app = builder.Build();
//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
