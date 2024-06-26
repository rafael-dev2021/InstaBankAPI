using BankingServiceAPI.Extensions;
using BankingServiceAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructureModule();
builder.Services.AddOpenApiExtensions();
builder.Services.AddDependencyInjectionJwt();
builder.Services.AddAuthorization();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = false; 
    options.SingleLine = true;
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.AddMapEndpointsDependencyInjection();

await app.RunAsync();