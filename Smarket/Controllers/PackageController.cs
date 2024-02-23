using Microsoft.AspNetCore.Mvc;
using Smarket.DataAccess.Repository.IRepository;
using Smarket.Models;
using Smarket.Models.ViewModels;


namespace Smarket.Controllers
{
    public class PackageController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public PackageController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("Get")]
        public async Task<IActionResult> Get()
        {
            try
            { 

                var Packages = await _unitOfWork.Package.GetAllAsync(null, p => p.Product, p => p.Inventory);
                return Ok(Packages);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
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
                    InventoryId = viewModel.InventoryId,
                    
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
            var obj = await _unitOfWork.Package.FirstOrDefaultAsync(u => u.Id == id, p => p.Product, p => p.Inventory);
            if (obj == null)
                return NotFound();
            else
                return Ok(obj);
        }
        // Search for package first then update it 
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, PackageDto obj)
        {
            var package = await _unitOfWork.Package.FirstOrDefaultAsync(u => u.Id == id, p => p.Product, p => p.Inventory);
            package.Date = obj.Date;
            package.ExpireDate = obj.ExpireDate;
            package.Stock = obj.Stock;
            package.Price = obj.Price;
            package.ListPrice = obj.ListPrice;
            package.ProductId = obj.ProductId;
            package.Inventory.Id = obj.InventoryId;

            _unitOfWork.Package.Update(package);
            await _unitOfWork.Save();
            return Ok(obj);

        }

    }
}

