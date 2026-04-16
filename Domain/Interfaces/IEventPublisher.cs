namespace SportsStore.Core.Domain.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync<T>(T message, string? routingKey = null) where T : class;
}
