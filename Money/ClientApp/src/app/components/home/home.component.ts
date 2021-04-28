import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AppService } from 'src/app/services/app.service';
import { purchaseRoute, todayPriceRoute } from "../../routes";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {

  constructor(private router: Router) {
  }

  onTodayPriceClick(): void {
    this.router.navigate([todayPriceRoute]);
  }

  onPurchaseClick(): void {
    this.router.navigate([purchaseRoute]);
  }
}
