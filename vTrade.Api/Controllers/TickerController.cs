using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vTrade.Queries;

namespace vTrade.Controllers
{
    [Route("api/ticker")]
    [ApiController]
    public class TickerController : ControllerBase
    {
        private readonly IAddTickerCommand _addTickerCommand;

        public TickerController(IAddTickerCommand addTickerCommand)
        {
            _addTickerCommand = addTickerCommand;
        }

        // POST api/ticker/add
        [Authorize]
        [Route("add")]
        [HttpPost]
        public async Task<IActionResult> Buy(TickerAddModel model)
        {
            if(!User.HasClaim("role", "1"))
            {
                return Forbid();
            }

            var result = await _addTickerCommand.Execute(model.Ticker);

            return Ok(result);
        }
    }

    public class TickerAddModel
    {
        public string Ticker { get; set; }
    }
}
