import { useParams } from "react-router-dom";
import { ISkill } from "../../types/skill";
import { useEffect, useState } from "react";
import { GetSkillById } from "../../services/skillsService";
import Header from "../../layouts/Header";

export default function SkillDetails() {
    const { skillId } = useParams<{ skillId: string }>();
    const [skill, setSkill] = useState<ISkill | null>(null);
    const [, setError] = useState<string | null>(null);

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
            <div className="mt-[100px]">
                <p>Name: {skill.name}</p>
                <p>Description: {skill.description}</p>
            </div>
        </>
    )
}