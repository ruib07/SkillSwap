import { ISendEmail, IChangePassword } from "../types/authentication";
import apiAuthRequest from "./helpers/apiAuthService";

export const RecoverPasswordSendEmail = async (sendEmail: ISendEmail) =>
    await apiAuthRequest("POST", "users/recoverpassword", sendEmail);

export const RecoverPasswordChange = async (changePassword: IChangePassword) =>
    await apiAuthRequest("PUT", "users/updatepassword", changePassword);