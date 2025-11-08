using PIV.Models;

namespace PIV.interfaces
{
    public interface IPrevisionService
    {
        Task<string?> GenerateContent1(List<Weather> weatherData, string date, CancellationToken cancellationToken = default);
    }
}
