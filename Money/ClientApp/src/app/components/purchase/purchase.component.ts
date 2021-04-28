import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CurrencyType } from 'src/app/enums/currency.enum';
import { CurrencyModel } from 'src/app/models/currency.model';
import { homeRoute } from 'src/app/routes';
import { AppService } from 'src/app/services/app.service';
import { CurrencyService } from 'src/app/services/currency.service';
import { getPrettyErrorMsg } from 'src/app/utils/network';
import { ToastService } from "src/app/services/toast.service";

@Component({
  selector: 'app-purchase',
  templateUrl: './purchase.component.html',
  styleUrls: ['./purchase.component.css']
})
export class PurchaseComponent implements OnInit {

  isBusy = false;
  currencies: CurrencyModel[] = [
    {
      currency: CurrencyType.dollar,
      name: "Dollar",
      price: -1
    },
    {
      currency: CurrencyType.real,
      name: "Real",
      price: -1
    }
  ];
  formGroup: FormGroup;

  constructor(
    private appService: AppService,
    private currencyService: CurrencyService,
    private toastService: ToastService,
    private router: Router) {
    this.formGroup = new FormGroup({
      currencyType: new FormControl(CurrencyType["dollar"], [Validators.required]),
      amount: new FormControl(0, [Validators.required, Validators.pattern(/^-?(0|[1-9]\d*)?$/), this.amountValidator]),
    });
  }

  ngOnInit() {
    this.appService.changeMaintTitle('Comprar');
    this.appService.showBackButton(true);
  }

  async createTransaction(): Promise<void> {
    const amount = this.formGroup.get("amount").value as number;
    const type = this.formGroup.get("currencyType").value as CurrencyType;
    this.appService.showBackButton(false);
    this.appService.showMainProgressBar(true);
    this.isBusy = true;
    const response = await this.currencyService.createTransaction(amount, type);
    if (response.succeed) {
      this.toastService.success('Exito', 'Compra exitosa');
      this.router.navigate([homeRoute]);
    } else {
      this.toastService.error('Error', getPrettyErrorMsg(response.errorMessageId));
    }
    console.warn(response);
    this.isBusy = false;
    this.appService.showBackButton(true);
    this.appService.showMainProgressBar(false);
  }

  amountValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const amount = control.value;
    if (amount !== undefined && (isNaN(amount) || amount <= 0)) {
      return { 'amount': true };
    }
    return null;
  }
}
