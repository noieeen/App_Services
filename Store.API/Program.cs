using System.Reflection;
using Core;
using Core.ExceptionHandlers;
using OpenTelemetry.Resources;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
var serviceName = Assembly.GetCallingAssembly().GetName().Name ?? "Service";
var serviceVersion = "1.0.0";


// Configure resource for OpenTelemetry
var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
    .AddAttributes(new Dictionary<string, object>
    {
        ["module.name"] = "Store Api"
    });
builder.AddServiceDefaults(resourceBuilder);
builder.AddDefaultLogging();

// Add services to the container.

builder.Services.AddControllers();


// == Register the global exception handler == //
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<DivideByZeroExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<UnauthorizedExceptionHandler>();
builder.Services.AddExceptionHandler<ForbiddenExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>(); // **Do last order, Handle other exception cases

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
// Use the global exception handler[middleware]
app.UseExceptionHandler();

app.UseAuthorization();
app.MapDefaultEndpoints(); // /health, /alive
app.MapControllers();

app.Run();