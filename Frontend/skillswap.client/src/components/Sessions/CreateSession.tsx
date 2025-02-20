import { FormEvent, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { showToast } from "../../utils/toastHelper";
import { ISession } from "../../types/session";
import { CreateSession } from "../../services/sessionsService";
import Img from "/assets/SkillSwap-Logo.png";
import { IPayment, PaymentsStatus } from "../../types/payment";
import { SendPayment } from "../../services/paymentsService";

export default function SessionCreation() {
    const { mentorshipRequestId, learnerId, mentorId } = useParams();
    const [sessionTime, setSessionTime] = useState("");
    const [duration, setDuration] = useState(0);
    const [videoLink, setVideoLink] = useState("");
    const [amount, setAmount] = useState("");
    const navigate = useNavigate();

    const handleSessionCreation = async (e: FormEvent) => {
        e.preventDefault();

        const newSession: ISession = {
            mentorshipRequestId: mentorshipRequestId!,
            sessionTime,
            duration,
            videoLink,
            amount: parseFloat(amount) || 0,
        };

        const newPayment: IPayment = {
            payerId: learnerId!,
            mentorId: mentorId!,
            amount: newSession.amount,
            status: PaymentsStatus.Pending,
        }

        try {
            const sessionResponse = await CreateSession(newSession);
            const sessionId = sessionResponse.data.id;
            await SendPayment(newPayment);
            
            showToast("Session created successfully!", "success");
            navigate(`/Session/${sessionId}`);
        } catch {
            showToast("Something went wrong!", "error");
        }
    };

    return (
        <div className="flex min-h-full flex-1 justify-center px-6 py-12 lg:px-8">
            <div className="relative w-full max-w-lg">
                <img alt="SkillSwap" src={Img} className="mx-auto h-12 w-auto" /><br />

                <div className="w-full bg-gray-800 max-w-lg border border-gray-600 rounded-lg shadow">
                    <div className="p-6 space-y-4 md:space-y-6 sm:p-8">
                        <h1 className="text-xl font-bold leading-tight tracking-tight text-gray-300 md:text-2xl text-left">
                            Create Skill
                        </h1>
                        <form className="space-y-4 md:space-y-6" onSubmit={handleSessionCreation}>
                            <div>
                                <label className="block mb-2 text-sm font-medium text-gray-300">Session Time *</label>
                                <input
                                    type="datetime-local"
                                    className="bg-gray-700 border border-gray-500 text-gray-400 rounded-lg block w-full p-2.5"
                                    placeholder="Session Time"
                                    required
                                    value={sessionTime}
                                    onChange={(e) => setSessionTime(e.target.value)}
                                />
                            </div>

                            <div>
                                <label className="block mb-2 text-sm font-medium text-gray-300">Duration (minutes) *</label>
                                <input
                                    type="number"
                                    className="bg-gray-700 border border-gray-500 text-gray-400 rounded-lg block w-full p-2.5"
                                    placeholder="Duration"
                                    required
                                    value={duration}
                                    onChange={(e) => setDuration(Number(e.target.value))}
                                />
                            </div>

                            <div>
                                <label className="block mb-2 text-sm font-medium text-gray-300">Video Link (optional)</label>
                                <input
                                    type="text"
                                    className="bg-gray-700 border border-gray-500 text-gray-400 rounded-lg block w-full p-2.5"
                                    placeholder="Video Link"
                                    value={videoLink}
                                    onChange={(e) => setVideoLink(e.target.value)}
                                />
                            </div>

                            <div>
                                <label className="block mb-2 text-sm font-medium text-gray-300">Amount *</label>
                                <input
                                    type="number"
                                    min="0"
                                    step="0.01"
                                    className="bg-gray-700 border border-gray-500 text-gray-400 rounded-lg block w-full p-2.5"
                                    placeholder="Amount"
                                    value={amount}
                                    onChange={(e) => {
                                        const value = e.target.value;
                                        if (value === "" || parseFloat(value) >= 0) {
                                            setAmount(value);
                                        }
                                    }}
                                />
                            </div>

                            <button
                                type="submit"
                                className="w-full text-white bg-blue-600 hover:bg-blue-700 cursor-pointer focus:ring-4 focus:outline-none focus:ring-purple-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center"
                            >
                                Create Skill
                            </button>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
}
