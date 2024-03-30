using FonTech.Application.Dependency_Injection;
using FonTech.Producer.DependencyInjection;
using FonTech.Consumer.DependecyInjection;
using FonTech.DAL.DependencyInjection;
using FonTech.Domain.Settings;
using FonTech.Api.MiddleWares;
using FonTech.Api;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(nameof(RabbitMqSettings)));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.DefaultSettings));

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddProducer();
builder.Services.AddConsumer();

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();
builder.Services.AddAuthenticationAndAuthorization(builder);
builder.Services.AddSwager();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

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

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();

app.MapControllers();

app.Run();