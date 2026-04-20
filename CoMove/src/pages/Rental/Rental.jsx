import { Center, Grid, Loader, Paper, Stack, Text } from "@mantine/core";
import PageLayout from "../../components/common/PageLayout/PageLayout";
import { useNavigate, useParams } from "react-router-dom";
import { useUser } from "../../assets/scripts/AuthUser";
import { useMutation, useQuery } from "@tanstack/react-query";
import { API_URL } from "../../assets/scripts/Config";
import { useEffect } from "react";
import { notifications } from "@mantine/notifications";
import { formatDateTime, formatPrice } from "../../assets/scripts/Utilities";
import VehicleCard from "../../components/common/VehicleCard/VehicleCard";
import RentalData from "../../components/Rental/RentalData/RentalData";
import RentalChat from "../../components/Rental/RentalChat/RentalChat";
import RentalDash from "../../components/Rental/RentalDash/RentalDash";

function Rental() {
    const navigate = useNavigate()
    const { rentalId } = useParams();
    const { data: authUser } = useUser();

    const { data: rental, isLoading, error, isError } = useQuery({
        queryKey: ['rental', rentalId],
        queryFn: async () => {
            const resp = await fetch(`${API_URL}/Rental/${rentalId}`, { 
                credentials: "include" 
            });

            if (resp.status === 403 || resp.status === 404) {
                navigate("/rentals");
                return null;
            } else if (!resp.ok) {
                throw new Error("Nem sikerült lekérni a bérlés adatait!");
            }

            return resp.json();
        },
        refetchInterval: 15000,
    });

    return (<>
        <PageLayout
            title={rental?.vehicle ? `${rental.vehicle.manufacturer} ${rental.vehicle.model} bérlése` : `Bérlés részletei`}
            subtitle={rental ? `${formatDateTime(rental.start)} - ${formatDateTime(rental.end)} - ${formatPrice(rental.rentalPrice)} + (díjak)` : ''}
        >
            { isLoading ? (
                <Center pt={100}><Loader color='var(--background)' /></Center>
            ) : (
                isError ? (
                    <Center pt={100}><Text c='var(--lightpurple)'>{error.message}</Text></Center>
                ) : (
                    <Grid gutter='xl'>
                        <Grid.Col span={{ base: 12, md: 4 }}>
                            <Stack gap={15}>
                                <VehicleCard 
                                    vehicle={rental.vehicle} 
                                    onClick={() => navigate(`/vehicle/${rental.vehicle.id}`)} 
                                />

                                <RentalData rental={rental} />
                            </Stack>
                        </Grid.Col>
                        
                        <Grid.Col span={{ base: 12, md: 8 }}>
                            <Stack gap={15}>
                                <RentalDash rental={rental} />
                                <RentalChat rentalId={rentalId} />
                            </Stack>
                        </Grid.Col>
                    </Grid>
                )
            )}

        </PageLayout>
    </>)
}

export default Rental;