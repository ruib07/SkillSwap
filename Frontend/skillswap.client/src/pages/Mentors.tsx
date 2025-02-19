import { useEffect, useState } from "react";
import { IUser } from "../types/user";
import { useNavigate } from "react-router-dom";
import { GetMentors } from "../services/usersService";
import Header from "../layouts/Header";
import MentorSearchBar from "../components/Search/MentorSearchBar";

export default function Mentors() {
    const [mentors, setMentors] = useState<IUser[]>([]);
    const [filteredMentors, setFilteredMentors] = useState<IUser[]>([]);
    const [error, setError] = useState<string | null>(null);
    const [currentPage, setCurrentPage] = useState(1);
    const [mentorsPerPage] = useState(14);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchMentors = async () => {
            try {
                const response = await GetMentors();
                const mentorsArray = response.data || [];
                setMentors(mentorsArray);
                setFilteredMentors(mentorsArray);
            } catch {
                setError("Failed to load mentors.");
            }
        };

        fetchMentors();
    }, []);

    const handleMentorClick = (mentorId: string) => {
        navigate(`/Mentor/${encodeURIComponent(mentorId)}`);
    };

    const indexOfLastMentor = currentPage * mentorsPerPage;
    const indexOfFirstMentor = indexOfLastMentor - mentorsPerPage;
    const currentMentors = filteredMentors.slice(indexOfFirstMentor, indexOfLastMentor);

    const paginate = (pageNumber: number) => setCurrentPage(pageNumber);
    const totalPages = Math.ceil(filteredMentors.length / mentorsPerPage);

    return (
        <>
            <Header /><br /><br />
            <div className="p-8 bg-gray-900">
                <h1 className="text-4xl font-bold text-center text-gray-200 mb-8">
                    Available Mentors
                </h1>

                <div className="flex justify-center mb-6">
                    <MentorSearchBar
                        mentors={mentors}
                        setFilteredMentors={setFilteredMentors}
                    />
                </div>

                {error && <p className="text-red-500 text-center mb-6">{error}</p>}

                <div className="space-y-8">
                    {currentMentors.map((mentor) => (
                        <div
                            key={mentor.id}
                            onClick={() => handleMentorClick(mentor.id!)}
                            className="flex items-center p-6 bg-gray-800 rounded-lg shadow-lg hover:shadow-2xl hover:bg-gray-700 transition duration-300 cursor-pointer"
                        >
                            <img
                                src={mentor.profilePicture || "/assets/default-avatar.png"}
                                alt={mentor.name}
                                className="w-20 h-20 rounded-full object-cover border-2 border-gray-600"
                            />

                            <div className="ml-6">
                                <h3 className="text-xl font-semibold text-gray-200">
                                    {mentor.name}
                                </h3>
                                <p className="text-sm text-gray-400">{mentor.email}</p>
                                <p className="mt-2 text-gray-300 text-sm">{mentor.bio}</p>
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
