import { ISkill } from "../types/skill";
import apiRequest from "./helpers/apiService";

export const GetAllSkills = async () => apiRequest("GET", "skills", undefined, false);

export const GetSkillById = async (skillId: string) => apiRequest("GET", `skills/${skillId}`, undefined, false);

export const CreateSkill = async (newSkill: ISkill) => apiRequest("POST", "skills", newSkill, true);