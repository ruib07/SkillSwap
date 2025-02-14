import apiRequest from "./helpers/apiService";

export const GetUserById = async (userId: string) =>
    await apiRequest("GET", `users/${userId}`);