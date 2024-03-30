using FonTech.Producer.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace FonTech.Producer;

public class Producer : IMessageProducer
{
    public void SendMessage<T>(T message, string routingKey, string? exchange = null)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        var json = JsonConvert.SerializeObject(message, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });
        var body = Encoding.UTF8.GetBytes(json);
        channel.BasicPublish(exchange, routingKey, body: body);
    }
}
