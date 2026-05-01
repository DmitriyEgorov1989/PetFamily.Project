namespace PetFamily.Core.Ports;

public interface IMessageQueueService<TMessage>
{
    Task WriteAsync(TMessage message, CancellationToken cancellationToken);
    Task<TMessage> ReadAsync(CancellationToken cancellationToken);
}