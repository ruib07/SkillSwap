import { useState, useEffect } from "react";
import { ISkill } from "../types/skill";
import { useNavigate } from "react-router-dom";
import { GetAllSkills } from "../services/skillsService";
import Header from "../layouts/Header";
import SkillSearchBar from "../components/Search/SkillSearchBar";

export default function Skills() {
    const [skills, setSkills] = useState<ISkill[]>([]);
    const [filteredSkills, setFilteredSkills] = useState<ISkill[]>([]);
    const [error, setError] = useState<string | null>(null);
    const [currentPage, setCurrentPage] = useState(1);
    const [skillsPerPage] = useState(18);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchSkills = async () => {
            try {
                const response = await GetAllSkills();
                const skillsArray = response.data.$values || [];
                setSkills(skillsArray);
                setFilteredSkills(skillsArray);
            } catch {
                setError("Failed to load skills.");
            }
        };

        fetchSkills();
    }, []);

    const handleSkillClick = async (skillId: string) => {
        navigate(`/Skill/${encodeURIComponent(skillId)}`);
    };

    const indexOfLastProject = currentPage * skillsPerPage;
    const indexOfFirstProject = indexOfLastProject - skillsPerPage;
    const currentSkills = filteredSkills.slice(indexOfFirstProject, indexOfLastProject);

    const paginate = (pageNumber: number) => setCurrentPage(pageNumber);
    const totalPages = Math.ceil(filteredSkills.length / skillsPerPage);

    return (
        <>
            <Header />
            <br />
            <br />
            <div className="flex flex-col items-center p-8 min-h-screen">
                <h1 className="text-3xl font-bold mb-6 text-center text-gray-200">
                    Skills
                </h1>
                {error && <p className="text-red-500 text-center mb-6">{error}</p>}

                <SkillSearchBar
                    skills={skills}
                    setFilteredSkills={setFilteredSkills}
                />

                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 w-full max-w-8xl">
                    {currentSkills.map((skill, index) => (
                        <div
                            key={index}
                            onClick={() => handleSkillClick(skill.id!)}
                            className="max-w-sm rounded-xl overflow-hidden hover:shadow-md hover:shadow-gray-700 shadow-md cursor-pointer bg-gray-800 transform transition duration-300 hover:scale-105 hover:shadow-2xl border border-gray-500"
                        >
                            <div className="px-6 py-4">
                                <div className="font-bold text-xl text-center mb-2 text-gray-200">
                                    {skill.name}
                                </div>
                                <p className="text-gray-400 text-base">{skill.description}</p>
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
    )
}