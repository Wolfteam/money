import { HttpClient, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { AppResponse } from "../dto/app_response.dto";
import { GetTodayCurrencyRequest } from "../dto/currency/requests/get_today_currency_request.dto";
import { CreateTransactionRequest } from "../dto/transactions/requests/create_transaction_request.dto";
import { TransactionResponse } from "../dto/transactions/responses/transaction_response.dto";
import { CurrencyType } from "../enums/currency.enum";
import { handleErrorResponse } from "../utils/network";

@Injectable({
    providedIn: 'root'
})
export class CurrencyService {
    public get baseUrl(): string {
        return `${environment.apiUrl}/currency`;
    }

    constructor(private httpClient: HttpClient) { }

    async getTodayPrice(forCurrency: CurrencyType): Promise<AppResponse<number>> {
        var requestDto: GetTodayCurrencyRequest = {
            type: forCurrency
        };

        let params = new HttpParams()
        for (let key of Object.keys(requestDto)) {
            params = params.set(key, requestDto[key]);
        }
        try {
            return await this.httpClient.get<AppResponse<number>>(`${this.baseUrl}`, {
                params: params
            }).toPromise();
        } catch (error) {
            return handleErrorResponse<AppResponse<number>>(error);
        }
    }

    async createTransaction(amount: number, currencyType: CurrencyType): Promise<AppResponse<TransactionResponse>> {
        var requestDto: CreateTransactionRequest = {
            amountToPay: amount,
            currencyType: currencyType,
            userId: 1
        };

        try {
            return await this.httpClient
                .post<AppResponse<TransactionResponse>>(`${this.baseUrl}/purchase`, requestDto)
                .toPromise();
        } catch (error) {
            return handleErrorResponse<AppResponse<TransactionResponse>>(error);
        }
    }
}