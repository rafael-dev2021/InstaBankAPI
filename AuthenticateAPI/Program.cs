using AuthenticateAPI.Endpoints;
using AuthenticateAPI.Extensions;
using AuthenticateAPI.Middleware;

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

app.UseCors("CorsPolicy");

app.UseMiddleware<SecurityFilterMiddleware>();
app.UseMiddleware<LogoutHandlerMiddleware>();
app.UseMiddleware<LoggingMiddleware>();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthenticateEndpoints();

app.Run();



