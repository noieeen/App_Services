using System.Reflection;
using ApiSetup;
using Logging;


// var builder = WebApplication.CreateBuilder(args);

// builder.Host.UseSerilog();
var serviceName = Assembly.GetCallingAssembly().GetName().Name ?? "Service";
var serviceVersion = "1.0.0";
var attrs = new Dictionary<string, object>
{
    ["module.name"] = "Store Api"
};


// Configure resource for OpenTelemetry
// var resourceBuilder = ResourceBuilder.CreateDefault()
//     .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
//     .AddAttributes(new Dictionary<string, object>
//     {
//         ["module.name"] = "Store Api"
//     });

// builder.AddServiceDefaults(resourceBuilder);


// Add services to the container.

// builder.Services.AddControllers();
// builder.Services.AddDefaultApiServices(builder.Configuration);

// == Register the global exception handler == //
// builder.Services.AddProblemDetails();
// builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
// builder.Services.AddExceptionHandler<DivideByZeroExceptionHandler>();
// builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
// builder.Services.AddExceptionHandler<UnauthorizedExceptionHandler>();
// builder.Services.AddExceptionHandler<ForbiddenExceptionHandler>();
// builder.Services.AddExceptionHandler<GlobalExceptionHandler>(); // **Do last order, Handle other exception cases

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

var builder = WebApplication.CreateBuilder(args);


builder.AddDefaultApi(serviceName, serviceVersion, attrs);
builder.AddStructuredLogging();
var app = builder.Build();
app.UseDefaultApiPipeline();
// app.MapDefaultEndpoints(); // /health, /alive
app.Run();