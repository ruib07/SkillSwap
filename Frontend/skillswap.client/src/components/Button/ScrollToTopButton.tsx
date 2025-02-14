import { useState, useEffect } from "react";

export default function ScrollToTopButton() {
    const [windowScrolled, setWindowScrolled] = useState<boolean>(false);

    useEffect(() => {
        const handleScroll = () => {
            setWindowScrolled(window.scrollY > 300);
        };

        window.addEventListener("scroll", handleScroll);

        return () => {
            window.removeEventListener("scroll", handleScroll);
        };
    }, []);

    const scrollToTop = () => {
        window.scrollTo({ top: 0, behavior: "smooth" });
    };

    return (
        <div
            className={`fixed bottom-8 right-8 bg-blue-500 text-white rounded-full w-12 h-12 flex items-center justify-center shadow-lg transition-opacity duration-400 ease-in-out ${windowScrolled ? "opacity-100" : "opacity-0 invisible"
                } sm:block hidden`}
        >
            <button
                onClick={scrollToTop}
                className="flex items-center justify-center w-full h-full bg-transparent border-none cursor-pointer focus:outline-none hover:scale-110"
                aria-label="Scroll to top"
            >
                <span className="text-xl">↑</span>
            </button>
        </div>
    );
}