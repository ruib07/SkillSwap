import { useEffect, useState } from "react";
import UserProfileHeader from "../../layouts/ProfileHeader";
import { useNavigate } from "react-router-dom";
import { ISkill } from "../../types/skill";
import { GetSkillsByUser } from "../../services/userSkillsService";
import RemoveSkill from "../Skills/DeleteSkillModal";

export default function UserSkills() {
    const [skills, setSkills] = useState<ISkill[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [showDeleteModal, setShowDeleteModal] = useState<string | null>(null);
    const [expandedDescription, setExpandedDescription] = useState<string | null>(null);
    const navigate = useNavigate();
    const userId = localStorage.getItem("userId") || sessionStorage.getItem("userId");

    useEffect(() => {
        const fetchUserSkills = async () => {
            if (!userId) {
                setLoading(false);
                return;
            }

            try {
                const userSkillsResponse = await GetSkillsByUser(userId);
                setSkills(userSkillsResponse.data);
            } catch {
                setError("Failed to fetch skills.");
            } finally {
                setLoading(false);
            }
        };

        fetchUserSkills();
    }, []);

    const handleToggleDescription = (projectId: string) => {
        setExpandedDescription((prev) => (prev === projectId ? null : projectId));
    };

    const handleDelete = () => {
        setTimeout(() => {
            window.location.reload();
        }, 6000);
    };

    if (loading) {
        return (
            <p className="text-center text-gray-200">Loading your skills...</p>
        );
    }

    if (error) {
        return <p className="text-center text-red-500">{error}</p>;
    }

    return (
        <>
            <UserProfileHeader />
            <div className="mt-[80px] p-8">
                <h2 className="text-2xl font-bold text-gray-200 mb-6 text-center">
                    My Skills
                </h2>

                {skills.length > 0 ? (
                    <div className="overflow-x-auto">
                        <table className="min-w-full bg-gray-800 border border-gray-700">
                            <thead>
                                <tr className="bg-gray-900 text-gray-300">
                                    <th className="py-3 px-2 text-left">Skills</th>
                                    <th className="py-3 px-2 text-left">Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                {skills.map((skill) => (
                                    <tr key={skill.id} className="border-t border-gray-700">
                                        <td className="p-2 hover:bg-gray-700">
                                            <div className="flex items-center gap-3">
                                                <div>
                                                    <h3
                                                        className="text-base font-semibold text-gray-200 hover:text-blue-500 hover:underline cursor-pointer"
                                                        onClick={() =>
                                                            navigate(
                                                                `/Skill/${encodeURIComponent(skill.id!)}`
                                                            )
                                                        }
                                                    >
                                                        {skill.name}
                                                    </h3>
                                                    <p className="text-gray-400 text-sm">
                                                        {expandedDescription === skill.id
                                                            ? skill.description
                                                            : `${skill.description.substring(0, 120)}...`}
                                                    </p>
                                                    <button
                                                        className="text-blue-500 text-xs mt-2 cursor-pointer"
                                                        onClick={() => handleToggleDescription(skill.id!)}
                                                    >
                                                        {expandedDescription === skill.id
                                                            ? "Show less"
                                                            : "Show more"}
                                                    </button>
                                                </div>
                                            </div>
                                        </td>
                                        <td className="p-2">
                                            <button
                                                onClick={() => setShowDeleteModal(skill.id!)}
                                                className="bg-red-500 text-white px-3 py-1 rounded-md hover:bg-red-600 text-sm cursor-pointer"
                                            >
                                                <svg
                                                    xmlns="http://www.w3.org/2000/svg"
                                                    fill="none"
                                                    viewBox="0 0 24 24"
                                                    strokeWidth="1.5"
                                                    stroke="currentColor"
                                                    className="size-6"
                                                >
                                                    <path
                                                        strokeLinecap="round"
                                                        strokeLinejoin="round"
                                                        d="m14.74 9-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 0 1-2.244 2.077H8.084a2.25 2.25 0 0 1-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 0 0-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 0 1 3.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 0 0-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 0 0-7.5 0"
                                                    />
                                                </svg>
                                            </button>
                                            {showDeleteModal === skill.id && (
                                                <RemoveSkill
                                                    userId={userId!}
                                                    skillId={skill.id!}
                                                    onClose={() => setShowDeleteModal(null)}
                                                    onConfirm={handleDelete}
                                                />
                                            )}
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                ) : (
                    <p className="text-center text-gray-200">You have no skills yet. Please add some skills.</p>
                )}
            </div>
        </>
    );
}
