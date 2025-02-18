import { FormEvent, useState } from "react";
import { useNavigate } from "react-router-dom";
import { ISkill } from "../../types/skill";
import { CreateSkill } from "../../services/skillsService";
import { showToast } from "../../utils/toastHelper";
import Img from "/assets/SkillSwap-Logo.png";

export default function SkillCreation() {
    const [name, setName] = useState("");
    const [description, setDescription] = useState("");
    const navigate = useNavigate();

    const handleSkillCreation = async (e: FormEvent) => {
        e.preventDefault();

        const newSkill: ISkill = { name, description };

        try {
            const res = await CreateSkill(newSkill);
            const skillId = res.data.id;
            showToast("Skill created successfully!", "success");
            navigate(`/Skill/${skillId}`);
        } catch {
            showToast("Something went wrong!", "error");
        }
    };

    return (
        <div className="flex min-h-full flex-1 justify-center px-6 py-12 lg:px-8">
            <div className="relative w-full max-w-lg">
                <img alt="SkillSwap" src={Img} className="mx-auto h-12 w-auto" /><br />

                <div className="w-full bg-gray-800 max-w-lg border border-gray-600 rounded-lg shadow">
                    <div className="p-6 space-y-4 md:space-y-6 sm:p-8">
                        <h1 className="text-xl font-bold leading-tight tracking-tight text-gray-300 md:text-2xl text-left">
                            Create Skill
                        </h1>
                        <form className="space-y-4 md:space-y-6" onSubmit={handleSkillCreation}>
                            <div>
                                <label className="block mb-2 text-sm font-medium text-gray-300">Skill Name *</label>
                                <input
                                    type="text"
                                    className="bg-gray-700 border border-gray-500 text-gray-400 rounded-lg block w-full p-2.5"
                                    placeholder="Skill Name"
                                    required
                                    value={name}
                                    onChange={(e) => setName(e.target.value)}
                                />
                            </div>

                            <div>
                                <label className="block mb-2 text-sm font-medium text-gray-300">Description (optional)</label>
                                <textarea
                                    className="bg-gray-700 border border-gray-500 text-gray-400 rounded-lg block w-full p-2.5"
                                    rows={4}
                                    placeholder="Tell more about this skill..."
                                    value={description}
                                    onChange={(e) => setDescription(e.target.value)}
                                />
                            </div>

                            <button
                                type="submit"
                                className="w-full text-white bg-blue-600 hover:bg-blue-700 cursor-pointer focus:ring-4 focus:outline-none focus:ring-purple-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center"
                            >
                                Create Skill
                            </button>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
}
