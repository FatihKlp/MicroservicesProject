using Shared.DTOs;

namespace Shared.Interfaces
{
    public interface ILogServiceClient
    {
        Task SendLogAsync(LogEntryDto log);
    }
}
