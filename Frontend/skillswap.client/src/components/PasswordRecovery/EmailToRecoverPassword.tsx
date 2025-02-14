import { FormEvent, useState } from "react";
import { RecoverPasswordSendEmail } from "../../services/recoverPasswordService";
import { showToast } from "../../utils/toastHelper";
import Img from "/assets/SkillSwap-Logo.png";

export default function RecoverPasswordEmail() {
    const [email, setEmail] = useState("");

    const handleEmailSending = async (event: FormEvent) => {
        event.preventDefault();
        try {
            const sendEmailData = { email };
            await RecoverPasswordSendEmail(sendEmailData);
            showToast("Recovery email sent successfully!", "success");
        } catch {
            showToast("Error sending recovery email!", "error");
        }
    };

    return (
        <div className="flex min-h-full flex-1 justify-center px-6 py-12 lg:px-8">
            <div className="relative w-full max-w-lg">
                <img alt="CreativeHub" src={Img} className="mx-auto h-12 w-auto" /><br />

                <div className="w-full bg-gray-800 max-w-lg border border-gray-600 rounded-lg shadow">
                    <div className="p-6 space-y-4 md:space-y-6 sm:p-8">
                        <form className="space-y-4 md:space-y-6" onSubmit={handleEmailSending}>

                            <div>
                                <label className="block mb-2 text-sm font-medium text-gray-300">Email *</label>
                                <input
                                    type="email"
                                    className="bg-gray-700 border border-gray-500 text-gray-400 rounded-lg block w-full p-2.5"
                                    placeholder="name@example.com"
                                    required
                                    value={email}
                                    onChange={(e) => setEmail(e.target.value)}
                                />
                            </div>

                            <button
                                type="submit"
                                className="w-full text-white bg-blue-600 hover:bg-blue-700 cursor-pointer focus:ring-4 focus:outline-none focus:ring-purple-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center"
                            >
                                Send Email
                            </button>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
}