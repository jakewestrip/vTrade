using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vTrade.Queries;

namespace vTrade.Controllers
{
    [Route("api/portfolio")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioQuery _portfolioQuery;

        public PortfolioController(IPortfolioQuery portfolioQuery)
        {
            _portfolioQuery = portfolioQuery;
        }

        // GET api/portfolio
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var UserId = int.Parse(User.Claims.First(x => x.Type == "sub").Value);
            var result = await _portfolioQuery.Execute(UserId);
            return Ok(result);
        }
    }
}
