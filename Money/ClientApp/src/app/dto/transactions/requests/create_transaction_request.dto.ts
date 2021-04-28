import { CurrencyType } from "src/app/enums/currency.enum";

export interface CreateTransactionRequest {
    userId: number;
    currencyType: CurrencyType;
    amountToPay: number;
}