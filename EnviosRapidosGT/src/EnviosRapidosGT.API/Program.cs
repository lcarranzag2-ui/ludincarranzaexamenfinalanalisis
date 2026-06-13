using EnviosRapidosGT.API.Data;
using EnviosRapidosGT.API.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=enviosrapidosgt.db";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Envios Rapidos GT API",
        Version = "v1",
        Description = "API para el sistema de gestion de envios y paqueteria a nivel nacional."
    });
});

builder.Services.AddSingleton<IEnvioService>(_ => new EnvioService(connectionString));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

var dbInit = new DatabaseInitializer(connectionString);
dbInit.Initialize();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Envios Rapidos GT API v1");
    c.RoutePrefix = string.Empty;
});

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
