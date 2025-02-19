import { ISession } from "../types/session";
import apiRequest from "./helpers/apiService";

export const GetSessionById = async (sessionId: string) => apiRequest("GET", `sessions/${sessionId}`, undefined, true);

export const GetSessionsByMentorshipRequest = async (mentorshipRequestId: string) =>
    apiRequest("GET", `sessions/bymentorshiprequest/${mentorshipRequestId}`, undefined, true);

export const CreateSession = async (newSession: ISession) => apiRequest("POST", "sessions", newSession, true);

export const UpdateSession = async (sessionId: string, newSessionData: Partial<ISession>) =>
    apiRequest("PUT", `sessions/${sessionId}`, newSessionData, true);

export const DeleteSession = async (sessionId: string) => apiRequest("DELETE", `sessions/${sessionId}`, undefined, true);