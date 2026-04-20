import { useQuery } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { API_URL } from "../../assets/scripts/Config";
import { Loader, Center, Stack, Text, Button, SimpleGrid } from "@mantine/core";
import PageLayout from "../../components/common/PageLayout/PageLayout";
import VehicleCard from "../../components/common/VehicleCard/VehicleCard";

function MyVehicles() {
    const navigate = useNavigate();

    const { data: vehicles, isLoading, isError } = useQuery({
        queryKey: ["ownedVehicles"],
        queryFn: async () => {
            const resp = await fetch(`${API_URL}/Vehicle/Owned`, {
                credentials: "include",
            });
            if (resp.status !== 200) return null;
            return resp.json();
        },
    });

    return (
        <PageLayout
            title="Járműveim"
            subtitle="Az Ön által bérbeadott járművek"
            rightContent={
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
                <Center pt={100}><Loader color="var(--background)" /></Center>
            ) : isError ? (
                <Center pt={100}><Text c="var(--lightpurple)" fz={15}>Hiba történt a járművek betöltésekor.</Text></Center>
            ) : vehicles?.length === 0 ? (
                <Center pt={100}><Text c="var(--lightpurple)" fz={15}>Még nincs hozzáadott járműve.</Text></Center>
            ) : (
                <SimpleGrid cols={{
                    base: 1,
                    sm: 2,
                    lg: 4,
                }}>
                    {vehicles.map(vehicle => {
                        return (
                            <VehicleCard key={vehicle.id} vehicle={vehicle} onClick={() => {
                                navigate(`/vehicle/${vehicle.id}`)
                            }} />
                    )
                    })}
                </SimpleGrid>
            )}
        </PageLayout>
    );
}

export default MyVehicles;
