import { useEffect, useState } from "react";
import Header from "../layouts/Header";
import { GetMentorshipRequestsByLearner, GetMentorshipRequestsByMentor, UpdateMentorshipRequest } from "../services/mentorshipRequestsService";
import { GetSkillById } from "../services/skillsService";
import { GetUserById } from "../services/usersService";
import { IMentorshipRequest, mentorshipStatusMap } from "../types/mentorshipRequest";
import { IUser } from "../types/user";

export default function MentorshipRequests() {
    const [user, setUser] = useState<IUser | null>(null);
    const [mentorshipRequests, setMentorshipRequests] = useState<IMentorshipRequest[]>([]);
    const [requestDetails, setRequestDetails] = useState<{ [key: string]: { name: string, skill: string } }>({});
    const [error, setError] = useState<string | null>(null);
    const [currentPage, setCurrentPage] = useState(1);
    const [mentorsPerPage] = useState(14);

    useEffect(() => {
        const fetchUserAndRequests = async () => {
            try {
                const userId = localStorage.getItem("userId") || sessionStorage.getItem("userId");
                if (!userId) {
                    setError("User not found.");
                    return;
                }

                const userResponse = await GetUserById(userId);
                const userData = userResponse.data;
                setUser(userData);

                let requestsResponse;
                if (userData.isMentor) {
                    requestsResponse = await GetMentorshipRequestsByMentor(userId);
                } else {
                    requestsResponse = await GetMentorshipRequestsByLearner(userId);
                }

                const requests = requestsResponse.data.$values || [];
                setMentorshipRequests(requests);

                requests.forEach(async (request: IMentorshipRequest) => {
                    const personId = userData.isMentor ? request.learnerId : request.mentorId;
                    const personResponse = await GetUserById(personId);
                    const skillResponse = await GetSkillById(request.skillId);

                    setRequestDetails(prev => ({
                        ...prev,
                        [request.id!]: { name: personResponse.data.name, skill: skillResponse.data.name }
                    }));
                });
            } catch {
                setError("Failed to load mentorship requests.");
            }
        };

        fetchUserAndRequests();
    }, []);

    const handleUpdateStatus = async (mentorshipRequestId: string, newStatus: number) => {
        try {
            await UpdateMentorshipRequest(mentorshipRequestId, { status: newStatus });

            setMentorshipRequests(prev =>
                prev.map(request =>
                    request.id === mentorshipRequestId ? { ...request, status: newStatus } : request
                )
            );
        } catch {
            setError("Failed to update mentorship request.");
        }
    };

    const indexOfLastMentorshipReq = currentPage * mentorsPerPage;
    const indexOfFirstMentorshipReq = indexOfLastMentorshipReq - mentorsPerPage;
    const currentMentorshipReq = mentorshipRequests.slice(indexOfFirstMentorshipReq, indexOfLastMentorshipReq);

    const paginate = (pageNumber: number) => setCurrentPage(pageNumber);
    const totalPages = Math.ceil(mentorshipRequests.length / mentorsPerPage);

    return (
        <>
            <Header /><br /><br />
            <div className="p-8 bg-gray-900 min-h-screen">
                <h1 className="text-4xl font-bold text-center text-gray-200 mb-8">
                    Your Mentorship Requests
                </h1>

                {error && <p className="text-red-500 text-center mb-6">{error}</p>}

                {mentorshipRequests.length === 0 ? (
                    <p className="text-gray-300 text-center">No mentorship requests found.</p>
                ) : (
                    <div className="space-y-6">
                        {currentMentorshipReq.map((request) => (
                            <div
                                key={request.id}
                                className="p-6 bg-gray-800 rounded-lg shadow-lg hover:shadow-2xl hover:bg-gray-700 transition duration-300"
                            >
                                <div className="flex justify-between items-center">
                                    <div>
                                        <h3 className="text-xl font-semibold text-gray-200">
                                            {requestDetails[request.id!]?.name}
                                        </h3>
                                        <p className="text-gray-400">
                                            Skill requested:
                                            <span className="ms-1 font-bold text-gray-300">
                                                {requestDetails[request.id!]?.skill}
                                            </span>
                                        </p>
                                        <p className="text-gray-400">
                                            Status:
                                            <span className="ms-1 font-bold text-gray-300">
                                                {mentorshipStatusMap[request.status]}
                                            </span>
                                        </p>
                                        <p className="text-gray-400">
                                            Scheduled at:
                                            <span className="ms-1 font-bold text-gray-300">
                                                {new Date(request.scheduledAt).toLocaleString().slice(0, -3)}
                                            </span>
                                        </p>
                                    </div>

                                    <div className="mt-4 flex space-x-4">
                                        {user?.isMentor ? (
                                            <>
                                                <button
                                                    onClick={() => handleUpdateStatus(request.id!, 1)}
                                                    disabled={request.status !== 0}
                                                    className={`cursor-pointer px-4 py-2 text-white font-bold rounded ${request.status === 0 ? "bg-green-600 hover:bg-green-700" : "bg-gray-500 cursor-not-allowed"
                                                        }`}
                                                >
                                                    Accept
                                                </button>

                                                <button
                                                    onClick={() => handleUpdateStatus(request.id!, 3)}
                                                    className="cursor-pointer px-4 py-2 text-white font-bold bg-red-600 hover:bg-red-700 rounded"
                                                >
                                                    Cancel
                                                </button>
                                            </>
                                        ) : (
                                            <button
                                                onClick={() => handleUpdateStatus(request.id!, 3)}
                                                className="cursor-pointer px-4 py-2 text-white font-bold bg-red-600 hover:bg-red-700 rounded"
                                            >
                                                Cancel
                                            </button>
                                        )}
                                    </div>
                                </div>
                            </div>
                        ))}
                    </div>
                )}

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
