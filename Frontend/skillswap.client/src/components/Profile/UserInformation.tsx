import { ChangeEvent, useEffect, useState } from "react";
import { GetUserById, UpdateUser } from "../../services/usersService";
import { IUser } from "../../types/user";
import UserProfileHeader from "../../layouts/ProfileHeader";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faEye, faEyeSlash } from "@fortawesome/free-solid-svg-icons";
import DeleteAccount from "./DeleteAccount";

export default function MyInformation() {
    const [user, setUser] = useState<IUser | null>(null);
    const [isEditing, setIsEditing] = useState(false);
    const [visible, setVisible] = useState<boolean>(true);
    const [, setError] = useState<string | null>(null);
    const [showDeleteModal, setShowDeleteModal] = useState<string | null>(null);
    const [formData, setFormData] = useState<Partial<IUser>>({
        name: "",
        email: "",
        password: "",
        bio: "",
        profilePicture: "",
        balance: 0
    });

    useEffect(() => {
        const fetchUser = async () => {
            try {
                const userId = localStorage.getItem("userId") || sessionStorage.getItem("userId");
                const response = await GetUserById(userId!);
                setUser(response.data);
                setFormData({
                    name: response.data.name,
                    email: response.data.email,
                    password: "",
                    bio: response.data.bio,
                    profilePicture: response.data.profilePicture,
                    balance: response.data.balance
                });
            } catch {
                setError("Failed to load user information.");
            }
        };

        fetchUser();
    }, []);

    const handleInputChange = (e: ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleTextAreaChange = (e: ChangeEvent<HTMLTextAreaElement>) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleEdit = () => setIsEditing(true);

    const handleCancel = () => {
        setFormData({
            name: user?.name || "",
            email: user?.email || "",
            password: "",
            bio: user?.bio || "",
            profilePicture: user?.profilePicture || "",
            balance: user?.balance || 0,
        });
        setIsEditing(false);
    };

    const handleSave = async () => {
        try {
            await UpdateUser(formData);
            setUser({ ...user!, ...formData });
            setIsEditing(false);
            window.location.reload();
        } catch {
            alert("Failed to update profile.");
        }
    };

    const togglePasswordVisibility = () => {
        setVisible(!visible);
    };

    return (
        <>
            <UserProfileHeader />
            <div className="flex items-start justify-center mt-[120px]">
                <div className="bg-gray-800 shadow-lg rounded-lg p-8 max-w-2xl w-full">
                    <h2 className="text-3xl font-bold text-blue-500 mb-6 text-center">
                        Profile
                    </h2>

                    {user ? (
                        <div className="flex items-center space-x-8">
                            <div className="flex-shrink-0">
                                <img
                                    src={
                                        formData.profilePicture ||
                                        "https://pngimg.com/d/anonymous_mask_PNG28.png"
                                    }
                                    alt="User Avatar"
                                    className="w-32 h-32 rounded-full border-4 border-blue-500"
                                />
                            </div>
                            <div className="flex-1">
                                <div className="grid grid-cols-2 gap-6 mb-6">
                                    <div>
                                        <input
                                            type="text"
                                            name="name"
                                            value={formData.name}
                                            onChange={handleInputChange}
                                            disabled={!isEditing}
                                            className={`w-full mt-1 p-2 border bg-gray-800 text-gray-200 ${isEditing
                                                    ? "border-blue-500 focus:border-blue-600"
                                                    : "border-gray-400"
                                                } rounded-md`}
                                        />
                                    </div>
                                    <div>
                                        <input
                                            type="email"
                                            name="email"
                                            value={formData.email}
                                            onChange={handleInputChange}
                                            disabled={!isEditing}
                                            className={`w-full mt-1 p-2 border bg-gray-800 text-gray-200 ${isEditing
                                                    ? "border-blue-500 focus:border-blue-600"
                                                    : "border-gray-300"
                                                } rounded-md`}
                                        />
                                    </div>
                                </div>

                                <div className="mb-6">
                                    <label className="block text-sm font-semibold text-gray-400">
                                        Password
                                    </label>
                                    <div className="relative">
                                        <input
                                            name="password"
                                            type={visible ? "password" : "text"}
                                            value={formData.password}
                                            onChange={handleInputChange}
                                            disabled={!isEditing}
                                            className={`w-full mt-1 p-2 pr-10 border bg-gray-800 text-gray-200 ${isEditing
                                                    ? "border-blue-500 focus:border-blue-600"
                                                    : "border-gray-300"
                                                } rounded-md`}
                                        />
                                        <span
                                            className="absolute inset-y-0 right-3 flex items-center cursor-pointer text-gray-400"
                                            onClick={togglePasswordVisibility}
                                        >
                                            <FontAwesomeIcon icon={visible ? faEye : faEyeSlash} />
                                        </span>
                                    </div>
                                </div>

                                <div className="mb-6">
                                    <label className="block text-sm font-semibold text-gray-400">
                                        Bio
                                    </label>
                                    <textarea
                                        name="bio"
                                        value={formData.bio}
                                        onChange={handleTextAreaChange}
                                        disabled={!isEditing}
                                        className={`w-full mt-1 p-2 border bg-gray-800 text-gray-200 ${isEditing
                                                ? "border-blue-500 focus:border-blue-600"
                                                : "border-gray-300"
                                            } rounded-md`}
                                        rows={4}
                                        placeholder="Tell us about yourself..."
                                    />
                                </div>

                                <div className="mb-6">
                                    <label className="block text-sm font-semibold text-gray-400">
                                        Profile Picture
                                    </label>
                                    <input
                                        name="profilePicture"
                                        type="text"
                                        value={formData.profilePicture}
                                        onChange={handleInputChange}
                                        disabled={!isEditing}
                                        className={`w-full mt-1 p-2 border bg-gray-800 text-gray-200 ${isEditing
                                                ? "border-blue-500 focus:border-blue-600"
                                                : "border-gray-300"
                                            } rounded-md`}
                                        placeholder="Enter Profile Picture"
                                    />
                                </div>

                                <div>
                                    <input
                                        name="balance"
                                        type="number"
                                        min="0"
                                        step="0.01"
                                        value={formData.balance}
                                        onChange={handleInputChange}
                                        disabled={!isEditing}
                                        className={`w-full mt-1 p-2 border bg-gray-800 text-gray-200 ${isEditing
                                            ? "border-blue-500 focus:border-blue-600"
                                            : "border-gray-400"
                                            } rounded-md`}
                                    />
                                </div>

                                {!isEditing ? (
                                    <button
                                        onClick={handleEdit}
                                        className="flex justify-center mt-4 w-full bg-blue-500 text-gray-200 py-2 rounded-md hover:bg-blue-600 transition cursor-pointer"
                                    >
                                        Edit Information
                                        <svg
                                            xmlns="http://www.w3.org/2000/svg"
                                            fill="none"
                                            viewBox="0 0 24 24"
                                            strokeWidth="1.5"
                                            stroke="currentColor"
                                            className="ms-2 size-6"
                                        >
                                            <path
                                                strokeLinecap="round"
                                                strokeLinejoin="round"
                                                d="m16.862 4.487 1.687-1.688a1.875 1.875 0 1 1 2.652 2.652L10.582 16.07a4.5 4.5 0 0 1-1.897 1.13L6 18l.8-2.685a4.5 4.5 0 0 1 1.13-1.897l8.932-8.931Zm0 0L19.5 7.125M18 14v4.75A2.25 2.25 0 0 1 15.75 21H5.25A2.25 2.25 0 0 1 3 18.75V8.25A2.25 2.25 0 0 1 5.25 6H10"
                                            />
                                        </svg>
                                    </button>
                                ) : (
                                    <div className="mt-4 flex space-x-4">
                                        <button
                                            onClick={handleCancel}
                                                className="flex-1 bg-gray-300 text-gray-900 py-2 rounded-md hover:bg-gray-400 transition cursor-pointer"
                                        >
                                            Cancel
                                        </button>
                                        <button
                                            onClick={handleSave}
                                                className="flex-1 bg-blue-500 text-gray-200 py-2 rounded-md hover:bg-blue-600 transition cursor-pointer"
                                        >
                                            Save
                                        </button>
                                    </div>
                                )}
                                <button
                                    onClick={() => setShowDeleteModal(user.id!)}
                                    className="flex justify-center mt-4 w-full bg-red-500 text-gray-200 py-2 rounded-md hover:bg-red-600 transition cursor-pointer"
                                >
                                    Delete Account
                                    <svg
                                        xmlns="http://www.w3.org/2000/svg"
                                        fill="none"
                                        viewBox="0 0 24 24"
                                        strokeWidth="1.5"
                                        stroke="currentColor"
                                        className="ms-2 size-6"
                                    >
                                        <path
                                            strokeLinecap="round"
                                            strokeLinejoin="round"
                                            d="m14.74 9-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 0 1-2.244 2.077H8.084a2.25 2.25 0 0 1-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 0 0-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 0 1 3.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 0 0-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 0 0-7.5 0"
                                        />
                                    </svg>
                                </button>
                                {showDeleteModal === user.id && (
                                    <DeleteAccount
                                        userId={user.id!}
                                        onClose={() => setShowDeleteModal(null)}
                                        onConfirm={() => {
                                            setUser(null); 
                                            setShowDeleteModal(null);
                                        }}
                                    />

                                )}
                            </div>
                        </div>
                    ) : (
                        <p className="text-center text-gray-600">Loading profile...</p>
                    )}
                </div>
            </div>
        </>
    );
}