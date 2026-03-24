import { useQuery } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import Cards from "../../components/Cards/Cards";
import { API_URL } from "../../assets/scripts/Config";
import { Loader, Center, Stack, Text, Button } from "@mantine/core";
import PageLayout from "../../components/PageLayout/PageLayout";

function MyVehicles() {
    const navigate = useNavigate();

    const auth = JSON.parse(localStorage.getItem("auth"));
    const token = auth?.token;

    const { data: vehicles, isLoading, isError } = useQuery({
        queryKey: ["ownedVehicles"],
        queryFn: async () => {
            const resp = await fetch(`${API_URL}/Vehicle/Owned`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            if (resp.status !== 200) return null;
            return resp.json();
        },
        enabled: !!token,
    });

    return (
        <PageLayout
            title="Járműveim"
            subtitle="Az Ön által bérbeadott járművek"
            heroContent={
                <Button
                    onClick={() => navigate("/vehicles/add")}
                    style={{ background: 'var(--button)' }}
                    radius="md"
                >
                    Jármű hozzáadása
                </Button>
            }
        >
            {isLoading ? (
                <Center pt={80}><Loader color="var(--background)" /></Center>
            ) : isError ? (
                <Center pt={80}><Text c="gray" fz={15}>Hiba történt a járművek betöltésekor.</Text></Center>
            ) : vehicles?.length === 0 ? (
                <Center pt={80}><Text c="gray" fz={15}>Még nincs hozzáadott járműve.</Text></Center>
            ) : (
                <Cards cars={vehicles} />
            )}
        </PageLayout>
    );
}

export default MyVehicles;
