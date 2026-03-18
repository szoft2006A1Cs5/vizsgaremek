import { useQuery, useQueryClient } from "@tanstack/react-query";


export function useUser() {
    const auth = JSON.parse(localStorage.getItem("auth"));
    const token = auth?.token;

    return useQuery({
        queryKey: ["authUser"],
        queryFn: async () => {
            const resp = await fetch("https://localhost:7245/api/User", 
            {
                method: "GET",
                headers: {
                    Authorization: `Bearer ${token}`
                }
            });

            if (resp.status !== 200) {
                localStorage.removeItem("auth");
                return null;
            }

            return await resp.json();
        },
        enabled: !!token,
        refetchInterval: token ? 60000 : false,
        staleTime: 60000,
    })
}

export function useLogout() {
    const queryClient = useQueryClient();
    return () => {
        localStorage.removeItem("auth");
        queryClient.setQueryData(['authUser'], null);
        queryClient.clear();
    }
}