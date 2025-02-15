import apiRequest from "./helpers/apiService";

export const GetUserById = async (userId: string) => apiRequest("GET", `users/${userId}`, undefined, false);

export const GetMentors = async () => apiRequest("GET", "users/mentors", undefined, false);