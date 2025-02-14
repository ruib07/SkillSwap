import { ISendEmail, IChangePassword } from "../types/authentication";
import apiAuthRequest from "./helpers/apiAuthService";

export const RecoverPasswordSendEmail = async (sendEmail: ISendEmail) => apiAuthRequest("POST", "users/recoverpassword", sendEmail);

export const RecoverPasswordChange = async (changePassword: IChangePassword) => apiAuthRequest("PUT", "users/updatepassword", changePassword);