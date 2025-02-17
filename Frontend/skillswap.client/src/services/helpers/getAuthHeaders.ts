export function GetAuthHeaders() {
    const token = localStorage.getItem("token") || sessionStorage.getItem("token");

    if (!token) return;

    return {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
    };
}