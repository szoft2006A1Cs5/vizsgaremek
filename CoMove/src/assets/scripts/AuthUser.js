import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { API_URL } from "./Config";
import { fetchAPI } from "./Utilities";
import { useNavigate } from "react-router-dom";

export function useUser() {
    return useQuery({
        queryKey: ["authUser"],
        queryFn: async () => {
            const resp = await fetchAPI(`/User`);

            if (!resp.ok) throw new Error("Nincs bejelentkezve!");

            return resp.json();
        },
        refetchInterval: 60000,
        staleTime: 60000,
        refetchOnWindowFocus: true
    })
}

export function useLogout() {
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    return useMutation({
        mutationFn: () => fetchAPI(`/Auth/Logout`, { method: "POST" }),
        onSuccess: () => {
            queryClient.setQueryData(['authUser'], null);
            queryClient.clear();
            navigate("/");
        },
    });
}
