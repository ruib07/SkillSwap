import { IRegistration, ILogin } from '../types/authentication';
import apiAuthRequest from './helpers/apiAuthService';

export const Registration = async (newUser: IRegistration) => apiAuthRequest("POST", "users", newUser);

export const Login = async (login: ILogin) => apiAuthRequest("POST", "auth/login", login);