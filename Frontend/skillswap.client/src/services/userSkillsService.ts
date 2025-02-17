import { IUserSkill } from "../types/userSkill";
import apiRequest from "./helpers/apiService";

export const GetSkillByUser = async (userId: string) => apiRequest("GET", `user-skills/byuser/${userId}`, undefined, true);

export const UserHasSkill = async (userId: string, skillId: string) => apiRequest("GET", `user-skills/hasskill/${userId}/${skillId}`, undefined, true);

export const AddSkillToUser = async (newUserSkill: IUserSkill) => apiRequest("POST", `user-skills`, newUserSkill, true);