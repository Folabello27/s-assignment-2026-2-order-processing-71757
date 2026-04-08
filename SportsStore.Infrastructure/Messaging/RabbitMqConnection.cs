using RabbitMQ.Client;

namespace SportsStore.Infrastructure.Messaging;

public class RabbitMqConnection : IAsyncDisposable
{
    private readonly IConnectionFactory _connectionFactory;
    private IConnection? _connection;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public RabbitMqConnection(string hostname, int port, string username, string password)
    {
        _connectionFactory = new ConnectionFactory
        {
            HostName = hostname,
            Port = port,
            UserName = username,
            Password = password,
            AutomaticRecoveryEnabled = true,
            TopologyRecoveryEnabled = true
        };
    }

    public async Task<IConnection> GetConnectionAsync()
    {
        if (_connection is { IsOpen: true })
            return _connection;

        await _semaphore.WaitAsync();
        try
        {
            if (_connection is not { IsOpen: true })
            {
                _connection = await _connectionFactory.CreateConnectionAsync();
            }
            return _connection;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.CloseAsync();
            _connection.Dispose();
        }
        _semaphore.Dispose();
    }
}
