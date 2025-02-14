import { useState, useEffect, FormEvent } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faEye, faEyeSlash } from "@fortawesome/free-solid-svg-icons";
import { RecoverPasswordChange } from "../../services/recoverPasswordService";
import { showToast } from "../../utils/toastHelper";
import Img from "/assets/SkillSwap-Logo.png";

export default function RecoverPasswordUpdate() {
    const [newPassword, setNewPassword] = useState("");
    const [confirmNewPassword, setConfirmNewPassword] = useState("");
    const [token, setToken] = useState("");
    const [visibleNewPassword, setVisibleNewPassword] = useState(false);
    const [visibleConfirmPassword, setVisibleConfirmPassword] = useState(false);
    const navigate = useNavigate();
    const location = useLocation();

    useEffect(() => {
        const queryParams = new URLSearchParams(location.search);
        const tokenParam = queryParams.get("token");
        if (tokenParam) {
            setToken(tokenParam);
        }
    }, [location]);

    const handlePasswordChange = async (event: FormEvent) => {
        event.preventDefault();

        if (newPassword !== confirmNewPassword) {
            showToast("Passwords do not match!", "error");
            return;
        }

        try {
            await RecoverPasswordChange({ token, newPassword, confirmNewPassword });
            showToast("Password changed successfully!", "success");
            navigate("/Authentication/Login");
        } catch {
            showToast("Password has not been changed!", "error");
        }
    };

    return (
        <div className="flex min-h-full flex-1 justify-center px-6 py-12 lg:px-8">
            <div className="relative w-full max-w-lg">
                <img alt="CreativeHub" src={Img} className="mx-auto h-12 w-auto" /><br />

                <div className="w-full bg-gray-800 max-w-lg border border-gray-600 rounded-lg shadow">
                    <div className="p-6 space-y-4 md:space-y-6 sm:p-8">
                        <form className="space-y-4 md:space-y-6" onSubmit={handlePasswordChange}>

                            <div>
                                <label className="block mb-2 text-sm font-medium text-gray-300">New Password *</label>
                                <div className="mt-2 relative">
                                    <input
                                        type={visibleNewPassword ? "text" : "password"}
                                        className="bg-gray-700 border border-gray-500 text-gray-400 rounded-lg block w-full p-2.5"
                                        placeholder="New password"
                                        required
                                        value={newPassword}
                                        onChange={(e) => setNewPassword(e.target.value)}
                                    />
                                    <span
                                        className="absolute inset-y-0 right-0 pr-3 flex items-center cursor-pointer text-gray-400"
                                        onClick={() => setVisibleNewPassword(!visibleNewPassword)}
                                    >
                                        <FontAwesomeIcon
                                            icon={visibleNewPassword ? faEye : faEyeSlash}
                                        />
                                    </span>
                                </div>
                            </div>

                            <div>
                                <label className="block mb-2 text-sm font-medium text-gray-300">Confirm New Password *</label>
                                <div className="mt-2 relative">
                                    <input
                                        type={visibleConfirmPassword ? "text" : "password"}
                                        className="bg-gray-700 border border-gray-500 text-gray-400 rounded-lg block w-full p-2.5"
                                        placeholder="Confirm new password"
                                        required
                                        value={confirmNewPassword}
                                        onChange={(e) => setConfirmNewPassword(e.target.value)}
                                    />
                                    <span
                                        className="absolute inset-y-0 right-0 pr-3 flex items-center cursor-pointer text-gray-400"
                                        onClick={() =>
                                            setVisibleConfirmPassword(!visibleConfirmPassword)
                                        }
                                    >
                                        <FontAwesomeIcon
                                            icon={visibleConfirmPassword ? faEye : faEyeSlash}
                                        />
                                    </span>
                                </div>
                            </div>

                            <button
                                type="submit"
                                className="w-full text-white bg-blue-600 hover:bg-blue-700 cursor-pointer focus:ring-4 focus:outline-none focus:ring-purple-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center"
                            >
                                Change Password
                            </button>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
}