using AuthenticateAPI.Endpoints;
using AuthenticateAPI.Extensions;
using AuthenticateAPI.Middleware;
using AuthenticateAPI.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApiExtensions();
builder.Services.AddInfrastructureModule();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await UserRolesData.AddUserRolesDataAsync(app);

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<CustomLogoutHandler>();
app.UseMiddleware<SecurityFilter>();
app.UseMiddleware<LoggingMiddleware>();

app.MapAuthenticateEndpoints();

app.Run();



