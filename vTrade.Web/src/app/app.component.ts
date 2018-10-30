import { Component, ViewChild } from '@angular/core';
import { AuthService } from './auth/auth.service';
import { StockService } from './stock.service';
import { PortfolioComponent } from './portfolio/portfolio.component';

import {NgbModal, ModalDismissReasons} from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent {
  @ViewChild('folio') folio: PortfolioComponent;
  title = 'vTradeWeb';
  _isAdmin: boolean = false;

  newTickerString: string = "";

  constructor(
    private stockService: StockService,
    private readonly authService: AuthService,
    private modalService: NgbModal)
    {
      this._isAdmin = authService.isAdmin();
    }

  logout() {
    this.authService.logout();
  }

  openNewTicker(content) {
    this.modalService.open(content, {ariaLabelledBy: 'modal-basic-title'}).result.then((result) => {
      this.newTicker();
    }, () => {});
  }

  newTicker() {
    this.stockService.newTicker(this.newTickerString).subscribe(() => this.stockService.getPortfolio().subscribe((data: any) => this.folio.update(data.rows, data.money)));
  }
}
