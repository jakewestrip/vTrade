using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vTrade.Models;
using vTrade.Queries;

namespace vTrade.Controllers
{
    [Route("api/share")]
    [ApiController]
    public class ShareController : ControllerBase
    {
        private readonly IBuyShareCommand _buyShareCommand;
        private readonly ISellShareCommand _sellShareCommand;
        private readonly IDayPricesQuery _dayPricesQuery;

        public ShareController(IBuyShareCommand buyShareCommand, ISellShareCommand sellShareCommand, IDayPricesQuery dayPricesQuery)
        {
            _buyShareCommand = buyShareCommand;
            _sellShareCommand = sellShareCommand;
            _dayPricesQuery = dayPricesQuery;
        }

        // POST api/share/buy
        [Authorize]
        [Route("buy")]
        [HttpPost]
        public async Task<IActionResult> Buy(ShareRequest request)
        {
            var UserId = int.Parse(User.Claims.First(x => x.Type == "sub").Value);

            var result = await _buyShareCommand.Execute(request.Ticker, request.NumShares, UserId);

            return Ok(result);
        }

        // POST api/share/sell
        [Authorize]
        [Route("sell")]
        [HttpPost]
        public async Task<IActionResult> Sell(ShareRequest request)
        {
            var UserId = int.Parse(User.Claims.First(x => x.Type == "sub").Value);

            var result = await _sellShareCommand.Execute(request.Ticker, request.NumShares, UserId);

            return Ok(result);
        }

        //GET api/share/{ticker}/prices
        [Authorize]
        [Route("{ticker}/prices")]
        [HttpGet]
        public async Task<DayPricesDto> GetPrices(string ticker)
        {
            var result = await _dayPricesQuery.Execute(ticker);

            return result;
        }
    }

    public class ShareRequest
    {
        public string Ticker { get; set; }
        public int NumShares { get; set; }
    }
}
