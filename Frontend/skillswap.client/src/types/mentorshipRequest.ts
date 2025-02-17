export interface IMentorshipRequest {
    id?: string;
    mentorId: string;
    learnerId: string;
    skillId: string;
    status: number;
    scheduledAt: string;
}