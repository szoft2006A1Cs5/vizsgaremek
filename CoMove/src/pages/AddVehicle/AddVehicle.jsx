import { useNavigate } from 'react-router-dom';
import { useMutation } from '@tanstack/react-query';
import { notifications } from '@mantine/notifications';
import VehicleForm from '../../components/VehicleForm/VehicleForm';
import { API_URL } from '../../assets/scripts/Config';
import { useUser } from '../../assets/scripts/AuthUser';
import PageLayout from '../../components/PageLayout/PageLayout';

function AddVehicle() {
    const navigate = useNavigate();
    const { data: authUser, isLoading: userLoading } = useUser();

    if (!userLoading && !authUser) navigate("/login")

    const vehicleAddMutation = useMutation({
        mutationFn: async (values) => {
            const resp = await fetch(`${API_URL}/Vehicle`, {
                method: 'POST',
                credentials: "include",
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(values),
            });
            if (resp.status === 409) throw new Error('A megadott VIN, rendszám vagy biztosítási szám már foglalt.');
            if (!resp.ok) throw new Error('Hiba történt a jármű mentésekor.');
            return resp.json();
        },
        onSuccess: (vehicle) => navigate(`/vehicle/${vehicle.id}/edit`),
        onError: (err) => notifications.show({ title: 'Hiba', message: err.message, color: 'red' }),
    });

    return (
            <PageLayout 
                title='Új jármű hozzáadása'
                subtitle='Adja meg a jármű adatait!'
            >
                <VehicleForm
                    onSubmit={(val) => vehicleAddMutation.mutate(val)}
                    loading={vehicleAddMutation.isPending}
                />
            </PageLayout>
    );
}

export default AddVehicle;
