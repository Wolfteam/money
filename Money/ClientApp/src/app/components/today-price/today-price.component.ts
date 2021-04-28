import { Component, OnDestroy, OnInit } from '@angular/core';
import { CurrencyType } from 'src/app/enums/currency.enum';
import { CurrencyModel } from 'src/app/models/currency.model';
import { AppService } from 'src/app/services/app.service';
import { CurrencyService } from 'src/app/services/currency.service';
import { ToastService } from 'src/app/services/toast.service';
import { getPrettyErrorMsg } from 'src/app/utils/network';

@Component({
  selector: 'app-today-price',
  templateUrl: './today-price.component.html',
  styleUrls: ['./today-price.component.css']
})
export class TodayPriceComponent implements OnInit, OnDestroy {
  currencies: CurrencyModel[] = [];
  isBusy = false;
  private availabeCurrencies = [CurrencyType.dollar, CurrencyType.real];

  constructor(
    private currencyService: CurrencyService,
    private appService: AppService,
    private toastService: ToastService) {
    this.appService.showBackButton(true);
    this.appService.changeMaintTitle('Cotizaciones');
    this.appService.showMainProgressBar(true);
  }

  async ngOnInit(): Promise<void> {
    await this.onRefreshClick();
  }

  ngOnDestroy(): void {
    this.appService.showMainProgressBar(false);
  }

  async onRefreshClick(): Promise<void> {
    this.currencies = [];
    this.isBusy = true;
    this.appService.showBackButton(false);
    for (let index = 0; index < this.availabeCurrencies.length; index++) {
      const currency = this.availabeCurrencies[index];
      const response = await this.currencyService.getTodayPrice(currency);
      if (response.succeed) {
        this.currencies.push({
          name: currency == CurrencyType.dollar ? "Dollar" : "Real",
          currency: currency,
          price: response.result
        });
      } else {
        this.toastService.error('Error', getPrettyErrorMsg(response.errorMessageId));
        break;
      }
    }
    this.isBusy = false;
    this.appService.showMainProgressBar(false);
    this.appService.showBackButton(true);
  }
}
