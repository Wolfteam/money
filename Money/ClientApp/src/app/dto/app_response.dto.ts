import { EmptyResponse } from "./empty_response.dto";

export interface AppResponse<T> extends EmptyResponse {
    result: T;
}