using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using SportsStore.Core.Domain.Interfaces;

namespace SportsStore.Infrastructure.Messaging;

public class RabbitMqPublisher : IEventPublisher, IAsyncDisposable
{
    private readonly RabbitMqConnection _connection;
    private readonly string _exchangeName;
    private IChannel? _channel;

    public RabbitMqPublisher(RabbitMqConnection connection, string exchangeName = "sportsstore")
    {
        _connection = connection;
        _exchangeName = exchangeName;
    }

    private async Task<IChannel> GetChannelAsync()
    {
        if (_channel is { IsOpen: true })
            return _channel;

        var connection = await _connection.GetConnectionAsync();
        _channel = await connection.CreateChannelAsync();

        await _channel.ExchangeDeclareAsync(
            exchange: _exchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);

        return _channel;
    }

    public async Task PublishAsync<T>(T message, string? routingKey = null) where T : class
    {
        var channel = await GetChannelAsync();
        var typeName = typeof(T).Name;
        var key = routingKey ?? typeName;

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var props = new BasicProperties
        {
            ContentType = "application/json",
            MessageId = Guid.NewGuid().ToString(),
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
            Type = typeName
        };

        await channel.BasicPublishAsync(
            exchange: _exchangeName,
            routingKey: key,
            mandatory: false,
            basicProperties: props,
            body: body);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync();
            _channel.Dispose();
        }
    }
}
