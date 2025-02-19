import { DeleteReview } from "../../services/reviewsService";
import { IDeleteReview } from "../../types/review";
import { showToast } from "../../utils/toastHelper";

export default function RemoveReview({
    reviewId,
    onClose,
    onConfirm,
}: IDeleteReview) {
    const handleDelete = () => {
        try {
            DeleteReview(reviewId);
            onConfirm();
            showToast("Review removed successfully!", "success");
            onClose();
        } catch {
            showToast("Review remotion failed!", "error");
            onClose();
        }
    };

    return (
        <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50 z-50">
            <div className="bg-gray-800 rounded-lg p-6 w-full max-w-md">
                <h2 className="text-xl font-semibold mb-4 text-gray-200 text-center">
                    Are you sure you want to delete this review?
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