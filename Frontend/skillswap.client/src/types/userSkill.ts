export interface IUserSkill {
    userId: string;
    skillId: string;
}

export interface IDeleteUserSkill {
    userId: string;
    skillId: string;
    onClose: () => void;
    onConfirm: () => void;
}