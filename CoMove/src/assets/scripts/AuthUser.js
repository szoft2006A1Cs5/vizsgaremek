import { useQuery, useQueryClient } from "@tanstack/react-query";
import { API_URL } from "./Config";

export function useUser() {
    return useQuery({
        queryKey: ["authUser"],
        queryFn: async () => {
            const resp = await fetch(`${API_URL}/User`, {
                method: "GET",
                credentials: "include",
            });

            if (resp.status !== 200) {
                return null;
            }

            return await resp.json();
        },
        refetchInterval: 60000,
        staleTime: 60000,
        refetchOnWindowFocus: true,
    })
}

export function useLogout() {
    const queryClient = useQueryClient();
    return async () => {
        await fetch(`${API_URL}/Auth/Logout`, { method: "POST", credentials: "include" });
        queryClient.setQueryData(['authUser'], null);
        queryClient.clear();
    }
}
