import { EmptyResponse } from "../dto/empty_response.dto";

export const handleErrorResponse = <T extends EmptyResponse>(error: any): T => {
    const errorResponse = (error.error === null || error.error === undefined) ?
        undefined :
        error.error as T;

    if (errorResponse)
        return errorResponse;
    console.log(error);
    const genericResponse: EmptyResponse = {
        errorMessageId: 'NA',
        errorMessage: 'Unknown error',
        succeed: false
    };

    return genericResponse as T;
}

export const getPrettyErrorMsg = (errorMsgId: string): string => {
    switch (errorMsgId) {
        case "APP_1":
            return "Invalid request";
        case "APP_2":
            return "Today's price could not be calculated";
        case "APP_3":
            return "The resource you were looking for was not found";
        case "APP_4":
            return "User cannot make purchases";
        default:
            return "Unknown error occurred";
    }
}