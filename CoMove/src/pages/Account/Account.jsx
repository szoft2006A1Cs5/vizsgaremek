import { useNavigate, useParams } from "react-router-dom";
import PageLayout from "../../components/common/PageLayout/PageLayout";
import { useUser } from "../../assets/scripts/hooks/AuthUser";
import { useEffect } from "react";
import { useQuery } from "@tanstack/react-query";
import { API_URL } from "../../assets/scripts/Config";
import { Center, Loader, Stack, Text } from "@mantine/core";
import AccountBalance from "../../components/Account/AccountBalance";
import AccountEdit from "../../components/Account/AccountEdit";
import { fetchAPI } from "../../assets/scripts/Utilities";

function Account() {
    const navigate = useNavigate();
    const params = useParams();
    const { data: authUser, isSuccess: authUserSuccess, isLoading: authUserLoading } = useUser();
    const userId = params.userId ? params.userId : authUser?.id;

    const { data: user, error, isError, isLoading } = useQuery({
        queryKey: ["user", String(userId)],
        queryFn: async () => {
            const resp = await fetchAPI(`/User/${userId}`);

            if (!resp.ok) throw new Error("Nem sikerült lekérni a felhasználói adatokat!");

            return resp.json();
        },
        enabled: authUserSuccess,
        initialData: userId == authUser?.id ? authUser : undefined, 
    });

    useEffect(() => {
        if (authUserSuccess && userId != authUser?.id &&
            authUser?.role !== "administrator")
            navigate("/")
    }, [userId, authUser])

    return (
        <PageLayout 
            title="Fiókbeállítások"
            subtitle="Változtasson adatain!"
        >
            { (isLoading || authUserLoading) ?
                <Center pt={100}><Loader color="var(--background)" /></Center>
             :
                (
                    isError 
                    ? <Center pt={100}><Text c='var(--lightpurple'>{error.message}</Text></Center>
                    : (
                        <Stack gap={15}>
                            { authUser && authUser.role !== "administrator" && (<AccountBalance user={user} />) }
                            <AccountEdit user={user} />
                        </Stack>
                    )
                )
            }
        </PageLayout>
    )
}

export default Account;