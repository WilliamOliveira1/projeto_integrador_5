using Google.GenAI;
using PIV.interfaces;
using PIV.Models;
using PIV.Models.Dto;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PIV.ClientApp.src.services.Prevision
{
    public class PrevisionService : IPrevisionService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PrevisionService> _logger;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        public PrevisionService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<PrevisionService> logger)
        {
            _httpClient = new HttpClient();
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string?> GenerateContent1(List<Weather> weatherData, string date, CancellationToken cancellationToken = default)
        {            
            var apiKey = _configuration["OpenAI:ApiKey"];
            var client = new Client(apiKey: apiKey);

            var requestBody = new
            {
                contents = new[]
                {
                    new { text = $@"
                    Dada a seguinte lista de registros meteorológicos diários e a data alvo, utilize os dados da lista meteorológica fornecida e forneça uma previsão 
                    meteorológica estimada com precipitação média, temperatura média e umidade média para a próxima semana a partir da data em questão.
                    Não retorne nenhum texto apenas in plain text com quebra de linha para cada dado: precipitação média: (short text), umidade média: (short text), temperatura média (short text).
                    Data: {JsonSerializer.Serialize(new { date = date, weather = weatherData }, _jsonOptions)}" } 
                }
            };
            var json = JsonSerializer.Serialize(requestBody);
            var response = await client.Models.GenerateContentAsync(
              model: "gemini-2.5-flash", contents: json
            );
            var cand2 = response?.Candidates != null && response.Candidates.Count > 0 ? response.Candidates[0] : null;
            var text2 = cand2?.Content?.Parts != null && cand2.Content.Parts.Count > 0 ? cand2.Content.Parts[0].Text : "Erro ao processar os dados.";
            return text2;
        }
    }
}