import { FormEvent, useState } from "react";
import { useNavigate } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faEye, faEyeSlash } from "@fortawesome/free-solid-svg-icons";
import { IRegistration } from "../types/authentication";
import { Registration } from "../services/authenticationService";
import { showToast } from "../utils/toastHelper";
import Img from "/assets/SkillSwap-Logo.png";

export default function NewRegistration() {
    const [name, setName] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [bio, setBio] = useState("");
    const [profilePicture, setProfilePicture] = useState("");
    const [balance, setBalance] = useState("");
    const [isMentor, setIsMentor] = useState<boolean>(false);
    const [visible, setVisible] = useState(true);
    const navigate = useNavigate();

    const handleRegister = async (e: FormEvent) => {
        e.preventDefault();

        const newUser: IRegistration = {
            name,
            email,
            password,
            bio,
            profilePicture,
            balance: parseFloat(balance) || 0,
            isMentor,
        };

        try {
            await Registration(newUser);
            showToast("Registration completed successfully!", "success");
            navigate("/Authentication/Login");
        } catch {
            showToast("Registration was not completed!", "error");
        }
    };

    const togglePasswordVisibility = () => {
        setVisible(!visible);
    };

    return (
        <div className="flex min-h-full flex-1 justify-center px-6 py-12 lg:px-8">
            <div className="relative w-full max-w-lg">
                <img alt="CreativeHub" src={Img} className="mx-auto h-12 w-auto" /><br />

                <div className="w-full bg-gray-800 max-w-lg border border-gray-600 rounded-lg shadow">
                    <div className="p-6 space-y-4 md:space-y-6 sm:p-8">
                        <h1 className="text-xl font-bold leading-tight tracking-tight text-gray-300 md:text-2xl text-left">
                            Create an Account
                        </h1>
                        <form className="space-y-4 md:space-y-6" onSubmit={handleRegister}>
                            <div>
                                <label className="block mb-2 text-sm font-medium text-gray-300">Full Name *</label>
                                <input
                                    type="text"
                                    className="bg-gray-700 border border-gray-500 text-gray-400 rounded-lg block w-full p-2.5"
                                    placeholder="Your Name"
                                    required
                                    value={name}
                                    onChange={(e) => setName(e.target.value)}
                                />
                            </div>

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

                            <div>
                                <label className="block mb-2 text-sm font-medium text-gray-300">Password *</label>
                                <div className="relative">
                                    <input
                                        type={visible ? "password" : "text"}
                                        className="bg-gray-700 border border-gray-500 text-gray-400 rounded-lg block w-full p-2.5 pr-10"
                                        placeholder="●●●●●●●●"
                                        required
                                        value={password}
                                        onChange={(e) => setPassword(e.target.value)}
                                    />
                                    <span
                                        className="absolute inset-y-0 right-0 flex items-center pr-3 cursor-pointer text-gray-400"
                                        onClick={togglePasswordVisibility}
                                    >
                                        <FontAwesomeIcon icon={visible ? faEye : faEyeSlash} />
                                    </span>
                                </div>
                            </div>

                            <div>
                                <label className="block mb-2 text-sm font-medium text-gray-300">Bio (optional)</label>
                                <textarea
                                    className="bg-gray-700 border border-gray-500 text-gray-400 rounded-lg block w-full p-2.5"
                                    rows={3}
                                    placeholder="Tell us about yourself..."
                                    value={bio}
                                    onChange={(e) => setBio(e.target.value)}
                                />
                            </div>

                            <div>
                                <label className="block mb-2 text-sm font-medium text-gray-300">Profile Picture URL (optional)</label>
                                <input
                                    type="text"
                                    className="bg-gray-700 border border-gray-500 text-gray-400 rounded-lg block w-full p-2.5"
                                    placeholder="https://your-image-url.com"
                                    value={profilePicture}
                                    onChange={(e) => setProfilePicture(e.target.value)}
                                />
                            </div>

                            <div>
                                <label className="block mb-2 text-sm font-medium text-gray-300">Balance *</label>
                                <input
                                    type="number"
                                    min="0"
                                    step="0.01"
                                    className="bg-gray-700 border border-gray-500 text-gray-400 rounded-lg block w-full p-2.5"
                                    placeholder="Balance"
                                    value={balance}
                                    onChange={(e) => {
                                        const value = e.target.value;
                                        if (value === "" || parseFloat(value) >= 0) {
                                            setBalance(value);
                                        }
                                    }}
                                />
                            </div>

                            <div>
                                <label className="block mb-2 text-sm font-medium text-gray-300">Are you a mentor? *</label>
                                <select
                                    className="bg-gray-700 border border-gray-500 text-gray-400 rounded-lg block w-full p-2.5"
                                    value={isMentor.toString()}
                                    onChange={(e) => setIsMentor(e.target.value === "true")}
                                    required
                                >
                                    <option value="true">Yes</option>
                                    <option value="false">No</option>
                                </select>
                            </div>

                            <button
                                type="submit"
                                className="w-full text-white bg-blue-600 hover:bg-blue-700 cursor-pointer focus:ring-4 focus:outline-none focus:ring-purple-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center"
                            >
                                Sign Up
                            </button>

                            <p className="text-sm font-light text-gray-400 text-left">
                                Have an account?{" "}
                                <a
                                    href="/Authentication/Login"
                                    className="font-medium text-blue-500 hover:underline">
                                    Sign In
                                </a>
                            </p>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
}
