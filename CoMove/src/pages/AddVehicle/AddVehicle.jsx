import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import VehicleForm from '../VehicleForm/VehicleForm';
import { API_URL } from '../../assets/scripts/Config';
import { useUser } from '../../assets/scripts/AuthUser';

function AddVehicle() {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
    const { data: authUser, isLoading: userLoading } = useUser();

    const auth = JSON.parse(localStorage.getItem('auth'));
    const token = auth?.token;

    if (!token || (!userLoading && !authUser)) navigate("/login")

    async function handleSubmit(values) {
        setLoading(true);
        setError(null);
        try {
            const resp = await fetch(`${API_URL}/Vehicle`, {
                method: 'POST',
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
            const vehicle = await resp.json();
            navigate(`/vehicle/${vehicle.id}/edit`);
        } finally {
            setLoading(false);
        }
    }

    return (
        <VehicleForm
            title="Jármű hozzáadása"
            subtitle="Adja meg az új jármű adatait"
            onSubmit={handleSubmit}
            loading={loading}
            error={error}
        />
    );
}

export default AddVehicle;
