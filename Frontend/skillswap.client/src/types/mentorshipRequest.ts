import { ReactNode } from "react";

export enum MentorshipRequestsStatus {
    Pending = 0,
    Accepted = 1,
    Completed = 2,
    Cancelled = 3
}

export const mentorshipStatusMap: { [key: number]: string } = {
    [MentorshipRequestsStatus.Pending]: "Pending",
    [MentorshipRequestsStatus.Accepted]: "Accepted",
    [MentorshipRequestsStatus.Completed]: "Completed",
    [MentorshipRequestsStatus.Cancelled]: "Cancelled"
};

export interface IMentorshipRequest {
    id?: string;
    mentorId: string;
    learnerId: string;
    skillId: string;
    status: number;
    scheduledAt: string;
}

export interface ModalProps {
    children: ReactNode;
    onClose: () => void;
}