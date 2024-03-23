using FonTech.DAL.DependencyInjection;
using FonTech.Application.Dependency_Injection;
using FonTech.Api;
using Serilog;
using FonTech.Domain.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.DefaultSettings));

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddApplication();

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();
builder.Services.AddAuthenticationAndAuthorization(builder);
builder.Services.AddSwager();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(op =>
    {
        op.SwaggerEndpoint("/swagger/v1/swagger.json", "FonTech Swagger v1.0");
        op.SwaggerEndpoint("/swagger/v2/swagger.json", "FonTech Swagger v2.0");
        op.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();