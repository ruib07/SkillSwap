export interface IUser {
    id?: string;
    name: string;
    email: string;
    password: string;
    bio: string;
    profilePicture: string;
    balance: number;
    isMentor: boolean;
}

export interface IDeleteUser {
    userId: string;
    onClose: () => void;
    onConfirm: () => void;
}