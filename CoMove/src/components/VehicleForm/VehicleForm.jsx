import { useForm } from '@mantine/form';
import {
    Paper, Grid, Stack, Group,
    Text, Divider, Button,
    TextInput, NumberInput, Textarea,
    Box,
} from '@mantine/core';

function VehicleForm({ initVal, onSubmit, loading }) {
    const vinRegex = /^[A-Z0-9]{17}$/;
    const licenseRegex = /^([A-Z]{4}[0-9]{3}|[A-Z]{3}[0-9]{3})$/;
    const threeColSpans = { base: 12, sm: 4}

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
            ...initVal,
        },
        validate: {
            vin: ((v) => vinRegex.test(v) ? null : 'Érvénytelen VIN (17 nagybetű vagy szám)'),
            licensePlate: ((v) => licenseRegex.test(v) ? null : 'Érvénytelen rendszám (pl. ABC123)'),
            insuranceNumber: ((v) => v.trim() ? null : 'Kötelező megadni'),
            manufacturer: ((v) => v.trim() ? null : 'Kötelező megadni'),
            model: ((v) => v.trim() ? null : 'Kötelező megadni'),
            fuelType: ((v) => v ? null : 'Kötelező megadni'),
            transmission: ((v) => v ? null : 'Kötelező megadni'),
            year: ((v) => v >= 1886 && v <= new Date().getFullYear() ? null : 'Érvénytelen évjárat'),
            horsepower: ((v) => v > 0 ? null : 'Érvénytelen érték'),
            odometerReading: ((v) => v >= 0 ? null : 'Érvénytelen érték'),
            avgFuelConsumption: ((v) => v > 0 ? null : 'Érvénytelen érték'),
        },
    });

    return (
        <Paper shadow="md" radius="md" p={32}>
            <form onSubmit={form.onSubmit(onSubmit)}>
                <Stack gap={25}>
                    <Box>
                        <Text fz={15} fw='bold' c="var(--background)" mb={10}>Alapadatok</Text>
                        <Grid>
                            <Grid.Col span={{ base: 12, sm: 6 }}>
                                <TextInput label="Gyártó" maxLength={16} {...form.getInputProps('manufacturer')} />
                            </Grid.Col>
                            <Grid.Col span={{ base: 12, sm: 6 }}>
                                <TextInput label="Modell" maxLength={32} {...form.getInputProps('model')} />
                            </Grid.Col>
                            <Grid.Col span={threeColSpans}>
                                <NumberInput label="Évjárat" min={1886} max={new Date().getFullYear()} {...form.getInputProps('year')} />
                            </Grid.Col>
                            <Grid.Col span={threeColSpans}>
                                <TextInput label="Üzemanyag" maxLength={20} {...form.getInputProps('fuelType')} />
                            </Grid.Col>
                            <Grid.Col span={threeColSpans}>
                                <TextInput label="Váltó" maxLength={16} {...form.getInputProps('transmission')} />
                            </Grid.Col>
                        </Grid>
                    </Box>

                    <Divider />

                    <Box>
                        <Text fz={15} fw='bold' c="var(--background)" mb={10}>Műszaki adatok</Text>
                        <Grid>
                            <Grid.Col span={threeColSpans}>
                                <NumberInput label="Teljesítmény (LE)" min={1} {...form.getInputProps('horsepower')} />
                            </Grid.Col>
                            <Grid.Col span={threeColSpans}>
                                <NumberInput label="Kilométeróra (km)" min={0} {...form.getInputProps('odometerReading')} />
                            </Grid.Col>
                            <Grid.Col span={threeColSpans}>
                                <NumberInput label="Átlagfogyasztás (L/100km)" min={0.1} decimalScale={1} step={0.1} {...form.getInputProps('avgFuelConsumption')} />
                            </Grid.Col>
                        </Grid>
                    </Box>

                    <Divider />

                    <Box>
                        <Text fz={15} fw='bold' c="var(--background)" mb={10}>Jogi adatok</Text>
                        <Grid>
                            <Grid.Col span={threeColSpans}>
                                <TextInput label="Alvázszám (VIN)" maxLength={17} {...form.getInputProps('vin')} />
                            </Grid.Col>
                            <Grid.Col span={threeColSpans}>
                                <TextInput label="Rendszám" maxLength={7} {...form.getInputProps('licensePlate')} />
                            </Grid.Col>
                            <Grid.Col span={threeColSpans}>
                                <TextInput label="Biztosítási szám" maxLength={64} {...form.getInputProps('insuranceNumber')} />
                            </Grid.Col>
                        </Grid>
                    </Box>

                    <Divider />

                    <Textarea label="Leírás" rows={3} maxLength={512} {...form.getInputProps('description')} />

                    <Group justify="flex-end">
                        <Button
                            type="submit"
                            loading={loading}
                            radius="md"
                            style={{ background: 'var(--button)' }}
                        >
                            Mentés
                        </Button>
                    </Group>
                </Stack>
            </form>
        </Paper>
    );
}

export default VehicleForm;
