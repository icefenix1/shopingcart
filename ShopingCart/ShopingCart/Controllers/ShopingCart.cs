using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ShopingCart.Contracts.Repositories;
using ShopingCart.Contracts.Workers;
using ShopingCart.Models.API;
using ShopingCart.Models.DB;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShopingCart.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ShopingCart(ICartLogic cartLogic) : ControllerBase
    {
        private readonly ICartLogic _cartLogic = cartLogic;

        private string GetUserEmail()
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();

            var token = authorizationHeader.Substring("Bearer ".Length);

            var TokenHandler = new JwtSecurityTokenHandler();

            var jwt = TokenHandler.ReadJwtToken(token);

            return jwt.Payload.Claims.FirstOrDefault(i => i.Type == "email").Value;


        }

        [HttpGet("{id}")]
        public ActionResult<CartDetails> GetCart(Guid id)
        {
            var cartDetails = _cartLogic.GetCartDetails(id);
            if (cartDetails == null || cartDetails.Customer != GetUserEmail())
            {
                return NotFound();
            }
            return Ok(cartDetails);
        }

        [HttpGet]
        public ActionResult<List<CartDetails>> GetCarts()
        {
            var customerEmail = GetUserEmail();

            var cartDetails = _cartLogic.GetCartDetails().Where(cart => cart.Customer == customerEmail).ToList();
            if (cartDetails == null)
            {
                return NotFound();
            }
            return Ok(cartDetails);
        }

        [HttpPost]
        public ActionResult<CartDetails> AddCart([FromBody] Cart cart)
        {
            if(cart.Customer != GetUserEmail())
            {
                return BadRequest("Invalid Customer");
            }

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

            if (cart.Customer != GetUserEmail())
            {
                return BadRequest("Invalid Customer");
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
            var cart = _cartLogic.GetCartDetails(id);

            if (cart.Customer != GetUserEmail())
            {
                return BadRequest("Invalid Customer");
            }

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
