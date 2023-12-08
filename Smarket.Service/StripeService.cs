using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Smarket.Models;
using Smarket.Services.IServices;
using Stripe.Checkout;

namespace Smarket.Services
{
    public class StripeService : IStripeService
    {
        public StripeService()
        {
        }

        public async Task<string> CreateCheckoutSession(IEnumerable<OrderItem> orderItemsList)
        {
            try
            {
                var domain = "http://127.0.0.1:5500/hello.html";
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = orderItemsList.Select(oi => new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(oi.Package.Price * 100),
                            Currency = "egy",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = oi.Package.Product.Name,
                                Description = oi.Package.Product.Description,
                                Images = new List<string> { oi.Package.Product.Image.Url },
                            },
                        },
                        Quantity = oi.Quantity,
                    }).ToList(),
                    Mode = "payment",
                    SuccessUrl = domain + "/Done.html",
                    CancelUrl = domain + "/fool.html",
                };

                var service = new SessionService();
                Session session = service.Create(options);

                return session.Url;
            }
            catch (Exception)
            {
                // Handle the exception, you might want to log it or perform other actions
                throw new Exception("An error occurred during checkout.");
            }
        }
    }
}