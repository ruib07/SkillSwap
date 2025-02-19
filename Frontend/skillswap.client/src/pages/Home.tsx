import Header from "../layouts/Header";
import { useNavigate } from "react-router-dom";

export default function Home() {
    const navigate = useNavigate();

    return (
        <>
            <Header />
            <br />
            <br />
            <div>
                <section
                    className="relative text-white py-20"
                    style={{
                        backgroundImage: "url('/assets/HomePageBanner.jpg')",
                        backgroundSize: "cover",
                        backgroundPosition: "center",
                    }}
                >
                    <div className="absolute inset-0 bg-opacity-50"></div>
                    <div className="container mx-auto text-center relative z-10">
                        <h2 className="text-5xl text-gray-100 font-bold mb-4">Welcome to SkillSwap</h2>
                        <p className="text-lg text-gray-100 mb-8">
                            Discover, request, and offer mentorship to other users.
                        </p>
                        <button
                            className="cursor-pointer bg-blue-500 text-gray-100 px-8 py-3 font-semibold rounded-md hover:bg-blue-600 transition"
                            onClick={() => navigate("/Skills")}
                        >
                            Get Started
                        </button>
                    </div>
                </section>

                <section className="py-16">
                    <div className="container mx-auto text-center">
                        <h3 className="text-3xl font-bold text-blue-500 mb-8">
                            Some skills you can add
                        </h3>
                        <div className="relative max-w-4xl mx-auto">
                            <p>Meter carousel de cards com skills</p>
                        </div>
                    </div>
                </section>

                <section className="py-16">
                    <div className="container mx-auto text-center">
                        <h3 className="text-3xl font-bold text-blue-500 mb-8">
                            Some of the available mentors
                        </h3>
                        <div className="relative max-w-4xl mx-auto">
                            <p>Meter carousel de cards com mentors</p>
                        </div>
                    </div>
                </section>

                <section className="py-20">
                    <div className="container mx-auto text-center">
                        <h3 className="text-3xl font-bold text-blue-500 mb-12">
                            Why Choose SkillSwap?
                        </h3>
                        <div className="grid grid-cols-1 md:grid-cols-3 gap-8 cursor-pointer">
                            <div className="p-6 bg-gray-800 border border-gray-600 shadow-md rounded-lg hover:shadow-md hover:shadow-gray-700 hover:scale-105 transition-transform transform">
                                <img
                                    src="/assets/icons/Mentor.png"
                                    alt="Mentoring"
                                    className="w-12 mx-auto mb-4"
                                />
                                <h4 className="text-xl font-semibold text-blue-500 mb-2">
                                    Mentorship Opportunities
                                </h4>
                                <p className="text-gray-300">
                                    Access experienced mentors to guide you through your professional journey.
                                </p>
                            </div>

                            <div className="p-6 bg-gray-800 border border-gray-600 shadow-md rounded-lg hover:shadow-md hover:shadow-gray-700 hover:scale-105 transition-transform transform">
                                <img
                                    src="assets/icons/Coding.png"
                                    alt="Software Development"
                                    className="w-12 mx-auto mb-4"
                                />
                                <h4 className="text-xl font-semibold text-blue-500 mb-2">
                                    Software Development Services
                                </h4>
                                <p className="text-gray-300">
                                    Hire skilled developers to bring your ideas to life with high-quality code.
                                </p>
                            </div>

                            <div className="p-6 bg-gray-800 border border-gray-600 shadow-md rounded-lg hover:shadow-md hover:shadow-gray-700 hover:scale-105 transition-transform transform">
                                <img
                                    src="assets/icons/Community.png"
                                    alt="Community Support"
                                    className="w-12 mx-auto mb-4"
                                />
                                <h4 className="text-xl font-semibold text-blue-500 mb-2">
                                    Community Support
                                </h4>
                                <p className="text-gray-300">
                                    Connect with a network of professionals and enthusiasts to grow your skills.
                                </p>
                            </div>
                        </div>
                    </div>
                </section>
            </div>
        </>
    );
}