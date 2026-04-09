using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using SportsStore.Core.Messaging.Contracts;
using SportsStore.Infrastructure.Messaging;

namespace SportsStore.ShippingService.Consumers;

public class ShippingConsumer : BackgroundService
{
    private readonly RabbitMqConnection _connection;
    private IChannel? _channel;
    private AsyncEventingBasicConsumer? _consumer;

    public ShippingConsumer(RabbitMqConnection connection)
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
                queue: "shipping-service",
                durable: true,
                exclusive: false,
                autoDelete: false);

            await _channel.QueueBindAsync(
                queue: "shipping-service",
                exchange: "sportsstore",
<<<<<<< HEAD
                routingKey: "ShippingRequested");
=======
                routingKey: "PaymentApproved");
>>>>>>> d53dbaa649fb2b7845b5dc91ce1f794377a85d00

            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

            _consumer = new AsyncEventingBasicConsumer(_channel);
            _consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
<<<<<<< HEAD
                    var shippingRequest = JsonSerializer.Deserialize<ShippingRequested>(message);

                    if (shippingRequest != null)
                    {
                        Log.Information("Shipping Service: Processing shipment for Order {OrderId}",
                            shippingRequest.OrderId);

                        await ProcessShipping(shippingRequest);
=======
                    var paymentApproved = JsonSerializer.Deserialize<PaymentApproved>(message);

                    if (paymentApproved != null)
                    {
                        Log.Information("Shipping Service: Processing shipment for Order {OrderId}",
                            paymentApproved.OrderId);

                        await ProcessShipping(paymentApproved);
>>>>>>> d53dbaa649fb2b7845b5dc91ce1f794377a85d00

                        await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Shipping Service: Error processing shipment");
                    await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: "shipping-service",
                autoAck: false,
                consumer: _consumer);

            Log.Information("Shipping Service: Started consuming messages from queue 'shipping-service'");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            Log.Information("Shipping Service: Consumer cancelled");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Shipping Service: Error in consumer");
            throw;
        }
    }

<<<<<<< HEAD
    private async Task ProcessShipping(ShippingRequested request)
=======
    private async Task ProcessShipping(PaymentApproved payment)
>>>>>>> d53dbaa649fb2b7845b5dc91ce1f794377a85d00
    {
        // Simulate shipping processing delay
        await Task.Delay(Random.Shared.Next(100, 500));

<<<<<<< HEAD
        var trackingNumber = $"SS{DateTime.UtcNow:yyyyMMdd}{request.OrderId:D6}{Random.Shared.Next(1000):D3}";
=======
        var trackingNumber = $"SS{DateTime.UtcNow:yyyyMMdd}{payment.OrderId:D6}{Random.Shared.Next(1000):D3}";
>>>>>>> d53dbaa649fb2b7845b5dc91ce1f794377a85d00
        var carriers = new[] { "FedEx", "UPS", "DHL", "USPS" };
        var carrier = carriers[Random.Shared.Next(carriers.Length)];
        var estimatedDispatch = DateTime.UtcNow.AddDays(Random.Shared.Next(1, 5));

        var shippingCreated = new ShippingCreated
        {
<<<<<<< HEAD
            OrderId = request.OrderId,
            CustomerId = request.CustomerId,
            CorrelationId = request.CorrelationId,
=======
            OrderId = payment.OrderId,
            CorrelationId = payment.CorrelationId,
>>>>>>> d53dbaa649fb2b7845b5dc91ce1f794377a85d00
            TrackingNumber = trackingNumber,
            Carrier = carrier,
            EstimatedDispatchDate = estimatedDispatch,
            Timestamp = DateTime.UtcNow
        };

        await PublishEvent(shippingCreated, "ShippingCreated");

        // Also publish order completed
        var orderCompleted = new OrderCompleted
        {
<<<<<<< HEAD
            OrderId = request.OrderId,
            CustomerId = request.CustomerId,
            CorrelationId = request.CorrelationId,
            TotalAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice),
=======
            OrderId = payment.OrderId,
            CorrelationId = payment.CorrelationId,
            TotalAmount = payment.Amount,
>>>>>>> d53dbaa649fb2b7845b5dc91ce1f794377a85d00
            CompletedAt = DateTime.UtcNow
        };

        await PublishEvent(orderCompleted, "OrderCompleted");

        Log.Information("Shipping Service: Shipment created for Order {OrderId}. Tracking: {TrackingNumber}, Carrier: {Carrier}",
<<<<<<< HEAD
            request.OrderId, trackingNumber, carrier);
=======
            payment.OrderId, trackingNumber, carrier);
>>>>>>> d53dbaa649fb2b7845b5dc91ce1f794377a85d00
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
