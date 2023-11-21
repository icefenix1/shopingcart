using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopingCart.Contracts.Workers;
using ShopingCart.Models.API;
using ShopingCart.Models.DB;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShopingCart.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class Products(IProductLogic product) : ControllerBase
    {
        readonly IProductLogic _product = product;

        // GET: api/<Products>
        [HttpGet]
        public ActionResult<List<ProductsDetails>> Get()
        {
            //var userName = User.Identity.Name;
            var products = _product.GetProductsDetails();
            if (products is null)
            {
                return NotFound();
            }

            return Ok(products);
        }

        // GET api/<Products>/5
        [HttpGet("{id}")]
        public ActionResult<ProductsDetails> Get(Guid id)
        {
            var product = _product.GetProductsDetails(id);
            if (product is null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // POST api/<Products>
        [HttpPost]
        public ActionResult<ProductsDetails> Post([FromBody] Product product)
        {
            if (product is null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            var result = _product.AddDBProduct(product);

            if (result == 0)
            {
                return BadRequest();
            }

            return Ok(_product.GetProductsDetails(product.Id));
        }

        [HttpPost("{id}/upload")]
        public async Task<ActionResult<ProductsDetails>> UploadImage(IFormFile file, Guid id)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }
            var result = await _product.UploadImage(file, id);

            if (result is null)
            {
                return BadRequest();
            }

            return Ok(_product.GetProductsDetails(id));
        }

        // PUT api/<Products>/5
        [HttpPut]
        public ActionResult<ProductsDetails> Put([FromBody] Product product)
        {
            if(product is null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            var result = _product.UpdateDBProduct(product);
            
            if (result == 0 )
            {
                return BadRequest();
            }

            return Ok(_product.GetProductsDetails(product.Id));
        }

        // DELETE api/<Products>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid product ID");
            }

            var result = _product.DeleteDBProduct(id);

            if (result == 0)
            {
                return BadRequest($"Unable to delete product with ID {id}");
            }

            return Ok($"Product {id} Deleted");
        }
    }
}
