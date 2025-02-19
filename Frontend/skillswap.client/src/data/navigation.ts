export const navigation = [
    { name: "Skills", href: "/Skills", requiresAuth: false },
    { name: "Mentors", href: "/Mentors", requiresAuth: false },
    { name: "Mentorship Requests", href: "/MentorshipRequests", requiresAuth: true },
    { name: "Sessions", href: "/Sessions", requiresAuth: true },
];

export const profileNavigation = [
    { name: "My Information", href: "/MyInfo", current: true },
    { name: "My Skills", href: "/MySkills", current: false },
    { name: "My Reviews", href: "/MyReviews", current: false },
    { name: "My Payments", href: "/MyPayments", current: false }
];