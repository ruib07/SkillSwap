import { IPayment, PaymentsStatus } from "../types/payment";
import apiRequest from "./helpers/apiService";

export const GetPaymentsByPayer = async (payerId: string) => apiRequest("GET", `payments/bypayer/${payerId}`, undefined, true);

export const GetPaymentsByMentor = async (mentorId: string) => apiRequest("GET", `payments/bymentor/${mentorId}`, undefined, true);

export const SendPayment = async (newPayment: IPayment) => apiRequest("POST", "payments", newPayment, true);

export const UpdatePaymentStatus = async (paymentId: string, status: PaymentsStatus) => apiRequest("PATCH", `payments/${paymentId}/status`, status, true);