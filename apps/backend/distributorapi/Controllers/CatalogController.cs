using Microsoft.AspNetCore.Mvc;
using DistributorApi.Services;

namespace DistributorApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogService _catalogService;

        public CatalogController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetCatalog()
        {
            var catalog = await _catalogService.GetCatalogAsync();
            return Ok(catalog);
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult<object>> GetProduct(int productId)
        {
            var product = await _catalogService.GetProductByIdAsync(productId);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<object>>> SearchProducts([FromQuery] string q)
        {
            if (string.IsNullOrEmpty(q))
                return BadRequest("Query parameter 'q' is required");

            var products = await _catalogService.SearchProductsAsync(q);
            return Ok(products);
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<object>>> GetProductsByCategory(string category)
        {
            var products = await _catalogService.GetProductsByCategoryAsync(category);
            return Ok(products);
        }
    }
}
