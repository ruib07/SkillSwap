import { IMentorshipRequest } from "../types/mentorshipRequest";
import apiRequest from "./helpers/apiService";

export const CreateMentorshipRequest = async (newMentorshipRequest: IMentorshipRequest) =>
    apiRequest("POST", "mentorship-requests", newMentorshipRequest, true);