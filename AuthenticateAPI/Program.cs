using AuthenticateAPI.Endpoints;
using AuthenticateAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureModule(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await UserRolesData.AddUserRolesDataAsync(app);

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthenticateEndpoints();

app.Run();


