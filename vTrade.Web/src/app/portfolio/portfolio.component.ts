import { Component, OnInit, ViewChild } from '@angular/core';
import { StockService } from '../stock.service';

import { Chart } from 'chart.js';

import {NgbModal, ModalDismissReasons} from '@ng-bootstrap/ng-bootstrap';

import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-portfolio',
  templateUrl: './portfolio.component.html'
})
export class PortfolioComponent implements OnInit {

  chartMessage;
  chart = [];
  stocks;
  shareNum = 0;
  money = "0.0";
  share = "";

  constructor(
    private stockService: StockService,
    private readonly authService: AuthService,
    private modalService: NgbModal) {
     }

  ngOnInit() {
    this.stockService.getPortfolio().subscribe((data: any) => { this.stocks = data.rows; this.money = data.money});
  }

  openBuy(content, el) {
    this.modalService.open(content, {ariaLabelledBy: 'modal-basic-title'}).result.then((result) => {
      this.buyShares(el);
    }, () => {});
  }

  openSell(content, el) {
    this.modalService.open(content, {ariaLabelledBy: 'modal-basic-title'}).result.then((result) => {
      this.sellShares(el);
    }, () => {});
  }

  buyShares(el) {
    this.stockService.buyShares(el.ticker, this.shareNum).subscribe(() => this.stockService.getPortfolio().subscribe((data: any) => { this.stocks = data.rows; this.money = data.money}));
  }

  sellShares(el) {
    this.stockService.sellShares(el.ticker, this.shareNum).subscribe(() => this.stockService.getPortfolio().subscribe((data: any) => { this.stocks = data.rows; this.money = data.money}));
  }

  update(stocks, money) {
    this.stocks = stocks;
    this.money = money;
  }

  openChart(content, el) {
    this.chartMessage = null;
    this.modalService.open(content, {ariaLabelledBy: 'modal-basic-title', size: 'lg', centered: true, windowClass: 'chartModal'}).result.then((result) => {}, () => {});

    this.stockService.getSharePrices(el.ticker).subscribe((data:any) => 
    {
      if(data == null)
      {
        this.chartMessage = "No chart data available.";
      }else{
        let prices = data['pricePoints'].map(d => d.closePrice);
        let volume = data['pricePoints'].map(d => d.volume);
        let dates = data['pricePoints'].map(d => d.date);

        this.chart = new Chart('canvas', {
            type: 'line',
            data: {
                labels: dates,
                datasets: [
                { 
                    data: prices,
                    borderColor: "#3cba9f",
                    fill: false,
                    yAxisID: 'Prices',
                    label: 'Closing Price'
                },
                { 
                    data: volume,
                    borderColor: "#ffcc00",
                    fill: false,
                    yAxisID: 'Volume',
                    label: 'Volume Traded'
                },
                ]
            },
            options: {
                legend: {
                  display: true
                },
                scales: {
                xAxes: [{
                    display: true
                }],
                yAxes: [{
                    display: true,
                    id: 'Prices',
                    position: 'left',
                    scaleLabel: {
                      labelString: "Australian Dollar ($)",
                      display: true
                    }
                }, {
                    display: true,
                    id: 'Volume',
                    position: 'right',
                    scaleLabel: {
                      labelString: "Share Volume",
                      display: true
                    }
                }],
                }
            }
        });
      }
    });
  }
}
