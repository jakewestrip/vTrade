import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { AppConfig } from './shared/app.config';

@Injectable({
  providedIn: 'root'
})
export class StockService {

  constructor(
    private readonly httpClient: HttpClient,
    private readonly AppConfig: AppConfig) { }

  getStocks()
  {
    var t = this.httpClient.get(`${AppConfig.settings.apiAddress}/api/stocks`);
    return t;
  }

  getPortfolio()
  {
    var t = this.httpClient.get(`${AppConfig.settings.apiAddress}/api/portfolio`);
    return t;
  }

  buyShares(ticker: string, num: number)
  {
    var t = this.httpClient.post(`${AppConfig.settings.apiAddress}/api/share/buy`, { "ticker": ticker, "numShares": num });
    return t;
  }

  sellShares(ticker: string, num: number)
  {
    var t = this.httpClient.post(`${AppConfig.settings.apiAddress}/api/share/sell`, { "ticker": ticker, "numShares": num });
    return t;
  }

  newTicker(ticker: string)
  {
    debugger;
    var t = this.httpClient.post(`${AppConfig.settings.apiAddress}/api/ticker/add`, { "ticker": ticker });
    return t;
  }

  getSharePrices(ticker: string)
  {
    var t = this.httpClient.get(`${AppConfig.settings.apiAddress}/api/share/${ticker}/prices`);
    return t;
  }
}
