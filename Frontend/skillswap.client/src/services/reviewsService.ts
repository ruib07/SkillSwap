import { IReview } from "../types/review";
import apiRequest from "./helpers/apiService";

export const GetReviewById = async (reviewId: string) => apiRequest("GET", `reviews/${reviewId}`, undefined, true);

export const GetReviewBySessionId = async (sessionId: string) => apiRequest("GET", `reviews/bysession/${sessionId}`, undefined, true);

export const GetReviewsByReviewerId = async (reviewerId: string) => apiRequest("GET", `reviews/byreviewer/${reviewerId}`, undefined, true);

export const CreateReview = async (newReview: IReview) => apiRequest("POST", `reviews`, newReview, true);

export const DeleteReview = async (reviewId: string) => apiRequest("DELETE", `reviews/${reviewId}`, undefined, true);