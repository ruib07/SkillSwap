import { ToastContainer } from "react-toastify";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import "react-toastify/dist/ReactToastify.css";

import GoToTopPage from "./components/Button/GoToTopPage";
import ScrollToTopButton from "./components/Button/ScrollToTopButton";

import Home from "./pages/Home";
import NotFound from "./pages/404";

import NewRegistration from "./pages/Registration";
import Authentication from "./pages/Login";
import RecoverPasswordEmail from "./components/PasswordRecovery/EmailToRecoverPassword";
import RecoverPasswordUpdate from "./components/PasswordRecovery/ChangePassword";

import Skills from "./pages/Skills";
import SkillDetails from "./components/Skills/SkillDetails";
import SkillCreation from "./components/Skills/CreateSkill";

import Mentors from "./pages/Mentors";
import MentorshipRequests from "./pages/MentorshipRequests";
import NewMentorshipRequest from "./components/Mentorships/CreateMentorshipRequest";

import Sessions from "./pages/Sessions";
import SessionDetails from "./components/Sessions/SessionDetails";
import SessionCreation from "./components/Sessions/CreateSession";

import MyInformation from "./components/Profile/UserInformation";
import UserSkills from "./components/Profile/UserSkills";
import UserReviews from "./components/Profile/UserReviews";
import UserPayments from "./components/Profile/UserPayments";

export default function App() {
    return (
        <Router>
            <div className="flex flex-col min-h-screen bg-gray-900">
                <ToastContainer
                    toastStyle={{
                        backgroundColor: "#1E1E1E",
                        color: "#E0E0E0",
                        border: "1px solid #374151",
                        boxShadow: "0px 4px 10px rgba(0, 0, 0, 0.5)",
                    }}
                />
                <GoToTopPage />
                <ScrollToTopButton />

                <div className="flex-grow container mx-auto">
                    <Routes>
                        <Route path="/" element={<Home />} />
                        <Route path="*" element={<NotFound />} />

                        <Route path="/Authentication/Registration" element={<NewRegistration />} />
                        <Route path="/Authentication/Login" element={<Authentication />} />
                        <Route path="/RecoverPassword/SendEmail" element={<RecoverPasswordEmail />} />
                        <Route path="/RecoverPassword/ChangePassword" element={<RecoverPasswordUpdate />} />

                        <Route path="/Skills" element={<Skills />} />
                        <Route path="/Skill/:skillId" element={<SkillDetails />} />
                        <Route path="/Skills/Create" element={<SkillCreation />} />

                        <Route path="/Mentors" element={<Mentors />} />
                        <Route path="/MentorshipRequests" element={<MentorshipRequests /> } />
                        <Route path="/Mentorship/Create" element={<NewMentorshipRequest />} />

                        <Route path="/Sessions" element={<Sessions />} />
                        <Route path="/Session/:sessionId" element={<SessionDetails /> } />
                        <Route path="/Session/Create/:mentorshipRequestId/:learnerId/:mentorId" element={<SessionCreation /> } />

                        <Route path="/MyInfo" element={<MyInformation />} />
                        <Route path="/MySkills" element={<UserSkills />} />
                        <Route path="/MyReviews" element={<UserReviews />} />
                        <Route path="/MyPayments" element={<UserPayments /> } />
                    </Routes>
                </div>
            </div>
        </Router>
    )
}
