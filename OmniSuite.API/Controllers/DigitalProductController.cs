using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmniSuite.Application.DigitalProduct.Commands;
using OmniSuite.Application.DigitalProduct.Queries;

namespace OmniSuite.API.Controllers
{
    [ApiController]
    [Route("digital-products")]
    [Authorize]
    public class DigitalProductController : BaseController
    {
        [HttpPost]
        public Task<IActionResult> Create([FromBody] CreateDigitalProductCommand command)
        {
            return SendCommand(command);
        }

        [HttpPut("{id}")]
        public Task<IActionResult> Update(Guid id, [FromBody] UpdateDigitalProductCommand command)
        {
            command.Id = id;
            return SendCommand(command);
        }

        [HttpDelete("{id}")]
        public Task<IActionResult> Delete(Guid id)
        {
            return SendCommand(new DeleteDigitalProductCommand { Id = id });
        }

        [HttpGet("{id}")]
        public Task<IActionResult> GetById(Guid id)
        {
            return SendQuery(new GetDigitalProductByIdQuery { Id = id });
        }

        [HttpGet]
        public Task<IActionResult> GetAll([FromQuery] GetDigitalProductsQuery query)
        {
            return SendQuery(query);
        }

        [HttpGet("category/{categoryId}")]
        public Task<IActionResult> GetByCategory(Guid categoryId, [FromQuery] GetDigitalProductsByCategoryQuery query)
        {
            query.CategoryId = categoryId;
            return SendQuery(query);
        }

        [HttpGet("my-purchases")]
        public Task<IActionResult> GetMyPurchases([FromQuery] GetUserPurchasedProductsQuery query)
        {
            return SendQuery(query);
        }

        [HttpPost("purchase")]
        public Task<IActionResult> Purchase([FromBody] PurchaseDigitalProductCommand command)
        {
            return SendCommand(command);
        }
    }
}
