using BankingServiceAPI.Extensions;
using BankingServiceAPI.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructureModule();
builder.Services.AddOpenApiExtensions();
builder.Services.AddDependencyInjectionJwt();
builder.Services.AddRedisCacheDependencyInjection();
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<TokenValidationMiddleware>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseMiddleware<TokenValidationMiddleware>();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<LoggingMiddleware>();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.AddMapEndpointsDependencyInjection();

await app.RunAsync();