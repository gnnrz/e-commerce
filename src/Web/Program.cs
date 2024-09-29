using CleanArchitecture.Application.Service;
using CleanArchitecture.Application.Validators;
using FluentValidation.AspNetCore;
using FluentValidation;
using Serilog;
using CleanArchitecture.Domain.State;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

// Configurando o Serilog como Logger
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) 
    .Enrich.FromLogContext()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) 
    .CreateLogger();


builder.Host.UseSerilog();


// Adicionando serviços necessários
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<OrderValidator>();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices();
builder.Services.AddSwaggerGen();

// Adicionando o serviço OrderService
builder.Services.AddSingleton<OrderState>();
builder.Services.AddScoped<OrderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        c.RoutePrefix = string.Empty;
    });
}
else
{
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseExceptionHandler(options => { });

app.MapControllers(); 

app.MapFallbackToFile("index.html");

app.Run();

public partial class Program { }
