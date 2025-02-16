import { useParams } from "react-router-dom";
import { ISkill } from "../../types/skill";
import { IUserSkill } from "../../types/userSkill";
import { useEffect, useState } from "react";
import { GetSkillById } from "../../services/skillsService";
import { showToast } from "../../utils/toastHelper";
import { AddSkillToUser } from "../../services/userSkillsService";
import Header from "../../layouts/Header";

export default function SkillDetails() {
    const { skillId } = useParams<{ skillId: string }>();
    const [skill, setSkill] = useState<ISkill | null>(null);
    const [, setError] = useState<string | null>(null);

    const userId = localStorage.getItem("userId") || sessionStorage.getItem("userId");

    useEffect(() => {
        const fetchSkill = async () => {
            try {
                const res = await GetSkillById(skillId!);
                setSkill(res.data);
            }
            catch (error) {
                setError(`Error fetching skill: ${error}`);
            }
        };

        fetchSkill();
    }, [skillId]);

    const handleAddSkillToUser = async () => {
        const newSkillToUser: IUserSkill = {
            userId: userId!,
            skillId: skillId!
        }

        try {
            await AddSkillToUser(newSkillToUser);
            showToast("Skill added successfully!", "success");
        } catch {
            showToast("Skill was not added!", "error");
        }
    }

    if (!skill) {
        return (
            <div className="flex items-center justify-center h-screen">
                <h1 className="text-lg text-gray-200">Skill not found.</h1>
            </div>
        );
    }

    return (
        <>
            <Header /><br />
            <div className="mt-24 container mx-auto p-4 md:p-8">
                <div className="max-w-3xl mx-auto bg-gray-800 text-white rounded-xl shadow-lg overflow-hidden">
                    <div className="p-8">
                        <h2 className="text-3xl font-bold text-center mb-4">{skill.name}</h2>
                        <div className="flex justify-center mb-6">
                            <div className="w-16 h-1 bg-blue-600 rounded-full"></div>
                        </div>

                        <p className="text-lg text-gray-400 mb-6">{skill.description}</p>

                        <div className="space-y-4">
                            <div className="flex justify-between items-center">
                                <span className="text-sm text-gray-400">Mentors available to this skill:</span>
                                <span className="text-lg font-semibold text-blue-400">TODO</span>
                            </div>
                        </div>

                        <div className="mt-8 flex justify-center">
                            <button
                                onClick={() => handleAddSkillToUser()}
                                className="px-6 py-2 text-white bg-blue-600 rounded-full hover:bg-blue-700 transition duration-300 cursor-pointer">
                                Add Skill to your stack
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </>
    )
}