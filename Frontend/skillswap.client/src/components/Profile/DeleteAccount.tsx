import { IDeleteUser } from "../../types/user";
import { DeleteUser } from "../../services/usersService";
import { showToast } from "../../utils/toastHelper";
import { useNavigate } from "react-router-dom";

export default function DeleteAccount({ userId, onClose, onConfirm }: IDeleteUser) {
    const navigate = useNavigate();

    const handleDelete = async () => {
        try {
            await DeleteUser(userId);
            localStorage.removeItem("token");
            sessionStorage.removeItem("token");
            localStorage.removeItem("userId");
            sessionStorage.removeItem("userId");

            onConfirm();
            onClose();
            navigate("/");
            window.location.reload();
        } catch {
            showToast("Project remotion failed!", "error");
            onClose();
        }
    };

    return (
        <div className="fixed inset-0 flex items-center justify-center bg-gray-900 bg-opacity-30 z-50">
            <div className="bg-gray-800 rounded-lg p-6 w-full max-w-md">
                <h2 className="text-xl font-semibold mb-4 text-gray-200 text-center">
                    Are you sure that you want to delete your account?
                </h2>
                <div className="flex justify-center space-x-4 mt-6">
                    <button
                        className="bg-red-600 text-white py-2 px-4 rounded-md hover:bg-red-500 transition duration-200 cursor-pointer"
                        onClick={handleDelete}
                    >
                        Yes
                    </button>
                    <button
                        className="bg-gray-300 text-gray-800 py-2 px-4 rounded-md hover:bg-gray-400 transition duration-200 cursor-pointer"
                        onClick={onClose}
                    >
                        Cancel
                    </button>
                </div>
            </div>
        </div>
    );
}