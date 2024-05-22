using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Smarket.DataAccess.Repository.IRepository;
using Smarket.Models;
using Smarket.Models.DTOs;
using Smarket.Services.IServices;
using Stripe.Checkout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Smarket.Models.Enum;
using Smarket.Services;

namespace Smarket.Controllers
{

    public class OrderController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IStripeService _stripeService;

        public OrderController(IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper, ITokenService tokenService, IEmailService emailService, IStripeService stripeService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
            _emailService = emailService;
            _stripeService = stripeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var obj = await _unitOfWork.Order.FirstOrDefaultAsync(x => x.Id == id,p=>p.Items);
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
        [HttpGet("GetShoppingCartbyUser")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetShoppingCartbyUser()
        {
            var realuser = await _userManager.GetUserAsync(User);
            var items = await _unitOfWork.CartItem.GetAllAsync(x => x.UserId == realuser.Id, p => p.Package.Product.Image);
            try
            {
                var shoppingCartDto = new ShoppingCartDto
                {
                    Username = realuser.UserName,
                    Packages = items.Select(item => new PackageDtoForCart
                    {
                        ProductId= item.Package.ProductId,
                        ListPrice = item.Package.ListPrice,
                        Price = item.Package.Price,
                        ProductImageUrl = item.Package.Product.Image.Url.ToString(),
                        ProductName = item.Package.Product.Name,
                        Count = item.Quantity

                    }).ToList()
                };
                return Ok(shoppingCartDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


            
        }

        [HttpPost]
        [Route("AddToCart")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
            var product = await _unitOfWork.Product.FirstOrDefaultAsync(c=>c.Id==addToCartDto.ProductId);

            var package = await _unitOfWork.Package.FirstOrDefaultAsync(c => c.ProductId ==product.Id);
            if (package == null)
            {
                return NotFound("Product not found");
            }
            var cartItemFound = await _unitOfWork.CartItem.GetAllAsync(c=>c.UserId== user.Id);
            if (cartItemFound != null)
            {
                foreach (var item in cartItemFound)
                {
                    if (item.PackageId == package.Id)
                    {
                        item.Quantity++;
                        _unitOfWork.CartItem.Update(item);
                        await _unitOfWork.Save();
                        return Ok(new { Message = "Product Have increment in Cart" });
                    }
                }
            }

            var cartItem = new CartItem
            {
                Quantity = addToCartDto.Quantity,
                PackageId = package.Id,
                UserId = user.Id,
            };

            await _unitOfWork.CartItem.AddAsync(cartItem);
            await _unitOfWork.Save();

            return Ok(new { Message = "Product added to cart successfully" });
        }

        [HttpPost]
        [Route("RemoveFromCart")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RemoveFromCart(ProductwithIdDto dto)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return NotFound("User not found");
                }
                var package = await _unitOfWork.Package.FirstOrDefaultAsync(x => x.ProductId == dto.productId);

                var cartItem = await _unitOfWork.CartItem.FirstOrDefaultAsync(ci => ci.UserId == user.Id && ci.PackageId == package.Id);
                if (cartItem == null)
                {
                    return NotFound("Product not found in the user's cart");
                }

                _unitOfWork.CartItem.Delete(cartItem);
                await _unitOfWork.Save();

                return Ok(new { Message = "Product removed from the cart successfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "An error occurred while removing the product from the cart." });
            }
        }


        [HttpPost]
        [Route("RemoveFromCartMobile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RemoveFromCartMobile(int productId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return NotFound("User not found");
                }
                var package = await _unitOfWork.Package.FirstOrDefaultAsync(x => x.ProductId == productId);

                var cartItem = await _unitOfWork.CartItem.FirstOrDefaultAsync(ci => ci.UserId == user.Id && ci.PackageId == package.Id);
                if (cartItem == null)
                {
                    return NotFound("Product not found in the user's cart");
                }

                _unitOfWork.CartItem.Delete(cartItem);
                await _unitOfWork.Save();

                return Ok(new { Message = "Product removed from the cart successfully" });
            }
            catch (Exception)
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

                var orderItemsList = await _unitOfWork.OrderItem.GetAllAsync(i => i.OrderId == order.Id, i => i.Package.Product.Image);
                var userCartItems = await _unitOfWork.CartItem.GetAllAsync(ci => ci.UserId == user.Id);


                _unitOfWork.CartItem.DeleteRange(userCartItems);
                await _unitOfWork.Save();


                var sessionUrl = await _stripeService.CreateCheckoutSession(orderItemsList);

                return Ok(new { SessionUrl = sessionUrl });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "An error occurred during checkout." });
            }
        }

        [HttpPost]
        [Route("ConfirmOrder/{orderId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ConfirmOrder(int orderId)
        {
            try
            {
                var order = await _unitOfWork.Order.FirstOrDefaultAsync(x => x.Id == orderId);

                if (order == null)
                {
                    return NotFound("Order not found");
                }
                order.Status = Status.Success;

                var orderItemsList = await _unitOfWork.OrderItem.GetAllAsync(i => i.OrderId == order.Id, i => i.Package.Product);

                foreach (var orderItem in orderItemsList)
                {
                    var package = orderItem.Package;
                    package.left -= orderItem.Quantity;
                    _unitOfWork.Package.Update(package);
                }
                await _unitOfWork.Save();

                await _emailService.EmailSender(order.User.Email, "Order Confirmation", "Thank you for your order!");
                return Ok(new { Message = "Order confirmed successfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "An error occurred while confirming the order." });
            }
        }

        [HttpPost]
        [Route("DenyOrder/{orderId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DenyOrder(int orderId)
        {
            try
            {
                var order = await _unitOfWork.Order.FirstOrDefaultAsync(x => x.Id == orderId);

                if (order == null)
                {
                    return NotFound("Order not found");
                }
                order.Status = Status.Deny;

                await _emailService.EmailSender(order.User.Email, "Order Denial", "Unfortunately, your order has been denied.");

                return Ok(new { Message = "Order denied successfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "An error occurred while denying the order." });
            }
        }


    }
}
