import { FormEvent, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { showToast } from "../../utils/toastHelper";
import Img from "/assets/SkillSwap-Logo.png";
import { IMentorshipRequest } from "../../types/mentorshipRequest";
import { CreateMentorshipRequest } from "../../services/mentorshipRequestsService";
import { GetMentors, GetUsers, GetUserById } from "../../services/usersService";
import { GetAllSkills } from "../../services/skillsService";

export default function NewMentorshipRequest() {
    const [mentorId, setMentorId] = useState("");
    const [learnerId, setLearnerId] = useState("");
    const [skillId, setSkillId] = useState("");
    const [scheduledAt, setScheduledAt] = useState("");
    const [, setError] = useState<string | null>(null);
    const [mentors, setMentors] = useState<{ id: string; name: string }[]>([]);
    const [learners, setLearners] = useState<{ id: string; name: string }[]>([]);
    const [skills, setSkills] = useState<{ id: string; name: string }[]>([]);
    const [isDropdownMentorVisible, setIsDropdownMentorVisible] = useState(false);
    const [isDropdownUserVisible, setIsDropdownUserVisible] = useState(false);
    const [isDropdownSkillVisible, setIsDropdownSkillVisible] = useState(false);
    const [userIsMentor, setUserIsMentor] = useState(false);
    const navigate = useNavigate();

    const userId = localStorage.getItem("userId") || sessionStorage.getItem("userId");

    useEffect(() => {
        const fetchData = async () => {
            try {
                const mentorsResponse = await GetMentors();
                const mentorsArray = mentorsResponse.data.$values || [];
                setMentors(mentorsArray);

                const usersResponse = await GetUsers();
                const usersArray = usersResponse.data.$values || [];
                setLearners(usersArray.filter((user: any) => !user.isMentor));

                const skillsResponse = await GetAllSkills();
                const skillsArray = skillsResponse.data.$values || [];
                setSkills(skillsArray);

                if (userId) {
                    const userResponse = await GetUserById(userId);
                    setUserIsMentor(userResponse.data.isMentor);
                }
            } catch {
                setError("Failed to load data.");
            }
        };

        fetchData();
    }, [userId]);

    const handleRegister = async (e: FormEvent) => {
        e.preventDefault();

        const newMentorshipRequest: IMentorshipRequest = {
            mentorId: "",
            learnerId: "",
            skillId,
            status: 0,
            scheduledAt,
        };

        if (userIsMentor) {
            newMentorshipRequest.mentorId = userId || "";
            newMentorshipRequest.learnerId = learnerId; 
        } else {
            newMentorshipRequest.mentorId = mentorId;
            newMentorshipRequest.learnerId = userId || "";
        }

        try {
            await CreateMentorshipRequest(newMentorshipRequest);
            showToast("Request completed successfully!", "success");
            navigate("/MentorshipRequests");
        } catch {
            showToast("Request was not completed!", "error");
        }
    };

    const handleMentorsSearch = (search: string) => {
        setMentorId(search);

        if (!search) {
            setMentors(mentors);
        }

        const filteredMentors = mentors.filter((mentor) =>
            mentor.name.toLowerCase().includes(search.toLowerCase())
        );
        setMentors(filteredMentors);
    };

    const handleLearnersSearch = (search: string) => {
        setLearnerId(search);

        if (!search) {
            setLearners(learners);
        }

        const filteredLearners = learners.filter((learner) =>
            learner.name.toLowerCase().includes(search.toLowerCase())
        );
        setLearners(filteredLearners);
    };

    const handleSkillsSearch = (search: string) => {
        setSkillId(search);

        if (!search) {
            setSkills(skills);
        }

        const filteredSkills = skills.filter((skill) =>
            skill.name.toLowerCase().includes(search.toLowerCase())
        );
        setSkills(filteredSkills);
    };

    return (
        <div className="flex min-h-full flex-1 justify-center px-6 py-12 lg:px-8">
            <div className="relative w-full max-w-lg">
                <img alt="SkillSwap" src={Img} className="mx-auto h-12 w-auto" /><br />

                <div className="w-full bg-gray-800 max-w-lg border border-gray-600 rounded-lg shadow">
                    <div className="p-6 space-y-4 md:space-y-6 sm:p-8">
                        <h1 className="text-xl font-bold leading-tight tracking-tight text-gray-300 md:text-2xl text-left">
                            Create mentorship request
                        </h1>
                        <form className="space-y-4 md:space-y-6" onSubmit={handleRegister}>
                            {userIsMentor ? (
                                <div>
                                    <label className="block mb-2 text-sm font-medium text-gray-300">Learner *</label>
                                    <input
                                        type="text"
                                        className="bg-gray-700 border border-gray-500 text-gray-400 rounded-lg block w-full p-2.5"
                                        placeholder="Search for learner"
                                        required
                                        value={learnerId}
                                        onChange={(e) => handleLearnersSearch(e.target.value)}
                                        onFocus={() => setIsDropdownUserVisible(true)}
                                        onBlur={() =>
                                            setTimeout(() => setIsDropdownUserVisible(false), 200)
                                        }
                                    />
                                    {isDropdownUserVisible && learners.length > 0 && (
                                        <div className="absolute z-10 bg-gray-800 text-white border rounded-md w-full mt-1 max-h-40 overflow-y-auto shadow-lg">
                                            {learners.map((learner) => (
                                                <div
                                                    key={learner.id}
                                                    className="px-4 py-2 cursor-pointer hover:bg-gray-700"
                                                    onClick={() => {
                                                        setLearnerId(learner.id);
                                                        setIsDropdownUserVisible(false);
                                                    }}
                                                >
                                                    {learner.name}
                                                </div>
                                            ))}
                                        </div>
                                    )}
                                </div>
                            ) : (
                                <div>
                                    <label className="block mb-2 text-sm font-medium text-gray-300">Mentor *</label>
                                    <input
                                        type="text"
                                        className="bg-gray-700 border border-gray-500 text-gray-400 rounded-lg block w-full p-2.5"
                                        placeholder="Search for mentor"
                                        required
                                        value={mentorId}
                                        onChange={(e) => handleMentorsSearch(e.target.value)}
                                        onFocus={() => setIsDropdownMentorVisible(true)}
                                        onBlur={() =>
                                            setTimeout(() => setIsDropdownMentorVisible(false), 200)
                                        }
                                    />
                                    {isDropdownMentorVisible && mentors.length > 0 && (
                                        <div className="absolute z-10 bg-gray-800 text-white border rounded-md w-full mt-1 max-h-40 overflow-y-auto shadow-lg">
                                            {mentors.map((mentor) => (
                                                <div
                                                    key={mentor.id}
                                                    className="px-4 py-2 cursor-pointer hover:bg-gray-700"
                                                    onClick={() => {
                                                        setMentorId(mentor.id);
                                                        setIsDropdownMentorVisible(false);
                                                    }}
                                                >
                                                    {mentor.name}
                                                </div>
                                            ))}
                                        </div>
                                    )}
                                </div>
                            )}


                            <div>
                                <label className="block mb-2 text-sm font-medium text-gray-300">Skill *</label>
                                <input
                                    type="text"
                                    className="bg-gray-700 border border-gray-500 text-gray-400 rounded-lg block w-full p-2.5"
                                    placeholder="Search for skill"
                                    required
                                    value={skillId}
                                    onChange={(e) => handleSkillsSearch(e.target.value)}
                                    onFocus={() => setIsDropdownSkillVisible(true)}
                                    onBlur={() =>
                                        setTimeout(() => setIsDropdownSkillVisible(false), 200)
                                    }
                                />
                                {isDropdownSkillVisible && skills.length > 0 && (
                                    <div className="absolute z-10 bg-gray-800 text-white border rounded-md w-full mt-1 max-h-40 overflow-y-auto shadow-lg">
                                        {skills.map((skill) => (
                                            <div
                                                key={skill.id}
                                                className="px-4 py-2 cursor-pointer hover:bg-gray-700"
                                                onClick={() => {
                                                    setSkillId(skill.id);
                                                    setIsDropdownSkillVisible(false);
                                                }}
                                            >
                                                {skill.name}
                                            </div>
                                        ))}
                                    </div>
                                )}
                            </div>

                            <div>
                                <label className="block mb-2 text-sm font-medium text-gray-300">Scheduled At *</label>
                                <input
                                    type="datetime-local"
                                    className="bg-gray-700 border border-gray-500 text-gray-400 rounded-lg block w-full p-2.5"
                                    placeholder=""
                                    value={scheduledAt}
                                    onChange={(e) => setScheduledAt(e.target.value)}
                                />
                            </div>

                            <button
                                type="submit"
                                className="w-full text-white bg-blue-600 hover:bg-blue-700 cursor-pointer focus:ring-4 focus:outline-none focus:ring-purple-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center"
                            >
                                Create Mentorship Request
                            </button>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
}
