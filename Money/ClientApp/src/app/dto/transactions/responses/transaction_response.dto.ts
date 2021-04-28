import { CurrencyType } from "src/app/enums/currency.enum";

export interface TransactionResponse {
    id: number;
    createdAt: Date;
    currencyType: CurrencyType;
    paidAmount: number;
    purchasedAmount: number;
    userId: number;
}