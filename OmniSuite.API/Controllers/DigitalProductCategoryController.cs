using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmniSuite.Application.DigitalProduct.Commands;
using OmniSuite.Application.DigitalProduct.Queries;

namespace OmniSuite.API.Controllers
{
    [ApiController]
    [Route("digital-product-categories")]
    [Authorize]
    public class DigitalProductCategoryController : BaseController
    {
        [HttpPost]
        public Task<IActionResult> Create([FromBody] CreateDigitalProductCategoryCommand command)
        {
            return SendCommand(command);
        }

        [HttpPut("{id}")]
        public Task<IActionResult> Update(Guid id, [FromBody] UpdateDigitalProductCategoryCommand command)
        {
            command.Id = id;
            return SendCommand(command);
        }

        [HttpDelete("{id}")]
        public Task<IActionResult> Delete(Guid id)
        {
            return SendCommand(new DeleteDigitalProductCategoryCommand { Id = id });
        }

        [HttpGet("{id}")]
        public Task<IActionResult> GetById(Guid id)
        {
            return SendQuery(new GetDigitalProductCategoryByIdQuery { Id = id });
        }

        [HttpGet]
        public Task<IActionResult> GetAll([FromQuery] GetDigitalProductCategoriesQuery query)
        {
            return SendQuery(query);
        }
    }
}
