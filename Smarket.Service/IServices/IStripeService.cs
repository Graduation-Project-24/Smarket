using Smarket.Models;

namespace Smarket.Services.IServices
{
    public interface IStripeService
    {
        Task<string> CreateCheckoutSession(IEnumerable<CartItem> orderItemsList);
    }
}