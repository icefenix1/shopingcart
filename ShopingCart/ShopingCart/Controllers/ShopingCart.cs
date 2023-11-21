using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopingCart.Contracts.Repositories;
using ShopingCart.Contracts.Workers;
using ShopingCart.Models.API;
using ShopingCart.Models.DB;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShopingCart.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ShopingCart(ICartLogic cartLogic) : ControllerBase
    {
        private readonly ICartLogic _cartLogic = cartLogic;

        

        [HttpGet("{id}")]
        public ActionResult<CartDetails> GetCart(Guid id)
        {
            var cartDetails = _cartLogic.GetCartDetails(id);
            if (cartDetails == null)
            {
                return NotFound();
            }
            return Ok(cartDetails);
        }

        [HttpGet]
        public ActionResult<List<CartDetails>> GetCarts()
        {
            var cartDetails = _cartLogic.GetCartDetails();
            if (cartDetails == null)
            {
                return NotFound();
            }
            return Ok(cartDetails);
        }

        [HttpPost]
        public ActionResult<CartDetails> AddCart([FromBody] Cart cart)
        {
            var result = _cartLogic.AddDBCart(cart);
            if (result == 0)
            {
                return BadRequest();
            }
            return Ok(_cartLogic.GetCartDetails(cart.Id));
        }

        [HttpPut("{id}")]
        public ActionResult<CartDetails> UpdateCart(Guid id, [FromBody] Cart cart)
        {
            if (id != cart.Id)
            {
                return BadRequest("Invalid ID");
            }

            var result = _cartLogic.UpdateDBCart(cart);
            if (result == 0)
            {
                return BadRequest();
            }
            return Ok(_cartLogic.GetCartDetails(cart.Id));
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteCart(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid cart ID");
            }
            var result = _cartLogic.DeleteDBCart(id);
            if (result == 0)
            {
                return BadRequest($"Unable to delete cart with ID {id}");
            }

            return Ok($"Cart {id} Deleted");
        }

        
    }
}
