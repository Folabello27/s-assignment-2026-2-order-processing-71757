using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using SportsStore.Core.Messaging.Contracts;
using SportsStore.Infrastructure.Messaging;

namespace SportsStore.InventoryService.Consumers;

public class InventoryCheckConsumer : BackgroundService
{
    private readonly RabbitMqConnection _connection;
    private IChannel? _channel;
    private AsyncEventingBasicConsumer? _consumer;

    // Simulated inventory database
    private static readonly Dictionary<int, int> _inventory = new()
    {
        [1] = 100,  // Kayak
        [2] = 200,  // Lifejacket
        [3] = 150,  // Soccer Ball
        [4] = 100,  // Corner Flags
        [5] = 10,   // Stadium
        [6] = 50,   // Thinking Cap
        [7] = 75,   // Unsteady Chair
        [8] = 30,   // Human Chess Board
        [9] = 15    // Bling-Bling King
    };

    public InventoryCheckConsumer(RabbitMqConnection connection)
    {
        _connection = connection;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var connection = await _connection.GetConnectionAsync();
            _channel = await connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(
                exchange: "sportsstore",
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);

            await _channel.QueueDeclareAsync(
                queue: "inventory-service",
                durable: true,
                exclusive: false,
                autoDelete: false);

            await _channel.QueueBindAsync(
                queue: "inventory-service",
                exchange: "sportsstore",
                routingKey: "OrderSubmitted");

            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

            _consumer = new AsyncEventingBasicConsumer(_channel);
            _consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var orderSubmitted = JsonSerializer.Deserialize<OrderSubmitted>(message);

                    if (orderSubmitted != null)
                    {
                        Log.Information("Inventory Service: Processing inventory check for Order {OrderId}",
                            orderSubmitted.OrderId);

                        await ProcessInventoryCheck(orderSubmitted);

                        await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Inventory Service: Error processing message");
                    await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: "inventory-service",
                autoAck: false,
                consumer: _consumer);

            Log.Information("Inventory Service: Started consuming messages from queue 'inventory-service'");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            Log.Information("Inventory Service: Consumer cancelled");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Inventory Service: Error in consumer");
            throw;
        }
    }

    private async Task ProcessInventoryCheck(OrderSubmitted order)
    {
        var stockAvailability = new Dictionary<int, int>();
        var insufficientStock = new List<string>();

        foreach (var item in order.Items)
        {
            if (_inventory.TryGetValue(item.ProductId, out var availableStock))
            {
                stockAvailability[item.ProductId] = availableStock;

                if (availableStock < item.Quantity)
                {
                    insufficientStock.Add($"{item.ProductName} (requested: {item.Quantity}, available: {availableStock})");
                }
            }
            else
            {
                insufficientStock.Add($"{item.ProductName} (product not found in inventory)");
            }
        }

        if (insufficientStock.Any())
        {
            // Publish inventory check failed
            var failed = new InventoryCheckFailed
            {
                OrderId = order.OrderId,
                CorrelationId = order.CorrelationId,
                Reason = $"Insufficient stock: {string.Join(", ", insufficientStock)}",
                CheckedAt = DateTime.UtcNow
            };

            await PublishEvent(failed, "InventoryCheckFailed");

            Log.Warning("Inventory Service: Check failed for Order {OrderId}: {Reason}",
                order.OrderId, failed.Reason);
        }
        else
        {
            // Reserve stock
            foreach (var item in order.Items)
            {
                _inventory[item.ProductId] -= item.Quantity;
            }

            // Publish inventory check completed
            var completed = new InventoryCheckCompleted
            {
                OrderId = order.OrderId,
                CorrelationId = order.CorrelationId,
                ReservedStock = order.Items.ToDictionary(i => i.ProductId, i => i.Quantity),
                StockAvailability = stockAvailability,
                CheckedAt = DateTime.UtcNow
            };

            await PublishEvent(completed, "InventoryCheckCompleted");

            Log.Information("Inventory Service: Check completed for Order {OrderId}. Stock reserved.",
                order.OrderId);
        }
    }

    private async Task PublishEvent<T>(T eventMessage, string routingKey) where T : class
    {
        var json = JsonSerializer.Serialize(eventMessage);
        var body = Encoding.UTF8.GetBytes(json);

        var props = new BasicProperties
        {
            ContentType = "application/json",
            MessageId = Guid.NewGuid().ToString(),
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
            Type = typeof(T).Name
        };

        await _channel.BasicPublishAsync(
            exchange: "sportsstore",
            routingKey: routingKey,
            mandatory: false,
            basicProperties: props,
            body: body);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null)
        {
            await _channel.CloseAsync();
            _channel.Dispose();
        }
        await base.StopAsync(cancellationToken);
    }
}
