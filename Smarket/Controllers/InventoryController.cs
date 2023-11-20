using Microsoft.AspNetCore.Mvc;
using Smarket.DataAccess.Repository.IRepository;
using Smarket.Models;

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
                var Inventories = await _unitOfWork.Inventory.GetAllAsync(includeProperties: p => new string[] { "Package" });
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
        public async Task<IActionResult> Create(Inventory obj)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Inventory.AddAsync(obj);
            }
            await _unitOfWork.Save();
            return Ok();
        }


        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var obj = await _unitOfWork.Inventory.FirstOrDefaultAsync(u => u.Id == id, includeProperties: p => new string[] { "Package" });
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
            var obj = await _unitOfWork.Category.FirstOrDefaultAsync(u => u.Id == id, includeProperties: p => new string[] { "Package" });
            if (obj == null)
                return NotFound();
            else
                return Ok(obj);
        }

        // Search for package first then update it 
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(Inventory obj)
        {
            var inventory = await _unitOfWork.Inventory.FirstOrDefaultAsync(u => u.Id == obj.Id, includeProperties: p => new string[] { "Package" });
            
            if (ModelState.IsValid)
            {
                _unitOfWork.Inventory.Update(obj);
                await _unitOfWork.Save();
                return Ok(obj);
            }
            else
                return NotFound();

        }

    }
}
