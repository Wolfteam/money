import { Injectable } from "@angular/core";
import { ToastrService } from "ngx-toastr";

@Injectable({
    providedIn: 'root'
})
export class ToastService {
    constructor(private toastr: ToastrService) { }

    success(title: string, msg: string): void {
        this.toastr.success(msg, title);
    }

    info(title: string, msg: string): void {
        this.toastr.info(msg, title);
    }

    warning(title: string, msg: string): void {
        this.toastr.warning(msg, title);
    }

    error(title: string, msg: string): void {
        this.toastr.error(msg, title);
    }
}