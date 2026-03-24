import { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { Center, Loader } from '@mantine/core';
import VehicleForm from '../VehicleForm/VehicleForm';
import VehicleImageManager from '../../components/VehicleImageManager/VehicleImageManager';
import VehicleAvailabilityManager from '../../components/VehicleAvailabilityManager/VehicleAvailabilityManager';
import { API_URL } from '../../assets/scripts/Config';
import { useUser } from '../../assets/scripts/AuthUser';

function EditVehicle() {
    const { carId } = useParams();
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
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

    async function handleSubmit(values) {
        setLoading(true);
        setError(null);
        try {
            const resp = await fetch(`${API_URL}/Vehicle/${carId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify(values),
            });
            if (resp.status === 409) {
                setError('A megadott VIN, rendszám vagy biztosítási szám már foglalt.');
                return;
            }
            if (!resp.ok) {
                setError('Hiba történt a jármű mentésekor.');
                return;
            }
            navigate('/vehicles');
        } finally {
            setLoading(false);
        }
    }

    if (isLoading) return <Center pt={80}><Loader color="#192570" /></Center>;
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
            onSubmit={handleSubmit}
            loading={loading}
            error={error}
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
