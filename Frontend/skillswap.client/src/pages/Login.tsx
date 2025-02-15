import { FormEvent, useState } from "react";
import { useNavigate } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faEye, faEyeSlash } from "@fortawesome/free-solid-svg-icons";
import { ILogin } from "../types/authentication";
import { Login } from "../services/authenticationService";
import { jwtDecode } from "jwt-decode";
import { showToast } from "../utils/toastHelper";
import Img from "/assets/SkillSwap-Logo.png";

export default function Authentication() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [visible, setVisible] = useState(true);
    const [rememberMe, setRememberMe] = useState(false);
    const navigate = useNavigate();

    const handleRegister = async (e: FormEvent) => {
        e.preventDefault();

        const auth: ILogin = { email, password };

        try {
            const res = await Login(auth);
            const token = res.data.accessToken;
            const decodedToken: any = jwtDecode(token);
            const userId = decodedToken.Id;

            if (rememberMe) {
                localStorage.setItem("token", token);
                localStorage.setItem("userId", userId);
            } else {
                sessionStorage.setItem("token", token);
                sessionStorage.setItem("userId", userId);
            }

            navigate("/");
        } catch {
            showToast("Something went wrong!", "error");
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
                            Sign in to your account
                        </h1>
                        <form className="space-y-4 md:space-y-6" onSubmit={handleRegister}>

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

                            <div className="flex items-center justify-between">
                                <div className="flex items-start">
                                    <div className="ml-3 text-sm">
                                        <label className="text-gray-500 dark:text-gray-300">Remember me</label>
                                    </div>
                                    <div className="ms-2 flex items-center h-5">
                                        <input
                                            aria-describedby="remember"
                                            type="checkbox"
                                            checked={rememberMe}
                                            onClick={() => setRememberMe(!rememberMe)}
                                            className="w-4 h-4 border rounded focus:ring-3 bg-gray-700 border-gray-900 focus:ring-blue-600 ring-offset-gray-800"
                                        />
                                    </div>
                                </div>
                                <a
                                    href="/RecoverPassword/SendEmail"
                                    className="font-semibold text-blue-500 hover:underline"
                                >
                                    Forgot password?
                                </a>
                            </div>


                            <button
                                type="submit"
                                className="w-full text-white bg-blue-600 hover:bg-blue-700 cursor-pointer focus:ring-4 focus:outline-none focus:ring-purple-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center"
                            >
                                Sign In
                            </button>

                            <p className="text-sm font-light text-gray-400 text-left">
                                Don’t have an account yet?{" "}
                                <a
                                    href="/Authentication/Registration"
                                    className="font-medium text-blue-500 hover:underline">
                                    Sign Up
                                </a>
                            </p>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
}
