import { IUser } from "../types/user";
import apiRequest from "./helpers/apiService";

export const GetUsers = async () => apiRequest("GET", "users", undefined, false);

export const GetUserById = async (userId: string) => apiRequest("GET", `users/${userId}`, undefined, false);

export const GetMentors = async () => apiRequest("GET", "users/mentors", undefined, false);

export const UpdateUser = async (newUserData: Partial<IUser>) => {
    const userId = localStorage.getItem("userId") || sessionStorage.getItem("userId");

   return apiRequest("PUT", `users/${userId}`, newUserData);
}

export const UpdateBalance = async (userId: string, balance: number) => apiRequest("PATCH", `users/${userId}/balance`, { balance }, true);

export const DeleteUser = async (userId: string) => apiRequest("DELETE", `users/${userId}`, undefined, true);