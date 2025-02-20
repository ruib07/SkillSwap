import Header from "../layouts/Header";
import { useNavigate } from "react-router-dom";
import { ISkill } from "../types/skill";
import { IUser } from "../types/user";
import { useEffect, useState } from "react";
import { GetAllSkills } from "../services/skillsService";
import { GetMentors, GetUserById } from "../services/usersService";

export default function Home() {
    const [isMentor, setIsMentor] = useState<boolean | null>(null);
    const [skills, setSkills] = useState<ISkill[]>([]);
    const [mentors, setMentors] = useState<IUser[]>([]);
    const [skillsCarouselIndex, setSkillsCarouselIndex] = useState(0);
    const [mentorsCarouselIndex, setMentorsCarouselIndex] = useState(0);
    const [, setError] = useState<string | null>(null);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchUser = async () => {
            try {
                const userId = localStorage.getItem("userId") || sessionStorage.getItem("userId");

                if (!userId) {
                    setIsMentor(null);
                    return;
                }

                const userResponse = await GetUserById(userId);
                setIsMentor(userResponse.data.isMentor);
            } catch {
                setError("Failed to fetch user.");
            }
        };

        fetchUser();
    }, []);

    useEffect(() => {
        const fetchSkillsAndMentors = async () => {
            try {
                const skillsResponse = await GetAllSkills();
                setSkills(skillsResponse.data);

                const mentorsResponse = await GetMentors();
                setMentors(mentorsResponse.data);
            } catch {
                setError("Failed to fetch skills and mentors.");
            }
        }

        fetchSkillsAndMentors();
    }, []);

    useEffect(() => {
        const interval = setInterval(() => {
            setSkillsCarouselIndex((prevIndex) => (prevIndex + 1) % skills.length);
        }, 5000);

        return () => clearInterval(interval);
    }, [skills.length]);

    useEffect(() => {
        const interval = setInterval(() => {
            setMentorsCarouselIndex((prevIndex) => (prevIndex + 1) % mentors.length);
        }, 5000);

        return () => clearInterval(interval);
    }, [mentors.length]);

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
                            onClick={() => navigate("/Mentors")}
                        >
                            Get Started
                        </button>
                    </div>
                </section>

                {isMentor && (
                    <section className="py-16">
                        <div className="container mx-auto text-center">
                            <h3 className="text-3xl font-bold text-blue-500 mb-8">
                                Some skills you can add
                            </h3>
                            <div className="relative max-w-4xl mx-auto">
                                {skills.length > 0 && (
                                    <div className="flex items-center">
                                        <button
                                            className="absolute cursor-pointer left-0 z-10 bg-blue-500 text-white rounded-full p-3 hover:bg-blue-600 shadow-lg transform -translate-x-1/2"
                                            onClick={() =>
                                                setSkillsCarouselIndex(
                                                    (prevIndex) => (prevIndex - 1 + skills.length) % skills.length
                                                )
                                            }
                                        >
                                            &lt;
                                        </button>
                                        <div className="w-full px-8">
                                            <div className="p-6 shadow-lg rounded-lg bg-gray-800 border border-gray-600">
                                                <h4 className="text-2xl font-bold text-blue-500 mb-2">
                                                    {skills[skillsCarouselIndex].name}
                                                </h4>
                                                <p className="text-gray-400 mb-2">
                                                    {skills[skillsCarouselIndex].description}
                                                </p>
                                            </div>
                                        </div>
                                        <button
                                            className="absolute cursor-pointer right-0 z-10 bg-blue-500 text-white rounded-full p-3 hover:bg-blue-600 shadow-lg transform translate-x-1/2"
                                            onClick={() =>
                                                setSkillsCarouselIndex(
                                                    (prevIndex) => (prevIndex + 1) % skills.length
                                                )
                                            }
                                        >
                                            &gt;
                                        </button>
                                    </div>
                                )}
                            </div>
                        </div>
                    </section>
                )}


                <section className="py-10">
                    <div className="container mx-auto text-center">
                        <h3 className="text-3xl font-bold text-blue-500 mb-8">
                            Some of the available mentors
                        </h3>
                        <div className="relative max-w-4xl mx-auto">
                            {mentors.length > 0 && (
                                <div className="flex items-center">
                                    <button
                                        className="absolute cursor-pointer left-0 z-10 bg-blue-500 text-white rounded-full p-3 hover:bg-blue-600 shadow-lg transform -translate-x-1/2"
                                        onClick={() =>
                                            setMentorsCarouselIndex(
                                                (prevIndex) => (prevIndex - 1 + mentors.length) % mentors.length
                                            )
                                        }
                                    >
                                        &lt;
                                    </button>
                                    <div className="w-full px-8">
                                        <div className="p-6 shadow-lg rounded-lg bg-gray-800 border border-gray-600 flex items-center gap-6">
                                            <img
                                                src={mentors[mentorsCarouselIndex].profilePicture || "/placeholder.png"}
                                                alt={mentors[mentorsCarouselIndex].name}
                                                className="w-20 h-20 object-cover rounded-full border-2 border-blue-500"
                                            />
                                            <div>
                                                <h4 className="text-2xl font-bold text-blue-500 mb-2">
                                                    {mentors[mentorsCarouselIndex].name}
                                                </h4>
                                                <h5 className="text-gray-400 mb-2">
                                                    {mentors[mentorsCarouselIndex].email}
                                                </h5>
                                                <p className="text-gray-400">
                                                    {mentors[mentorsCarouselIndex].bio}
                                                </p>
                                            </div>
                                        </div>
                                    </div>
                                    <button
                                        className="absolute cursor-pointer right-0 z-10 bg-blue-500 text-white rounded-full p-3 hover:bg-blue-600 shadow-lg transform translate-x-1/2"
                                        onClick={() =>
                                            setMentorsCarouselIndex(
                                                (prevIndex) => (prevIndex + 1) % mentors.length
                                            )
                                        }
                                    >
                                        &gt;
                                    </button>
                                </div>
                            )}
                        </div>
                    </div>
                </section>

                <section className="py-10">
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