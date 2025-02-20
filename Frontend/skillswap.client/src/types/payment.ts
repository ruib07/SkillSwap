export enum PaymentsStatus {
    Pending = 0,
    Completed = 1,
    Failed = 2
}

export const paymentsStatusMap: { [key: number]: string } = {
    [PaymentsStatus.Pending]: "Pending",
    [PaymentsStatus.Completed]: "Completed",
    [PaymentsStatus.Failed]: "Failed"
};

export interface IPayment {
    id?: string;
    payerId: string;
    mentorId: string;
    amount: number;
    status: number;
}

export interface IPaymentWithNames extends IPayment {
    mentorName: string;
    payerName: string;
    statusLabel: string;
}