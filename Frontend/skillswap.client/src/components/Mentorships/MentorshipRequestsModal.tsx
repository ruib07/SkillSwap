import { ModalProps } from "../../types/mentorshipRequest";

export default function MentorshipRequestsModal({ children, onClose }: ModalProps) {
    return (
        <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50 z-50">
            <div className="bg-gray-800 p-6 rounded-lg max-w-md w-full relative p-8">
                <button
                    className="absolute top-2 right-2 text-md"
                    onClick={onClose}
                >
                    ✖
                </button>
                {children}
            </div>
        </div>
    );
}
