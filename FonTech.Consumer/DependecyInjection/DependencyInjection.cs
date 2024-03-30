using Microsoft.Extensions.DependencyInjection;

namespace FonTech.Consumer.DependecyInjection;

public static class DependencyInjection
{
    public static void AddConsumer(this IServiceCollection services)
    {
        services.AddHostedService<RabbitMqListener>();
    }
}
