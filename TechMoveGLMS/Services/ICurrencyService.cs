using System.Threading.Tasks;

namespace TechMoveGLMS.Services
{
    public interface ICurrencyService
    {
        Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency);
    }
}