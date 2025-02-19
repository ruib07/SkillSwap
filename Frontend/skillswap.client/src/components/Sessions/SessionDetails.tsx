import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import Header from "../../layouts/Header";
import { GetSessionById } from "../../services/sessionsService";
import { ISession } from "../../types/session";
import NotFound from "../../pages/404";

export default function SessionDetails() {
    const { sessionId } = useParams<{ sessionId: string }>();
    const [session, setSession] = useState<ISession | null>(null);
    const [, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchSession = async () => {
            try {
                const res = await GetSessionById(sessionId!);
                setSession(res.data);
            }
            catch {
                setError("Error fetching session.");
            }
        };

        fetchSession();
    }, [sessionId]);

    if (!session) {
        return (
            <NotFound />
        );
    }

    return (
        <>
            <Header />
            <div className="mt-24 container mx-auto p-4 md:p-8">
                <div className="max-w-3xl mx-auto bg-gray-800 text-white rounded-xl shadow-lg overflow-hidden">
                    <div className="p-8">
                        <h2 className="text-3xl font-bold text-center mb-4">Session Details</h2>
                        <div className="flex justify-center mb-6">
                            <div className="w-16 h-1 bg-blue-600 rounded-full"></div>
                        </div>

                        <div className="flex">
                            <p className="text-gray-400 text-lg">Mentorship request ID: </p>
                            <p className="ms-2 text-lg text-gray-300 mb-6">{session.mentorshipRequestId}</p>
                        </div>

                        <div className="flex">
                            <p className="text-gray-400 text-lg">Session Time: </p>
                            <p className="ms-2 text-lg text-gray-300 mb-6">{new Date(session.sessionTime).toLocaleString().slice(0, -3)}</p>
                        </div>

                        <div className="flex">
                            <p className="text-gray-400 text-lg">Duration: </p>
                            <p className="ms-2 text-lg text-gray-300 mb-6">{session.duration} minutes</p>
                        </div>

                        <div className="flex">
                            <p className="text-gray-400 text-lg">Video Link: </p>
                            <p className="ms-2 text-lg text-gray-300">{session.videoLink || "none"}</p>
                        </div>
                    </div>
                </div>
            </div>
        </>
    )
}