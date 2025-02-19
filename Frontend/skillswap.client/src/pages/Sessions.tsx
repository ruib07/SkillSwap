import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import Header from "../layouts/Header";
import { GetUserById } from "../services/usersService";
import { ISession } from "../types/session";
import { IUser } from "../types/user";
import { GetMentorshipRequestsByLearner, GetMentorshipRequestsByMentor } from "../services/mentorshipRequestsService";
import { IMentorshipRequest } from "../types/mentorshipRequest";
import { GetSessionsByMentorshipRequest } from "../services/sessionsService";
import MentorshipRequestsModal from "../components/Mentorships/MentorshipRequestsModal";

export default function Sessions() {
    const [, setUser] = useState<IUser | null>(null);
    const [mentorshipRequests, setMentorshipRequests] = useState<IMentorshipRequest[]>([]);
    const [selectedMentorshipRequest, setSelectedMentorshipRequest] = useState<string | null>(null);
    const [sessions, setSessions] = useState<ISession[]>([]);
    const [error, setError] = useState<string | null>(null);
    const [currentPage, setCurrentPage] = useState(1);
    const [sessionsPerPage] = useState(14);
    const [isModalOpen, setIsModalOpen] = useState(true);

    const navigate = useNavigate();
    const userId = localStorage.getItem("userId") || sessionStorage.getItem("userId");

    useEffect(() => {
        const fetchData = async () => {
            try {
                const userResponse = await GetUserById(userId!);
                setUser(userResponse.data);

                let mentorshipRequestRes;
                if (userResponse.data.isMentor) {
                    mentorshipRequestRes = await GetMentorshipRequestsByMentor(userId!);
                } else {
                    mentorshipRequestRes = await GetMentorshipRequestsByLearner(userId!);
                }

                setMentorshipRequests(mentorshipRequestRes.data);
            } catch {
                setError("Failed to load mentorship requests.");
            }
        };

        fetchData();
    }, [userId]);

    useEffect(() => {
        const fetchSessions = async () => {
            if (!selectedMentorshipRequest) return;

            try {
                const sessionsResponse = await GetSessionsByMentorshipRequest(selectedMentorshipRequest);
                setSessions(sessionsResponse.data);
            } catch {
                setError("Failed to load sessions.");
            }
        };

        fetchSessions();
    }, [selectedMentorshipRequest]);

    const handleMentorshipRequestSelection = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const selectedId = e.target.value;
        if (selectedId) {
            setSelectedMentorshipRequest(selectedId);
            setIsModalOpen(false);
        }
    };

    const handleSessionClick = (sessionId: string) => {
        navigate(`/Session/${encodeURIComponent(sessionId)}`);
    };

    const indexOfLastProject = currentPage * sessionsPerPage;
    const indexOfFirstProject = indexOfLastProject - sessionsPerPage;
    const currentSessions = sessions.slice(indexOfFirstProject, indexOfLastProject);

    const paginate = (pageNumber: number) => setCurrentPage(pageNumber);
    const totalPages = Math.ceil(sessions.length / sessionsPerPage);

    return (
        <>
            <Header />
            <br />
            <br />
            {isModalOpen && (
                <MentorshipRequestsModal onClose={() => setIsModalOpen(false)}>
                    <h2 className="text-xl text-gray-100 font-bold mb-4">Select the mentorship request you want to see the sessions</h2>
                    <select
                        className="w-full p-2 border border-gray-300 rounded text-gray-500"
                        onChange={handleMentorshipRequestSelection}
                    >
                        <option value="">Select a mentorship request</option>
                        {mentorshipRequests.map((request) => (
                            <option key={request.id} value={request.id}>
                                {request.id}
                            </option>
                        ))}
                    </select>
                </MentorshipRequestsModal>
            )}

            <div className="flex flex-col items-center p-8">
                <h1 className="text-3xl font-bold mb-6 text-center text-gray-200">
                    Sessions
                </h1>
                {error && <p className="text-red-500 text-center mb-6">{error}</p>}

                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 w-full max-w-8xl">
                    {currentSessions.map((session, index) => (
                        <div
                            key={index}
                            onClick={() => handleSessionClick(session.id!)}
                            className="max-w-sm rounded-xl overflow-hidden hover:shadow-md hover:shadow-gray-700 shadow-md cursor-pointer bg-gray-800 transform transition duration-300 hover:scale-105 hover:shadow-2xl border border-gray-500"
                        >
                            <div className="px-6 py-4">
                                <div className="flex">
                                    <p className="text-gray-400 text-base">Session Time: </p>
                                    <p className="ms-2 text-gray-300 text-base">{new Date(session.sessionTime).toLocaleString().slice(0, -3)}</p>
                                </div>

                                <div className="flex">
                                    <p className="text-gray-400 text-base">Duration: </p>
                                    <p className="ms-2 text-gray-300 text-base">{session.duration} minutes</p>
                                </div>

                                <div className="flex">
                                    <p className="text-gray-400 text-base">Video Link: </p>
                                    <p className="ms-2 text-gray-300 text-base">{session.videoLink || "none"}</p>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>

                <div className="flex justify-center mt-8">
                    <button
                        onClick={() => paginate(currentPage - 1)}
                        disabled={currentPage === 1}
                        className="px-4 py-2 text-white bg-blue-500 rounded-l-md hover:bg-blue-600 disabled:bg-gray-400"
                    >
                        &lt;
                    </button>
                    <span className="px-4 py-2 text-white bg-gray-700">
                        Page {currentPage} of {totalPages}
                    </span>
                    <button
                        onClick={() => paginate(currentPage + 1)}
                        disabled={currentPage === totalPages}
                        className="px-4 py-2 text-white bg-blue-500 rounded-r-md hover:bg-blue-600 disabled:bg-gray-400"
                    >
                        &gt;
                    </button>
                </div>
            </div>
        </>
    );
}
