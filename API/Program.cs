using minimal_api;

var builder = WebApplication.CreateBuilder(args);

// Delega toda a configuração de serviços ao Startup
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

// Delega toda a configuração do pipeline ao Startup
startup.Configure(app, app.Environment);

app.Run();