import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { API_URL } from "./Config";

export function useUser() {
    return useQuery({
        queryKey: ["authUser"],
        queryFn: async () => {
            const resp = await fetch(`${API_URL}/User`, {
                method: "GET",
                credentials: "include",
            });

            if (!resp.ok)
                throw new Error("Nincs bejelentkezve!");

            return await resp.json();
        },
        refetchInterval: 60000,
        staleTime: 60000,
        refetchOnWindowFocus: true
    })
}

export function useLogout() {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: () => fetch(`${API_URL}/Auth/Logout`, { method: "POST", credentials: "include" }),
        onSuccess: () => {
            queryClient.setQueryData(['authUser'], null);
            queryClient.clear();
        },
    });
}
