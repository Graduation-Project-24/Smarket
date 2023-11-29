using Microsoft.AspNetCore.Mvc;
using Smarket.DataAccess.Repository.IRepository;
using Smarket.Models;
using Smarket.Models.ViewModels;


namespace Smarket.Controllers
{
    public class PackageController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly IEmailService _emailService;

        public PackageController(IUnitOfWork unitOfWork /*, IEmailService emailService*/)
        {
            _unitOfWork = unitOfWork;
            //_emailService = emailService;
        }

        [HttpGet("Get")]
        public async Task<IActionResult> Get()
        {
            try
            {
                // Include Inventory and Product
                //_emailService.EmailSender("maimallam57@gmail.com", "Thank you for your order! - Smarket", $"<div> <h2><strong>Dear, Mai</strong></h2> <h3>Test Email Service </h3> </div>");
                var Packages = await _unitOfWork.Package.GetAllAsync();
                return Ok(Packages);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        // No need for All Inventoies and Products in Creating, and you should do include in get method
        [HttpPost("Create")]
        public async Task<IActionResult> Create(PackageDto viewModel)
        {
            if (ModelState.IsValid)
            {
                Package package = new()
                {
                    Date = viewModel.Date,
                    ExpireDate = viewModel.ExpireDate,
                    Stock = viewModel.Stock,
                    Price = viewModel.Price,
                    ListPrice = viewModel.ListPrice,
                    ProductId = viewModel.ProductId,
                    InventoryId = viewModel.InventoryId
                };

                await _unitOfWork.Package.AddAsync(package);
                await _unitOfWork.Save();
                return Ok(viewModel);
            }
            else
                return NotFound();
        }


        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var obj = await _unitOfWork.Package.FirstOrDefaultAsync(u => u.Id == id, p => p.Product, p => p.Inventory);
            if (obj == null)
                return NotFound();
            else
            {
                _unitOfWork.Package.Delete(obj);
                await _unitOfWork.Save();
                return Ok();
            }
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var obj = await _unitOfWork.Package.FirstOrDefaultAsync(u => u.Id == id);
            if (obj == null)
                return NotFound();
            else
                return Ok(obj);
        }
        // Search for package first then update it 
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, PackageDto obj)
        {
            var package = await _unitOfWork.Package.FirstOrDefaultAsync(u => u.Id == id);
            package.Date = obj.Date;
            package.ExpireDate = obj.ExpireDate;
            package.Stock = obj.Stock;
            package.Price = obj.Price;
            package.ListPrice = obj.ListPrice;
            package.ProductId = obj.ProductId;
            package.InventoryId = obj.InventoryId;

            _unitOfWork.Package.Update(package);
            await _unitOfWork.Save();
            return Ok(obj);

        }

    }
}

