import { IMentorshipRequest } from "../types/mentorshipRequest";
import apiRequest from "./helpers/apiService";

export const GetMentorshipRequestsByMentor = async (mentorId: string) =>
    apiRequest("GET", `mentorship-requests/bymentor/${mentorId}`, undefined, true);

export const GetMentorshipRequestsByLearner = async (learnerId: string) =>
    apiRequest("GET", `mentorship-requests/bylearner/${learnerId}`, undefined, true);

export const CreateMentorshipRequest = async (newMentorshipRequest: IMentorshipRequest) =>
    apiRequest("POST", "mentorship-requests", newMentorshipRequest, true);

export const UpdateMentorshipRequest = async (mentorshipRequestId: string, newMentorshipRequestData: Partial<IMentorshipRequest>) => 
    apiRequest("PUT", `mentorship-requests/${mentorshipRequestId}`, newMentorshipRequestData, true);