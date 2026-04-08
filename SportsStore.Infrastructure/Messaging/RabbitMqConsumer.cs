using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace SportsStore.Infrastructure.Messaging;

public abstract class RabbitMqConsumer<T> : BackgroundService where T : class
{
    private readonly RabbitMqConnection _connection;
    private readonly string _queueName;
    private readonly string _routingKey;
    private readonly string _exchangeName;
    private IChannel? _channel;
    private AsyncEventingBasicConsumer? _consumer;

    protected RabbitMqConsumer(
        RabbitMqConnection connection,
        string queueName,
        string routingKey,
        string exchangeName = "sportsstore")
    {
        _connection = connection;
        _queueName = queueName;
        _routingKey = routingKey;
        _exchangeName = exchangeName;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var connection = await _connection.GetConnectionAsync();
            _channel = await connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(
                exchange: _exchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);

            await _channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            await _channel.QueueBindAsync(
                queue: _queueName,
                exchange: _exchangeName,
                routingKey: _routingKey);

            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

            _consumer = new AsyncEventingBasicConsumer(_channel);
            _consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var eventMessage = JsonSerializer.Deserialize<T>(message);

                    if (eventMessage is not null)
                    {
                        Log.Information("Received message {MessageType} from queue {Queue}",
                            typeof(T).Name, _queueName);

                        await HandleMessageAsync(eventMessage, stoppingToken);

                        await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);

                        Log.Information("Successfully processed message {MessageType}",
                            typeof(T).Name);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error processing message {MessageType}", typeof(T).Name);
                    await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: _queueName,
                autoAck: false,
                consumer: _consumer);

            Log.Information("Started consuming messages from queue {Queue} with routing key {RoutingKey}",
                _queueName, _routingKey);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            Log.Information("RabbitMQ consumer for {Queue} cancelled", _queueName);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in RabbitMQ consumer for queue {Queue}", _queueName);
            throw;
        }
    }

    protected abstract Task HandleMessageAsync(T message, CancellationToken cancellationToken);

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync();
            _channel.Dispose();
        }
        await base.StopAsync(cancellationToken);
    }
}
