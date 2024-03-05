using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Smarket.DataAccess.Repository.IRepository;
using Smarket.Models;
using Smarket.Models.DTOs;
using Smarket.Models.ViewModels;
using Stripe;

namespace Smarket.Controllers
{
    public class PackageController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PackageController> _logger;

        public PackageController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PackageController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PackageDtoWithInfo>>> GetPackages()
        {
            try
            {
                var packages = await _unitOfWork.Package.GetAllAsync(null,
                p => p.Product.Image, p => p.Inventory);

                var packageDtos = packages.Select(p => new PackageDtoWithInfo
                {
                    ProductName=p.Product.Name,
                    ProductDescription=p.Product.Description,
                    ProductImageUrl=p.Product.Image.Url,
                    InventoryName=p.Inventory.Name,
                    Price=p.Price,
                    ListPrice=p.ListPrice,
                    left=p.left,
                    Stock=p.Stock,
                    Date=p.Date,
                    ExpireDate=p.ExpireDate,
                });;

                return Ok(packageDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting packages");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> GetPackageDetails(int id)
        {
            try
            {
                var package = await _unitOfWork.Package.FirstOrDefaultAsync(i => i.Id == id,
                    p => p.Product, p => p.Inventory);

                if (package == null)
                {
                    return NotFound();
                }

                return Ok(package);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting package details for id {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("CreatePackage")]
        public async Task<IActionResult> CreatePackage([FromForm] PackageDto packageDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var package = _mapper.Map<Package>(packageDto);

                await _unitOfWork.Package.AddAsync(package);
                await _unitOfWork.Save();

                return Ok(new 
                {
                    id = package.Id,
                    ListPrice = package.ListPrice,
                    Price = package.Price,
                    Date = package.Date,
                    ExpireDate = package.ExpireDate,
                    Stock= package.Stock,
                    Left=package.left
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating package");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> UpdatePackage(int id, PackageDto packageDto)
        {
            try
            {
                var package = await _unitOfWork.Package.FirstOrDefaultAsync(i => i.Id == id);

                if (package == null)
                {
                    return NotFound();
                }

                _mapper.Map(packageDto, package);

                _unitOfWork.Package.Update(package);
                await _unitOfWork.Save();

                return Ok(package);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating package for id {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeletePackage(int id)
        {
            try
            {
                var package = await _unitOfWork.Package.FirstOrDefaultAsync(p => p.Id == id);

                if (package == null)
                {
                    return NotFound();
                }

                _unitOfWork.Package.Delete(package);
                await _unitOfWork.Save();

                return Ok("The package has been deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting package for id {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("CreatePackages")]
        public async Task<IActionResult> CreatePackages(List<PackageDto> packageDtos)
        {
            try
            {
                if (!ModelState.IsValid || packageDtos == null || !packageDtos.Any())
                {
                    return BadRequest("Invalid input data");
                }

                var packages = _mapper.Map<List<Package>>(packageDtos);

                foreach (var package in packages)
                {
                    await _unitOfWork.Package.AddAsync(package);
                }

                await _unitOfWork.Save();

                var response = packages.Select(package => new
                {
                    id = package.Id,
                    ListPrice = package.ListPrice,
                    Price = package.Price,
                    Date = package.Date,
                    ExpireDate = package.ExpireDate,
                    Stock = package.Stock,
                    Left = package.left
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating packages");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("DeleteAll")]
        public async Task<IActionResult> DeleteAllPackages()
        {
            try
            {
                var packages = await _unitOfWork.Package.GetAllAsync();

                _unitOfWork.Package.DeleteRange(packages);
                await _unitOfWork.Save();

                return Ok("All packages have been deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting packages");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
