using Microsoft.AspNetCore.Mvc;
using Smarket.DataAccess.Repository.IRepository;
using Smarket.Models;
using Smarket.Models.ViewModels;

namespace Smarket.Controllers
{
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
                var Inventories = await _unitOfWork.Inventory.GetAllAsync();
                return Ok(Inventories);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create()
        {
            if (!ModelState.IsValid)
            {
                await _unitOfWork.Inventory.AddAsync(new Inventory());
            }
            await _unitOfWork.Save();
            return Ok();
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            var obj = await _unitOfWork.Inventory.FirstOrDefaultAsync(u => u.Id == id);
            if (obj == null)
            {
                return Ok();
            }
            _unitOfWork.Inventory.Delete(obj);
            await _unitOfWork.Save();
            return Ok();
        }

        [HttpGet("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            var obj = await _unitOfWork.Category.FirstOrDefaultAsync(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            return Ok(obj);
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(Inventory obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Inventory.Update(obj);

                await _unitOfWork.Save();
            }
            return Ok(obj);
        }

    }
}
