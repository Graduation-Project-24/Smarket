using Microsoft.AspNetCore.Mvc;
using Smarket.DataAccess.Repository.IRepository;
using Smarket.Models;
using Smarket.Models.ViewModels;

namespace Smarket.Controllers
{
    // Inventory Should have List of Packages, so you Should include it on methods

    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public InventoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("Get")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var Inventories = await _unitOfWork.Inventory.GetAllAsync(null, i => i.Packages);
                return Ok(Inventories);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // This Method must have Parameter (Inventory)
        // Then pass this parameter to AddAsync()
        [HttpPost("Create")]
        public async Task<IActionResult> Create(InventoryDto viewModel)
        {
            if (ModelState.IsValid)
            {
                Inventory inventory = new()
                {
                    Name = viewModel.Name
                };

                await _unitOfWork.Inventory.AddAsync(inventory);
                await _unitOfWork.Save();
                return Ok(viewModel);
            }
            else
                return NotFound();
        }


        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var obj = await _unitOfWork.Inventory.FirstOrDefaultAsync(u => u.Id == id, p => p.Packages);
            if (obj == null)
                return NotFound();
            else
            {
                _unitOfWork.Inventory.Delete(obj);
                await _unitOfWork.Save();
                return Ok();
            }
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var obj = await _unitOfWork.Inventory.FirstOrDefaultAsync(u => u.Id == id, p => p.Packages);
            if (obj == null)
                return NotFound();
            else
                return Ok(obj);
        }

        // Search for package first then update it 
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, InventoryDto obj)
        {
            var inventory = await _unitOfWork.Inventory.FirstOrDefaultAsync(u => u.Id == id, p => p.Packages);
            inventory.Name = obj.Name;

            _unitOfWork.Inventory.Update(inventory);
            await _unitOfWork.Save();
            return Ok(obj);

        }

    }
}
