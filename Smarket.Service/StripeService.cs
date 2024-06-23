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
        public async Task<string> CreateCheckoutSession(IEnumerable<CartItem> orderItemsList, string successUrl)
        {
            try
            {
                var domain = "http://smarkeewet.great-site.net";
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = orderItemsList.Select(oi => new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(oi.Package.Price * 100),
                            Currency = "egp",
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
                    SuccessUrl = domain + "/confirm",
                    CancelUrl = domain + "/deny",
                };

                var service = new SessionService();
                Session session = service.Create(options);

                return session.Url;
            }
            catch (Exception)
            {
                throw new Exception("An error occurred during checkout.");
            }
        }
    }
}