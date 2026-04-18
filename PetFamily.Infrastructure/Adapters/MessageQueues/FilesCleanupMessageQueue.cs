using PetFamily.Core.Application.UseCases.CommonDto;
using PetFamily.Core.Ports;
using System.Threading.Channels;

namespace PetFamily.Infrastructure.Adapters.MessageQueues;

public class FilesCleanupMessageQueue : IMessageQueueService<IEnumerable<PetPhotoDto>>
{
    private readonly Channel<IEnumerable<PetPhotoDto>> _channel =
        Channel.CreateUnbounded<IEnumerable<PetPhotoDto>>();

    public async Task<IEnumerable<PetPhotoDto>> ReadAsync(CancellationToken cancellationToken)
    {
        return await _channel.Reader.ReadAsync(cancellationToken);
    }

    public async Task WriteAsync(IEnumerable<PetPhotoDto> messages, CancellationToken cancellationToken)
    {
        await _channel.Writer.WriteAsync(messages, cancellationToken);
    }
}