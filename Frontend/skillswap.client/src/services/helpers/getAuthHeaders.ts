export function GetAuthHeaders() {
    const token = localStorage.getItem("token");

    if (!token) return;

    return {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
    };
}