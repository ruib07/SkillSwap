import { useEffect, useState } from "react";
import UserProfileHeader from "../../layouts/ProfileHeader";
import { IPayment, IPaymentWithNames, PaymentsStatus, paymentsStatusMap } from "../../types/payment";
import { GetUserById, UpdateBalance } from "../../services/usersService";
import { GetPaymentsByMentor, GetPaymentsByPayer, UpdatePaymentStatus } from "../../services/paymentsService";

export default function UserPayments() {
    const [isMentor, setIsMentor] = useState<boolean | null>(null); 
    const [payments, setPayments] = useState<IPaymentWithNames[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [, setUserDetails] = useState<{ [key: string]: string }>({});

    const userId = localStorage.getItem("userId") || sessionStorage.getItem("userId");

    useEffect(() => {
        if (!userId) {
            setLoading(false);
            return;
        }

        const fetchUserAndPayments = async () => {
            try {
                const usersResponse = await GetUserById(userId);
                const mentorStatus = usersResponse.data.isMentor;
                setIsMentor(mentorStatus);

                const paymentsResponse = mentorStatus
                    ? await GetPaymentsByMentor(userId)
                    : await GetPaymentsByPayer(userId);

                const paymentsWithNames = await Promise.all(
                    paymentsResponse.data.map(async (payment: IPayment) => {
                        const mentorResponse = await GetUserById(payment.mentorId);
                        const payerResponse = await GetUserById(payment.payerId);
                        setUserDetails(prevState => ({
                            ...prevState,
                            [payment.mentorId]: mentorResponse.data.name,
                            [payment.payerId]: payerResponse.data.name,
                        }));
                        return {
                            ...payment,
                            mentorName: mentorResponse.data.name,
                            payerName: payerResponse.data.name,
                            statusLabel: paymentsStatusMap[payment.status],
                        };
                    })
                );

                setPayments(paymentsWithNames);
            } catch {
                setError("Failed to fetch payments.");
            } finally {
                setLoading(false);
            }
        };

        fetchUserAndPayments();
    }, [userId]);

    const handlePay = async (paymentId: string, amount: number, mentorId: string, payerId: string) => {
        try {
            await UpdatePaymentStatus(paymentId, PaymentsStatus.Completed);

            const payerResponse = await GetUserById(payerId);
            console.log(payerResponse.data.balance);
            const newPayerBalance = payerResponse.data.balance - amount;
            await UpdateBalance(payerId, newPayerBalance);

            const mentorResponse = await GetUserById(mentorId);
            console.log(mentorResponse.data.balance);
            const newMentorBalance = mentorResponse.data.balance + amount;
            await UpdateBalance(mentorId, newMentorBalance);

            setPayments((prevPayments) =>
                prevPayments.map((payment) =>
                    payment.id === paymentId
                        ? { ...payment, statusLabel: "Completed" }
                        : payment
                )
            );
        } catch {
            setError("Failed to process payment.");
        }
    };

    if (loading) {
        return <p className="text-center text-gray-200">Loading your payments...</p>;
    }

    if (error) {
        return <p className="text-center text-red-500">{error}</p>;
    }

    return (
        <>
            <UserProfileHeader />
            <div className="mt-[80px] p-8">
                <h2 className="text-2xl font-bold text-gray-200 mb-6 text-center">
                    My Payments
                </h2>

                {payments.length > 0 ? (
                    <div className="overflow-x-auto">
                        <table className="min-w-full bg-gray-800 border border-gray-700">
                            <thead>
                                <tr className="bg-gray-900 text-gray-300">
                                    <th className="py-3 px-2 text-left">Payments</th>
                                    <th className="py-3 px-2 text-left">Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                {payments.map((payment) => (
                                    <tr key={payment.id} className="border-t border-gray-700">
                                        <td className="p-2 hover:bg-gray-700">
                                            <div className="flex items-center gap-3">
                                                <div>
                                                    <div className="flex">
                                                        <p className="text-gray-400 text-md">Mentor: </p>
                                                        <p className="ms-2 text-gray-300 text-md">{payment.mentorName} </p>
                                                    </div>

                                                    <div className="flex">
                                                        <p className="text-gray-400 text-md">Payer: </p>
                                                        <p className="ms-2 text-gray-300 text-md">{payment.payerName} </p>
                                                    </div>

                                                    <div className="flex">
                                                        <p className="text-gray-400 text-md">Amount: </p>
                                                        <p className="ms-2 text-gray-300 text-md">{payment.amount} </p>
                                                    </div>

                                                    <div className="flex">
                                                        <p className="text-gray-400 text-md">Status: </p>
                                                        <p className="ms-2 text-gray-300 text-md">{payment.statusLabel} </p>
                                                    </div>
                                                </div>
                                            </div>
                                        </td>
                                        <td className="p-2">
                                            {!isMentor && (
                                                <button
                                                    className={`py-2 px-4 rounded ${payment.statusLabel === "Pending"
                                                            ? "bg-green-500 text-white cursor-pointer"
                                                            : "bg-blue-500 text-white cursor-not-allowed"
                                                        }`}
                                                    onClick={() =>
                                                        payment.statusLabel === "Pending" &&
                                                        handlePay(payment.id!, payment.amount, payment.mentorId, payment.payerId)
                                                    }
                                                    disabled={payment.statusLabel !== "Pending"}
                                                >
                                                    {payment.statusLabel === "Pending" ? "Pay" : "Payment Completed"}
                                                </button>
                                            )}
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                ) : (
                    <p className="text-center text-gray-200">You have no payments yet.</p>
                )}
            </div>
        </>
    );
}
