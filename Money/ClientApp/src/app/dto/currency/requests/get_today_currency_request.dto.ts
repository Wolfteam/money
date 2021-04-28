import { CurrencyType } from "src/app/enums/currency.enum";

export interface GetTodayCurrencyRequest {
    type: CurrencyType;
}