import axios from "axios";
import { GetAuthHeaders } from "./getAuthHeaders";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

const apiRequest = async (
    method: "GET" | "POST" | "PUT" | "PATCH" | "DELETE",
    endpoint: string,
    data?: any,
    withAuth: boolean = true 
) => {
    try {
        const url = `${API_BASE_URL}/${endpoint}`;
        const headers = withAuth ? GetAuthHeaders() : {}; 

        const response = await axios({
            method,
            url,
            data,
            headers,
        });

        return response;
    } catch {
        throw new Error(`Failed to ${method} ${endpoint}`);
    }
};

export default apiRequest;
