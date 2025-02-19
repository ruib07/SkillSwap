import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { GetUserById } from "../../services/usersService";
import { CreateReview, GetReviewBySessionId } from "../../services/reviewsService";
import { IReview } from "../../types/review";

export default function SessionReviews() {
    const { sessionId } = useParams<{ sessionId: string }>();
    const [reviews, setReviews] = useState<IReview[]>([]);
    const [names, setNames] = useState<{ [key: string]: string }>({});
    const [rating, setRating] = useState(0);
    const [comments, setComments] = useState("");
    const [isMentor, setIsMentor] = useState<boolean | null>(null);
    const [, setError] = useState<string | null>(null);

    const userId = localStorage.getItem("userId") || sessionStorage.getItem("userId");

    useEffect(() => {
        const fetchUserData = async () => {
            if (!userId) return;
            try {
                const userResponse = await GetUserById(userId);
                setIsMentor(userResponse?.data?.isMentor ?? false);
            } catch {
                setError("Failed to fetch user data.");
            }
        };

        fetchUserData();
    }, [userId]);

    useEffect(() => {
        const fetchReviews = async () => {
            try {
                const reviewsResponse = await GetReviewBySessionId(sessionId!);
                setReviews(reviewsResponse.data || []);
            } catch {
                setError("Failed to load comments");
            }
        };

        fetchReviews();
    }, [sessionId]);

    useEffect(() => {
        const fetchUserNames = async () => {
            const newNames: { [key: string]: string } = {};

            for (const review of reviews) {
                try {
                    const userResponse = await GetUserById(review.reviewerId);
                    newNames[review.reviewerId] = userResponse?.data?.name || "Unknown";
                } catch {
                    newNames[review.reviewerId] = "Unknown";
                }
            }

            setNames(newNames);
        };

        if (reviews.length > 0) {
            fetchUserNames();
        }
    }, [reviews]);

    const handleAddReview = async () => {
        if (!userId) return;

        try {
            const newReview: IReview = {
                sessionId: sessionId!,
                reviewerId: userId!,
                rating: rating,
                comments: comments
            };

            await CreateReview(newReview);

            setRating(0);
            setComments("");

            const reviewsResponse = await GetReviewBySessionId(sessionId!);
            setReviews(reviewsResponse.data || []);
        } catch {
            setError("Failed to add review.");
        }
    };

    return (
        <div className="mt-8">
            <h2 className="text-2xl font-semibold text-blue-500 mb-4">Reviews</h2>
            <div className="space-y-4">
                {reviews.map((review, index) => (
                    <div key={index} className="bg-gray-700 p-4 rounded-md">
                        <h4 className="font-bold text-gray-200">
                            {names[review.reviewerId] || "Unknown User"}
                        </h4>
                        <p className="text-gray-300">{review.comments}</p>
                        <small className="text-gray-400">
                            {new Date(review.createdAt!).toLocaleString().slice(0, -3)}
                        </small>
                    </div>
                ))}
            </div>

            {!isMentor && (
                <div className="mt-4">
                    <label className="block mb-2 text-sm font-medium text-gray-300">Rating *</label>
                    <input
                        type="number"
                        id="Rating"
                        name="Rating"
                        className="bg-gray-700 border border-gray-500 text-gray-400 rounded-lg block w-1/12 p-2.5"
                        placeholder="Rating (0 - 5)"
                        min="0"
                        max="5"
                        required
                        value={rating}
                        onChange={(e) => setRating(Number(e.target.value))}
                    />
                    <label className="mt-2 block mb-2 text-sm font-medium text-gray-300">Comment *</label>
                    <textarea
                        className="mt-2 bg-gray-700 border border-gray-500 text-gray-400 rounded-lg block w-3xl p-2.5"
                        rows={4}
                        placeholder="Add a comment..."
                        value={comments}
                        onChange={(e) => setComments(e.target.value)}
                    />
                    <button
                        onClick={handleAddReview}
                        className="bg-blue-500 text-gray-200 py-2 px-4 rounded-md mt-2 hover:bg-blue-600 transition duration-200 cursor-pointer disabled:bg-gray-500 disabled:cursor-not-allowed"
                        disabled={isMentor!}
                    >
                        Add Review
                    </button>
                </div>
            )}

            {isMentor && (
                <p className="text-red-400 mt-4">Mentors are not allowed to add reviews.</p>
            )}
        </div>
    );
}
