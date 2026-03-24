import { useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation } from '@tanstack/react-query';
import { notifications } from '@mantine/notifications';
import { Center, Loader } from '@mantine/core';
import VehicleForm from '../VehicleForm/VehicleForm';
import VehicleImageManager from '../../components/VehicleImageManager/VehicleImageManager';
import VehicleAvailabilityManager from '../../components/VehicleAvailabilityManager/VehicleAvailabilityManager';
import { API_URL } from '../../assets/scripts/Config';
import { useUser } from '../../assets/scripts/AuthUser';

function EditVehicle() {
    const { carId } = useParams();
    const navigate = useNavigate();
    const { data: authUser, isLoading: userLoading } = useUser();

    const auth = JSON.parse(localStorage.getItem('auth'));
    const token = auth?.token;

    const { data: vehicle, isLoading } = useQuery({
        queryKey: ['vehicle', carId],
        queryFn: async () => {
            const resp = await fetch(`${API_URL}/Vehicle/${carId}`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!resp.ok) throw new Error('Not found');
            return resp.json();
        },
    });

    const mutation = useMutation({
        mutationFn: async (values) => {
            const resp = await fetch(`${API_URL}/Vehicle/${carId}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
                body: JSON.stringify(values),
            });
            if (resp.status === 409) throw new Error('A megadott VIN, rendszám vagy biztosítási szám már foglalt.');
            if (!resp.ok) throw new Error('Hiba történt a jármű mentésekor.');
        },
        onSuccess: () => navigate('/vehicles'),
        onError: (err) => notifications.show({ title: 'Hiba', message: err.message, color: 'red' }),
    });

    if (isLoading) return <Center pt={80}><Loader color="var(--button)" /></Center>;
    if (!isLoading && !userLoading && authUser.id !== vehicle.ownerId) navigate("/vehicles");

    return (
        <VehicleForm
            title="Jármű szerkesztése"
            subtitle={vehicle ? `${vehicle.manufacturer} ${vehicle.model}` : ''}
            initialValues={vehicle ? {
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
            onSubmit={(values) => mutation.mutate(values)}
            loading={mutation.isPending}
            extra={
                <>
                    <VehicleAvailabilityManager vehicleId={carId} token={token} />
                    <VehicleImageManager vehicleId={carId} token={token} />
                </>
            }
        />
    );
}

export default EditVehicle;
