export interface ILogin {
    email: string;
    password: string;
}

export interface IRegistration {
    name: string;
    email: string;
    password: string;
    bio: string;
    profilePicture: string;
    balance: number;
}

export interface ISendEmail {
    email: string;
}

export interface IChangePassword {
    token: string;
    newPassword: string;
    confirmNewPassword: string;
}