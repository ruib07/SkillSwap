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
import NewMentorshipRequest from "./components/Mentorships/CreateMentorshipRequest";
import MyInformation from "./components/Profile/UserInformation";

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

                        <Route path="/Skills" element={<Skills /> } />
                        <Route path="/Skill/:skillId" element={<SkillDetails /> } />
                        <Route path="/Skills/Create" element={<SkillCreation />} />

                        <Route path="/Mentors" element={<Mentors />} />
                        <Route path="/Mentorship/Create" element={<NewMentorshipRequest />} />

                        <Route path="/MyInfo" element={<MyInformation /> } />
                    </Routes>
                </div>
            </div>
        </Router>
    )
}
