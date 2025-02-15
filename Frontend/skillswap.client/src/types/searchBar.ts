import { ISkill } from "./skill";
import { IUser } from "./user";

export interface ISkillSearchBar {
    skills: ISkill[];
    setFilteredSkills: (skills: ISkill[]) => void;
}

export interface IMentorSearchBar {
    mentors: IUser[];
    setFilteredMentors: (mentors: IUser[]) => void;
}