using Microsoft.AspNetCore.Mvc;
using Smarket.DataAccess;
using Smarket.DataAccess.Repository.IRepository;
using Smarket.Models;
using Smarket.Models.DTOs;
using Smarket.Models.ViewModels;
using Smarket.Services.IServices;

namespace Smarket.Controllers
{
    public class InventoryController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<InventoryController> _logger;


        public InventoryController(IUnitOfWork unitOfWork,
        ILogger<InventoryController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;

        }

        // Smarket\Controllers\InventoryController.cs

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryDto>>> GetInventories()
        {
            try
            {
                var inventories = await _unitOfWork.Inventory.GetAllAsync(null, i => i.Packages);

                var inventoryDtos = inventories.Select(i => new InventoryDto
                {
                    Name = i.Name,
                    Packages = i.Packages.Select(p => new PackageDto
                    {
                        ProductId = p.ProductId,
                        ListPrice = p.ListPrice,
                        Price = p.ListPrice,
                        Stock = p.Stock,
                        Date = p.Date,
                    }).ToList()
                });

                return Ok(inventoryDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventories");
                return StatusCode(500, "Internal server error");
            }
        }
        // Smarket\Controllers\InventoryController.cs

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> GetInventoryDetails(int id)
        {
            try
            {
                var inventory = await _unitOfWork.Inventory.FirstOrDefaultAsync(i => i.Id == id, i => i.Packages);

                if (inventory == null)
                {
                    return NotFound();
                }

                return Ok(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting inventory {id} details");
                return StatusCode(500, "Internal server error");
            }
        }

        // Smarket\Controllers\InventoryController.cs

        [HttpPost("Create")]
        public async Task<IActionResult> CreateInventory([FromForm] string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var inventory = new Inventory
                {
                    Name = name
                };

                await _unitOfWork.Inventory.AddAsync(inventory);
                await _unitOfWork.Save();

                return Ok(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating inventory");
                return StatusCode(500, "Internal server error");
            }
        }

        // Smarket\Controllers\InventoryController.cs

        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> UpdateInventory(int id, string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var inventory = await _unitOfWork.Inventory.FirstOrDefaultAsync(i => i.Id == id, i => i.Packages);

                if (inventory == null)
                {
                    return NotFound();
                }

                inventory.Name = name;

                _unitOfWork.Inventory.Update(inventory);
                await _unitOfWork.Save();

                return Ok(new
                {
                    Name = name,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating inventory {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        // Smarket\Controllers\InventoryController.cs

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            try
            {
                var inventory = await _unitOfWork.Inventory.FirstOrDefaultAsync(i => i.Id == id, i => i.Packages);

                if (inventory == null)
                {
                    return NotFound();
                }
                // Delete associated packages
                foreach (var package in inventory.Packages)
                {
                    _unitOfWork.Package.Delete(package);
                }

                _unitOfWork.Inventory.Delete(inventory);
                await _unitOfWork.Save();

                return Ok("The Inventoy have been Deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting inventory {id}");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
