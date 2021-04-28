import { CurrencyType } from "../enums/currency.enum";

export interface CurrencyModel {
    name: string,
    currency: CurrencyType,
    price: number,
}
