using System.Net.Http.Json;
using Shared.DTOs;
using Shared.Interfaces;

namespace Shared.Services
{
    public class LogServiceClient : ILogServiceClient
    {
        private readonly HttpClient _httpClient;

        public LogServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SendLogAsync(LogEntryDto log)
        {
            await _httpClient.PostAsJsonAsync("/api/logs", log);
        }
    }
}
