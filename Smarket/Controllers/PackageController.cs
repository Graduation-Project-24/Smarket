﻿using Microsoft.AspNetCore.Mvc;
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
                // Include Inventory and Product
                var Packages = await _unitOfWork.Package.GetAllAsync(includeProperties: p => new string[] {"Product", "Inventory"});
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
            var obj = await _unitOfWork.Package.FirstOrDefaultAsync(u => u.Id == id, includeProperties: p => new string[] { "Product", "Inventory" });
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
            var obj = await _unitOfWork.Package.FirstOrDefaultAsync(u => u.Id == id, includeProperties: p => new string[] { "Product", "Inventory" });
            if (obj == null)
                return NotFound();
            else
                return Ok(obj);
        }
        // Search for package first then update it 
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(Package obj)
        {
            var package = await _unitOfWork.Package.FirstOrDefaultAsync(u => u.Id == obj.Id, includeProperties: p => new string[] { "Product", "Inventory" });

            if (ModelState.IsValid)
            {
                _unitOfWork.Package.Update(obj);
                await _unitOfWork.Save();
                return Ok(obj);
            }
            else
                return NotFound();
            
        }

    }
}

