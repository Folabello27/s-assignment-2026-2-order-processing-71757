using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using SportsStore.Core.Messaging.Contracts;
using SportsStore.Infrastructure.Messaging;

namespace SportsStore.PaymentService.Consumers;

public class PaymentProcessingConsumer : BackgroundService
{
    private readonly RabbitMqConnection _connection;
    private IChannel? _channel;
    private AsyncEventingBasicConsumer? _consumer;

    // Test card numbers that will fail payment
    private static readonly HashSet<string> _rejectedCards = new(StringComparer.OrdinalIgnoreCase)
    {
        "9999999999999999",
        "0000000000000000",
        "DECLINED"
    };

    // Random rejection for simulation (10% failure rate)
    private readonly Random _random = new();

    public PaymentProcessingConsumer(RabbitMqConnection connection)
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
                queue: "payment-service",
                durable: true,
                exclusive: false,
                autoDelete: false);

            await _channel.QueueBindAsync(
                queue: "payment-service",
                exchange: "sportsstore",
                routingKey: "PaymentProcessingRequested");

            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

            _consumer = new AsyncEventingBasicConsumer(_channel);
            _consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var paymentRequest = JsonSerializer.Deserialize<PaymentProcessingRequested>(message);

                    if (paymentRequest != null)
                    {
                        Log.Information("Payment Service: Processing payment for Order {OrderId}, Amount: {Amount:C}",
                            paymentRequest.OrderId, paymentRequest.Amount);

                        await ProcessPayment(paymentRequest);

                        await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Payment Service: Error processing payment");
                    await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: "payment-service",
                autoAck: false,
                consumer: _consumer);

            Log.Information("Payment Service: Started consuming messages from queue 'payment-service'");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            Log.Information("Payment Service: Consumer cancelled");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Payment Service: Error in consumer");
            throw;
        }
    }

    private async Task ProcessPayment(PaymentProcessingRequested request)
    {
        // Simulate payment processing delay
        await Task.Delay(Random.Shared.Next(100, 500));

        bool shouldReject = false;
        string rejectionReason = string.Empty;

        // Check if card should be rejected
        if (!string.IsNullOrEmpty(request.PaymentMethod) &&
            _rejectedCards.Contains(request.PaymentMethod))
        {
            shouldReject = true;
            rejectionReason = "Card number is in the declined list";
        }
        // Random rejection (10% chance)
        else if (_random.Next(100) < 10)
        {
            shouldReject = true;
            rejectionReason = "Payment authorization failed - random rejection";
        }

        if (shouldReject)
        {
            var rejected = new PaymentRejected
            {
                OrderId = request.OrderId,
                CorrelationId = request.CorrelationId,
                Reason = rejectionReason,
                Amount = request.Amount,
                RejectedAt = DateTime.UtcNow
            };

            await PublishEvent(rejected, "PaymentRejected");

            Log.Warning("Payment Service: Payment rejected for Order {OrderId}. Reason: {Reason}",
                request.OrderId, rejectionReason);
        }
        else
        {
            var transactionId = $"TXN{DateTime.UtcNow:yyyyMMdd}{request.OrderId:D6}{_random.Next(1000):D3}";

            var approved = new PaymentApproved
            {
                OrderId = request.OrderId,
                CorrelationId = request.CorrelationId,
                TransactionId = transactionId,
                Amount = request.Amount,
                ApprovedAt = DateTime.UtcNow
            };

            await PublishEvent(approved, "PaymentApproved");

            Log.Information("Payment Service: Payment approved for Order {OrderId}. Transaction: {TransactionId}",
                request.OrderId, transactionId);
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
