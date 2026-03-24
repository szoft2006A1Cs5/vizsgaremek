import { useForm } from '@mantine/form';
import {
    Paper, Grid, Stack, Group,
    Text, Divider, Button, Alert,
    TextInput, NumberInput, Select, Textarea,
} from '@mantine/core';
import { IconAlertCircle } from '@tabler/icons-react';
import PageLayout from '../../components/PageLayout/PageLayout';
import { useNavigate } from 'react-router-dom';
import { useUser } from '../../assets/scripts/AuthUser';
import { useEffect } from 'react';

const FUEL_TYPES = ['Benzin', 'Dízel', 'Elektromos', 'Hibrid', 'LPG'];
const TRANSMISSIONS = ['Manuális', 'Automata'];

function VehicleForm({ title, subtitle, initialValues, onSubmit, loading, error, extra }) {
    const vinRegex = /^[A-Z0-9]{17}$/;
    const licenseRegex = /^([A-Z]{4}[0-9]{3}|[A-Z]{3}[0-9]{3})$/;

    const navigate = useNavigate()
    const { data: authUser, isLoading: userLoading } = useUser();

    useEffect(() => {
        if (!userLoading && (!!initialValues?.ownerId ? initialValues?.ownerId != authUser.id && authUser.role != "Administrator" : false)) 
            navigate("/vehicles");
    }, [authUser, userLoading, initialValues])

    const form = useForm({
        initialValues: {
            vin: '',
            licensePlate: '',
            manufacturer: '',
            model: '',
            year: new Date().getFullYear(),
            description: '',
            odometerReading: 0,
            horsepower: 0,
            avgFuelConsumption: 0,
            fuelType: '',
            insuranceNumber: '',
            transmission: '',
            ...initialValues,
        },
        validate: {
            vin: ((v) => vinRegex.test(v) ? null : 'Érvénytelen VIN (17 nagybetű vagy szám)'),
            licensePlate: ((v) => licenseRegex.test(v) ? null : 'Érvénytelen rendszám (pl. ABC123)'),
            insuranceNumber: ((v) => v.trim() ? null : 'Kötelező megadni'),
            manufacturer: ((v) => v.trim() ? null : 'Kötelező megadni'),
            model: ((v) => v.trim() ? null : 'Kötelező megadni'),
            fuelType: ((v) => v ? null : 'Kötelező megadni'),
            transmission: ((v) => v ? null : 'Kötelező megadni'),
            year: ((v) => v >= 1900 && v <= new Date().getFullYear() ? null : 'Érvénytelen évjárat'),
            horsepower: ((v) => v > 0 ? null : 'Pozitív szám szükséges'),
            odometerReading: ((v) => v >= 0 ? null : 'Érvénytelen érték'),
            avgFuelConsumption: ((v) => v > 0 ? null : 'Pozitív szám szükséges'),
        },
    });

    return (
        <PageLayout title={title} subtitle={subtitle}>
            <Stack gap={24}>
                    <Paper shadow="md" radius="md" p={32}>
                        <form onSubmit={form.onSubmit(onSubmit)}>
                            <Stack gap={24}>
                                <div>
                                    <Text fz={13} fw={700} c="#060631" mb={12}>Alapadatok</Text>
                                    <Grid>
                                        <Grid.Col span={{ base: 12, sm: 6 }}>
                                            <TextInput label="Gyártó" {...form.getInputProps('manufacturer')} />
                                        </Grid.Col>
                                        <Grid.Col span={{ base: 12, sm: 6 }}>
                                            <TextInput label="Modell" {...form.getInputProps('model')} />
                                        </Grid.Col>
                                        <Grid.Col span={{ base: 12, sm: 4 }}>
                                            <NumberInput label="Évjárat" min={1900} max={new Date().getFullYear()} {...form.getInputProps('year')} />
                                        </Grid.Col>
                                        <Grid.Col span={{ base: 12, sm: 4 }}>
                                            <Select label="Üzemanyag" data={FUEL_TYPES} {...form.getInputProps('fuelType')} />
                                        </Grid.Col>
                                        <Grid.Col span={{ base: 12, sm: 4 }}>
                                            <Select label="Váltó" data={TRANSMISSIONS} {...form.getInputProps('transmission')} />
                                        </Grid.Col>
                                    </Grid>
                                </div>

                                <Divider />

                                <div>
                                    <Text fz={13} fw={700} c="#060631" mb={12}>Műszaki adatok</Text>
                                    <Grid>
                                        <Grid.Col span={{ base: 12, sm: 4 }}>
                                            <NumberInput label="Teljesítmény (LE)" min={1} {...form.getInputProps('horsepower')} />
                                        </Grid.Col>
                                        <Grid.Col span={{ base: 12, sm: 4 }}>
                                            <NumberInput label="Kilométeróra (km)" min={0} {...form.getInputProps('odometerReading')} />
                                        </Grid.Col>
                                        <Grid.Col span={{ base: 12, sm: 4 }}>
                                            <NumberInput label="Átlagfogyasztás (L/100km)" min={0.1} decimalScale={1} step={0.1} {...form.getInputProps('avgFuelConsumption')} />
                                        </Grid.Col>
                                    </Grid>
                                </div>

                                <Divider />

                                <div>
                                    <Text fz={13} fw={700} c="#060631" mb={12}>Jogi adatok</Text>
                                    <Grid>
                                        <Grid.Col span={{ base: 12, sm: 4 }}>
                                            <TextInput label="Alvázszám (VIN)" {...form.getInputProps('vin')} />
                                        </Grid.Col>
                                        <Grid.Col span={{ base: 12, sm: 4 }}>
                                            <TextInput label="Rendszám" {...form.getInputProps('licensePlate')} />
                                        </Grid.Col>
                                        <Grid.Col span={{ base: 12, sm: 4 }}>
                                            <TextInput label="Biztosítási szám" {...form.getInputProps('insuranceNumber')} />
                                        </Grid.Col>
                                    </Grid>
                                </div>

                                <Divider />

                                <Textarea label="Leírás" rows={4} {...form.getInputProps('description')} />

                                {error && (
                                    <Alert icon={<IconAlertCircle size={16} />} color="red" variant="light" radius="md">
                                        {error}
                                    </Alert>
                                )}

                                <Group justify="flex-end">
                                    <Button
                                        type="submit"
                                        loading={loading}
                                        radius="md"
                                        style={{ background: 'linear-gradient(135deg, #192570, #0b1f66)' }}
                                    >
                                        Mentés
                                    </Button>
                                </Group>
                            </Stack>
                        </form>
                    </Paper>
                    {extra}
            </Stack>
        </PageLayout>
    );
}

export default VehicleForm;
