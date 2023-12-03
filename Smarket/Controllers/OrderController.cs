using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Smarket.DataAccess.Repository.IRepository;
using Smarket.Models;
using Smarket.Models.DTOs;
using Smarket.Services.IServices;
using Stripe.Checkout;
using Stripe;
using System.Security.Policy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Smarket.Controllers
{
    public class OrderController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public OrderController(IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var obj = await _unitOfWork.Order.FirstOrDefaultAsync(x => x.Id == id);
            if (obj is null)
                return NotFound();
            return Ok(new { Order = obj });
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _unitOfWork.Order.GetAllAsync(includeProperties: p => p.User);
            return Ok(orders);
        }

        [HttpGet]
        [Route("GetOrderByUser")]
        public async Task<IActionResult> GetOrderByUser(int id)
        {
            var user = await _unitOfWork.Order.GetAllAsync(x => x.UserId == id);

            return Ok(user);
        }
        [HttpGet]
        [Route("GetShoppingCartbyUser")]
        public async Task<IActionResult> GetShoppingCartbyUser(int id)
        {
            var user = await _unitOfWork.CartItem.GetAllAsync(x => x.UserId == id);

            return Ok(user);
        }

        [HttpPost]
        [Route("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto addToCartDto)
        {
            if (!ModelState.IsValid || addToCartDto == null)
            {
                return BadRequest(new { State = ModelState, AddToCartDto = addToCartDto });
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("User not found");
            }
            var package = await _unitOfWork.Package.FirstOrDefaultAsync(c => c.Id == addToCartDto.PackageId);
            if (package == null)
            {
                return NotFound("Product not found");
            }

            var cartItem = new CartItem
            {
                Quantity = addToCartDto.Quantity,
                PackageId = addToCartDto.PackageId,
                UserId = addToCartDto.UserId,
            };

            await _unitOfWork.CartItem.AddAsync(cartItem);
            await _unitOfWork.Save();

            return Ok(new { Message = "Product added to cart successfully" });
        }

        [HttpPost]
        [Route("RemoveFromCart")]
        public async Task<IActionResult> RemoveFromCart(int packageId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return NotFound("User not found");
                }

                var cartItem = await _unitOfWork.CartItem.FirstOrDefaultAsync(ci => ci.UserId == user.Id && ci.PackageId == packageId);
                if (cartItem == null)
                {
                    return NotFound("Product not found in the user's cart");
                }

                _unitOfWork.CartItem.Delete(cartItem);
                await _unitOfWork.Save();

                return Ok(new { Message = "Product removed from the cart successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while removing the product from the cart." });
            }
        }



        [HttpPost]
        [Route("Checkout")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Checkout()
        {
            try
            {
                var cartItems = await _unitOfWork.CartItem.GetAllAsync();
                if (!ModelState.IsValid || cartItems == null)
                {
                    return BadRequest(new { State = ModelState, CartItems = cartItems });
                }

                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return NotFound("User not found");
                }

                var orderItemDtos = cartItems.Select(cartItem => new OrderItem
                {
                    PackageId = cartItem.PackageId,
                    Quantity = cartItem.Quantity,
                }).ToList();


                var order = new Order
                {
                    Date = DateTime.Now,
                    TotalPrice = orderItemDtos.Sum(oi => oi.Price * oi.Quantity),
                    UserId = user.Id
                };

                await _unitOfWork.Order.AddAsync(order);
                await _unitOfWork.Save();

                foreach (var orderItem in orderItemDtos)
                {
                    orderItem.OrderId = order.Id;
                }

                await _unitOfWork.OrderItem.AddRangeAsync(orderItemDtos);
                await _unitOfWork.Save();

                var orderItemsList = await _unitOfWork.OrderItem.GetAllAsync(i => i.OrderId == order.Id, i => i.Package.Product);
                var userCartItems = await _unitOfWork.CartItem.GetAllAsync(ci => ci.UserId == user.Id);

                foreach (var orderItem in orderItemsList)
                {
                    var package = orderItem.Package;
                    package.Stock -= orderItem.Quantity;
                    _unitOfWork.Package.Update(package);
                }

                _unitOfWork.CartItem.DeleteRange(userCartItems);
                await _unitOfWork.Save();


                // Create a Stripe session
                var domain = "http://127.0.0.1:5500/hello.html";
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = orderItemsList.Select(oi => new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(oi.Package.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = oi.Package.Product.Name,
                                Description = oi.Package.Product.Description,
                            },
                        },
                        Quantity = oi.Quantity,
                    }).ToList(),
                    Mode = "payment",
                    SuccessUrl = domain + "/suck.html",
                    CancelUrl = domain + "/fool.html",
                };

                var service = new SessionService();
                Session session = service.Create(options);
                Response.Headers.Add("Location", session.Url);

                return new StatusCodeResult(303);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred during checkout." });
            }
        }

    }
}
