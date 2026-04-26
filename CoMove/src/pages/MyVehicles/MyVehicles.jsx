import { useQuery } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { Loader, Center, Text, Button, SimpleGrid } from "@mantine/core";
import PageLayout from "../../components/common/PageLayout/PageLayout";
import VehicleCard from "../../components/common/VehicleCard/VehicleCard";
import { fetchAPI } from "../../assets/scripts/Utilities";
import { useUser } from "../../assets/scripts/hooks/AuthUser";
import { useEffect } from "react";

function MyVehicles() {
    const navigate = useNavigate();
    const { data: authUser, isSuccess: userSuccess } = useUser();

    useEffect(() => {
        if (userSuccess && authUser?.role === "administrator")
            navigate("/");
    }, [authUser, userSuccess])

    const { data: vehicles, isLoading, isError, error } = useQuery({
        queryKey: ["ownedVehicles"],
        queryFn: async () => {
            const resp = await fetchAPI('/Vehicle/Owned');

            if (!resp.ok) throw new Error("Nem sikerült betölteni a járműveket!");
            
            return resp.json();
        },
    });

    return (
        <PageLayout
            title="Járműveim"
            subtitle="Az általad bérbeadott járművek"
            rightContent={
                <Button
                    onClick={() => navigate("/vehicle/add")}
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
                <Center pt={100}><Text c="var(--lightpurple)" fz={15}>{error.message ?? 'Hiba történt a járművek betöltésekor.'}</Text></Center>
            ) : vehicles?.length === 0 ? (
                <Center pt={100}><Text c="var(--lightpurple)" fz={15}>Még nincs hozzáadott járműved.</Text></Center>
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
