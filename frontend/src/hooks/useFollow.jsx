import { useMutation, useQueryClient } from "@tanstack/react-query";
import toast from "react-hot-toast";

const useFollow = () => {
    const queryClient = useQueryClient();

    const { mutate: follow, isPending } = useMutation({
        mutationFn: async (userId) => {
            try {
                const res = await fetch(`${import.meta.env.VITE_API_URL}/api/users/follow/${userId}`, {
                    method: "POST",
                    credentials: "include"
                });

                console.log("Server response:", res);

                const data = await res.json();
                if (!res.ok) throw new Error(data.error || "Something went wrong");
                return data;
            } catch (error) {
                console.error(error);
                throw error;
            }
        },
        onSuccess: () => {
            Promise.all([
                queryClient.invalidateQueries({ queryKey: ["suggestedUsers"] }),
                queryClient.invalidateQueries({ queryKey: ["authUser"] })
            ])
        },
        onError: () => {
            toast.error(error.message)
        }
    });
    return { follow, isPending };
};

export default useFollow;