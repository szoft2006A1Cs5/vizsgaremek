import { useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { notifications } from '@mantine/notifications';
import { Center, Loader, LoadingOverlay, Stack } from '@mantine/core';
import VehicleForm from '../../components/VehicleForm/VehicleForm';
import VehicleImageMenu from '../../components/VehicleForm/VehicleImageMenu/VehicleImageMenu';
import VehicleAvailabilityMenu from '../../components/VehicleForm/VehicleAvailabilityMenu/VehicleAvailabilityMenu';
import { API_URL } from '../../assets/scripts/Config';
import { useUser } from '../../assets/scripts/hooks/AuthUser';
import PageLayout from '../../components/common/PageLayout/PageLayout';
import { fetchAPI } from '../../assets/scripts/Utilities';
import { useEffect } from 'react';

function EditVehicle() {
    const { carId } = useParams();
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    
    const { data: authUser, isSuccess: userSuccess } = useUser();

    const { data: vehicle, isLoading } = useQuery({
        queryKey: ['vehicle', carId],
        queryFn: async () => {
            const resp = await fetchAPI(`/Vehicle/${carId}`);

            if (!resp.ok) throw new Error('Hiba!');
            
            return resp.json();
        },
    });

    const vehicleUpdateMutation = useMutation({
        mutationFn: async (vehicleData) => {
            const resp = await fetchAPI(`/Vehicle/${carId}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(vehicleData),
            });

            if (resp.status === 409) throw new Error('A megadott VIN, rendszám vagy biztosítási szám már foglalt.');
            if (!resp.ok) throw new Error('Hiba történt a jármű mentésekor.');
        },
        onSuccess: () => {
            queryClient.invalidateQueries(["vehicle", carId]);
            notifications.show({ title: 'Sikeres módosítás!', message: "A jármű adatai módosultak.", color: 'green' });
        },
        onError: (err) => notifications.show({ title: 'Hiba', message: err.message, color: 'red' }),
    });
    
    useEffect(() => {
        if (!isLoading && userSuccess && 
            authUser?.id !== vehicle?.ownerId && 
            authUser?.role !== "administrator") 
            navigate("/vehicles");
    }, [isLoading, userSuccess, authUser, vehicle])

    return (
        <>
            <PageLayout 
                title={!isLoading && vehicle ? 
                        `${vehicle.manufacturer} ${vehicle.model} szerkesztése` 
                        : 'Jármű szerkesztése'}
                subtitle='Változtasson járműve adatain!'
            >
                { isLoading ? 
                    <LoadingOverlay visible={true} />
                :
                    <Stack gap={20}>
                        <VehicleForm
                            initVal={vehicle ? {
                                ownerId: vehicle.ownerId ?? null,
                                vin: vehicle.vin ?? '',
                                licensePlate: vehicle.licensePlate ?? '',
                                manufacturer: vehicle.manufacturer ?? '',
                                model: vehicle.model ?? '',
                                year: vehicle.year ?? new Date().getFullYear(),
                                description: vehicle.description ?? '',
                                odometerReading: vehicle.odometerReading ?? 0,
                                horsepower: vehicle.horsepower ?? 0,
                                avgFuelConsumption: vehicle.avgFuelConsumption ?? 0,
                                fuelType: vehicle.fuelType ?? '',
                                insuranceNumber: vehicle.insuranceNumber ?? '',
                                transmission: vehicle.transmission ?? '',
                            } : undefined}
                            onSubmit={(val) => vehicleUpdateMutation.mutate(val)}
                            loading={vehicleUpdateMutation.isPending}
                        />
                        <VehicleAvailabilityMenu vehicleId={carId} />
                        <VehicleImageMenu vehicleId={carId} />
                    </Stack>
                }
            </PageLayout>
        </>
    );
}

export default EditVehicle;
