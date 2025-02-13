import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

const apiAuthRequest = async (
    method: "POST" | "PUT",
    endpoint: string,
    data?: any
) => {
    try {
        const url = `${API_BASE_URL}/${endpoint}`;

        const response = await axios({
            method,
            url,
            data,
        });

        return response;
    } catch {
        throw new Error(`Failed to ${method} ${endpoint}`);
    }
};

export default apiAuthRequest;