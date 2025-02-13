import { ToastContainer } from "react-toastify";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import "react-toastify/dist/ReactToastify.css";

import NotFound from "./pages/404";

export default function App() {
    return (
        <Router>
            <div className="flex flex-col min-h-screen bg-white">
                <ToastContainer />

                <div className="flex-grow container mx-auto">
                    <Routes>
                        <Route path="*" element={<NotFound />} />
                    </Routes>
                </div>
            </div>
        </Router>
  )
}
