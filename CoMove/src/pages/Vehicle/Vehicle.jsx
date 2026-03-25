import { useState } from 'react';
import { useNavigate, useParams, useSearchParams } from 'react-router-dom';
import { useQuery, useMutation } from '@tanstack/react-query';
import {
    Badge, Button, Rating, Table, Divider, Alert,
    Paper, Grid, Stack, Group, Center, Loader, Text,
    TextInput, NumberInput,
} from '@mantine/core';
import PageLayout from '../../components/PageLayout/PageLayout';
import { DateTimePicker } from '@mantine/dates';
import { Carousel } from '@mantine/carousel';
import { IconChevronLeft, IconChevronRight, IconCalendar, IconAlertCircle } from '@tabler/icons-react';
import { useUser } from '../../assets/scripts/AuthUser';
import { BACKEND_URL, API_URL } from '../../assets/scripts/Config';
import defaultImage from '../../assets/kepek/egyeb/default.png';
import '@mantine/carousel/styles.css';
import '@mantine/dates/styles.css';
import { notifications } from '@mantine/notifications';
import style from './Vehicle.module.css';

function formatDate(str) {
    if (!str) return '–';
    return new Date(str).toLocaleDateString('hu-HU');
}

function formatStatus(status) {
    const labels = {
        renterOffer:          'Bérlő ajánlata',
        ownerOffer:           'Tulajdonos ajánlata',
        offerAccepted:        'Ajánlat elfogadva',
        renterPickupAccepted: 'Bérlő átvette',
        ownerPickupAccepted:  'Tulaj átadta',
        active:               'Aktív',
        renterFinishAccepted: 'Bérlő visszaadta',
        ownerFinishAccepted:  'Tulaj átvette',
        finished:             'Befejezve',
        renterCancelled:      'Bérlő lemondta',
        ownerCancelled:       'Tulaj lemondta',
    };
    return labels[status] ?? status;
}

function Vehicle() {
    const { carId } = useParams();
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();

    const [start, setStart] = useState(
        searchParams.get('rentalStart') ? new Date(searchParams.get('rentalStart')) : null
    );
    const [end, setEnd] = useState(
        searchParams.get('rentalEnd') ? new Date(searchParams.get('rentalEnd')) : null
    );
    const [pickupLocation, setPickupLocation] = useState('');
    const [fuelLevel, setFuelLevel] = useState(50);

    const auth = JSON.parse(localStorage.getItem('auth'));
    const token = auth?.token;

    const { data: authUser, isSuccess: userSuccess } = useUser();

    const { data: vehicle, isLoading, isError } = useQuery({
        queryKey: ['vehicle', carId],
        queryFn: async () => {
            const resp = await fetch(`${API_URL}/Vehicle/${carId}`, {
                headers: token ? { Authorization: `Bearer ${token}` } : {},
            });
            if (!resp.ok) throw new Error('Not found');
            return resp.json();
        },
    });

    const { data: quote, isFetching: quoteFetching } = useQuery({
        queryKey: ['vehicle-quote', carId, start ? new Date(start).toISOString() : null, end ? new Date(end).toISOString() : null],
        queryFn: async () => {
            const endpoint = new URL(`${API_URL}/Vehicle/${carId}/Quote`);
            endpoint.searchParams.set('rentalStart', new Date(start).toISOString());
            endpoint.searchParams.set('rentalEnd', new Date(end).toISOString());
            const resp = await fetch(endpoint.toString(), {
                headers: token ? { Authorization: `Bearer ${token}` } : {},
            });
            if (resp.status === 409) return null;
            if (!resp.ok) throw new Error('Quote failed');
            return resp.json();
        },
        enabled: !!(start && end),
    });

    const createRentalMutation = useMutation({
        mutationFn: async () => {
            const resp = await fetch(`${API_URL}/Rental`, {
                method: 'POST',
                headers: {
                    Authorization: `Bearer ${token}`,
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    vehicleId: Number(carId),
                    start: new Date(start).toISOString(),
                    end: new Date(end).toISOString(),
                    pickupLocation,
                    fuelLevel,
                }),
            });
            if (!resp.ok) {
                const err = await resp.json().catch(() => ({}));
                throw new Error(err?.message ?? err?.title ?? 'Hiba történt a bérlés létrehozásakor.');
            }
            return resp.json();
        },
        onSuccess: (rental) => {
            navigate(`/rentals/${rental.id}`);
        },
        onError: (err) => {
            notifications.show({
                title: 'Hiba',
                message: err.message,
                color: 'red',
            });
        },
    });

    const canEdit = authUser && vehicle && (authUser.id === vehicle.ownerId || authUser.role === "Administrator");

    return (
        <PageLayout
            title={vehicle ? `${vehicle.manufacturer} ${vehicle.model}` : 'Jármű adatai'}
            subtitle={vehicle ? `${vehicle.year} - ${vehicle.fuelType} - ${vehicle.transmission}` : ''}
        >
                    {isLoading ? (
                        <Center pt={80}><Loader color="var(--background)" /></Center>
                    ) : isError || !vehicle ? (
                        <Center pt={80}><Text c="dimmed" fz={15}>A jármű nem található.</Text></Center>
                    ) : (
                        <Paper shadow="md" radius="md" style={{ overflow: 'hidden' }}>
                            <Grid>
                                <Grid.Col span={{ base: 12, md: 7 }} display='flex'>
                                    <Carousel
                                        height='100%'
                                        flex={1}
                                        withIndicators={1 < vehicle.images?.length}
                                        withControls={1 < vehicle.images?.length}
                                        previousControlIcon={<IconChevronLeft size={18} />}
                                        nextControlIcon={<IconChevronRight size={18} />}
                                        loop
                                    >
                                        {vehicle.images?.length > 0 ? vehicle.images?.map(img => (
                                            <Carousel.Slide key={img.imageId}>
                                                <img
                                                    className={style.vehicleCarouselImg}
                                                    src={`${BACKEND_URL}/${img.path}`}
                                                />
                                            </Carousel.Slide>
                                        )) : (
                                            <Carousel.Slide>
                                                <img className={style.vehicleCarouselImg} src={defaultImage} alt="Nincs kép" />
                                            </Carousel.Slide>
                                        )}
                                    </Carousel>
                                </Grid.Col>

                                <Grid.Col span={{ base: 12, md: 5 }}>
                                    <Stack p={30} gap={15} h="100%">
                                        {vehicle.owner && (
                                            <Text fz={13} c="dimmed">
                                                <Text component="span" fw='bold' c="var(--background)">{vehicle.owner.name}</Text>
                                                {vehicle.owner.addressSettlement ? ` - ${vehicle.owner.addressSettlement}` : ''}
                                            </Text>
                                        )}

                                        {vehicle.rating != null && (
                                            <Group gap={8}>
                                                <Rating value={Number(vehicle.rating) || 0} fractions={10} readOnly size="sm" />
                                                <Text fz={13} c="var(--lightpurple)">{(Number(vehicle.rating) || 0).toFixed(1)}</Text>
                                            </Group>
                                        )}

                                        <Stack gap={6}>
                                            {[
                                                ['Évjárat', vehicle.year],
                                                ['Teljesítmény', `${vehicle.horsepower} LE`],
                                                ['Kilométeróra', `${Number(vehicle.odometerReading).toLocaleString('hu-HU')} km`],
                                                ['Üzemanyag', vehicle.fuelType],
                                                ['Váltó', vehicle.transmission],
                                                ['Fogyasztás', `${vehicle.avgFuelConsumption} L/100km`],
                                            ].map(([label, value]) => (
                                                <Group key={label} justify="space-between">
                                                    <Text fz={13} c="var(--lightpurple)">{label}</Text>
                                                    <Text fz={13} fw='bold' c="var(--background)">{value}</Text>
                                                </Group>
                                            ))}
                                        </Stack>

                                        {vehicle.description?.trim() && (
                                            <Stack gap={6}>
                                                <Text fz={13} fw='bold' c="var(--background)">Leírás</Text>
                                                <Text fz={13} c="var(--lightpurple)">{vehicle.description}</Text>
                                            </Stack>
                                        )}

                                        {canEdit ?
                                            <Button 
                                                fullWidth 
                                                size="md" 
                                                radius="md" 
                                                style={{ background: 'var(--button)' }}
                                                onClick={() => navigate(`/Vehicle/${vehicle.id}/edit`)}
                                            >
                                                Jármű szerkesztése
                                            </Button>
                                        : (
                                            <Stack gap={12} mt="auto">
                                                <DateTimePicker
                                                    label="Bérlés kezdete"
                                                    placeholder="Válassz időpontot"
                                                    value={start}
                                                    onChange={setStart}
                                                    minDate={new Date()}
                                                    leftSection={<IconCalendar size={16} />}
                                                    radius="md"
                                                    size="sm"
                                                    clearable
                                                />
                                                <DateTimePicker
                                                    label="Bérlés vége"
                                                    placeholder="Válassz időpontot"
                                                    value={end}
                                                    onChange={setEnd}
                                                    minDate={start ?? new Date()}
                                                    leftSection={<IconCalendar size={16} />}
                                                    radius="md"
                                                    size="sm"
                                                    clearable
                                                />
                                                {start && end && (
                                                    quoteFetching ? (
                                                        <Center><Loader size="sm" color="var(--background)" /></Center>
                                                    ) : quote ? (
                                                        <>
                                                            <TextInput
                                                                label="Átvételi hely"
                                                                placeholder="pl. Budapest, Kossuth tér 1."
                                                                value={pickupLocation}
                                                                onChange={e => setPickupLocation(e.target.value)}
                                                                radius="md"
                                                                size="sm"
                                                                minLength={1}
                                                                maxLength={512}
                                                            />
                                                            <NumberInput
                                                                label="Üzemanyag szint (%)"
                                                                value={fuelLevel}
                                                                onChange={setFuelLevel}
                                                                min={0}
                                                                max={100}
                                                                radius="md"
                                                                size="sm"
                                                            />
                                                            <Text fz={13} c="dimmed">
                                                                Teljes ár:{' '}
                                                                <Text component="span" fw='bold' c="var(--background)">
                                                                    {quote.fullPrice?.toLocaleString('hu-HU')} Ft
                                                                </Text>
                                                            </Text>
                                                            {!token || (userSuccess && !authUser) ? (
                                                                <Button
                                                                    fullWidth size="md" radius="md"
                                                                    style={{ background: 'var(--button)' }}
                                                                    onClick={() => navigate('/login')}
                                                                >
                                                                    Bejelentkezés szükséges
                                                                </Button>
                                                            ) : (
                                                                <Button
                                                                    fullWidth size="md" radius="md"
                                                                    style={{ background: 'var(--button)' }}
                                                                    loading={createRentalMutation.isPending}
                                                                    onClick={() => createRentalMutation.mutate()}
                                                                >
                                                                    Bérlés megkezdése
                                                                </Button>
                                                            )}
                                                        </>
                                                    ) : (
                                                        <Alert icon={<IconAlertCircle size={16} />} color="red" variant="light" radius="md">
                                                            Erre az időszakra nem érhető el ajánlat.
                                                        </Alert>
                                                    )
                                                )}
                                            </Stack>
                                        )}
                                    </Stack>
                                </Grid.Col>
                            </Grid>

                            {canEdit && (
                                <>
                                    <Divider />
                                    <Stack p={28} gap={16}>
                                        <Group gap={10}>
                                            <Badge variant="filled" style={{ background: 'var(--button)' }}>
                                                Tulajdonos
                                            </Badge>
                                        </Group>

                                        <Stack gap={10}>
                                            {vehicle.vin && (
                                                <Stack gap={4}>
                                                    <Text fz={11} c="var(--lightpurple)">Alvázszám (VIN)</Text>
                                                    <Text fz={13} fw='bold' c="var(--background)" style={{ wordBreak: 'break-all' }}>{vehicle.vin}</Text>
                                                </Stack>
                                            )}
                                            {vehicle.licensePlate && (
                                                <Stack gap={4}>
                                                    <Text fz={11} c="var(--lightpurple)">Rendszám</Text>
                                                    <Text fz={13} fw='bold' c="var(--background)">{vehicle.licensePlate}</Text>
                                                </Stack>
                                            )}
                                            {vehicle.insuranceNumber && (
                                                <Stack gap={4}>
                                                    <Text fz={11} c="var(--lightpurple)">Biztosítási szám</Text>
                                                    <Text fz={13} fw='bold' c="var(--background)">{vehicle.insuranceNumber}</Text>
                                                </Stack>
                                            )}
                                        </Stack>

                                        {vehicle.rentals && vehicle.rentals.length > 0 && (
                                            <Stack gap={10}>
                                                <Text fz={14} fw={700} c="var(--background)">Bérlések</Text>
                                                <Table striped highlightOnHover withTableBorder>
                                                    <Table.Thead>
                                                        <Table.Tr>
                                                            <Table.Th>Bérlő</Table.Th>
                                                            <Table.Th>Kezdés</Table.Th>
                                                            <Table.Th>Vége</Table.Th>
                                                            <Table.Th>Állapot</Table.Th>
                                                        </Table.Tr>
                                                    </Table.Thead>
                                                    <Table.Tbody>
                                                        {vehicle.rentals.map((r) => (
                                                            <Table.Tr key={r.id} style={{ cursor: 'pointer' }} onClick={() => navigate(`/rentals/${r.id}`)}>
                                                                <Table.Td>{r.renter?.name ?? `#${r.renterId}`}</Table.Td>
                                                                <Table.Td>{formatDate(r.start)}</Table.Td>
                                                                <Table.Td>{formatDate(r.end)}</Table.Td>
                                                                <Table.Td>
                                                                    <Badge variant="light" color="blue">{formatStatus(r.status)}</Badge>
                                                                </Table.Td>
                                                            </Table.Tr>
                                                        ))}
                                                    </Table.Tbody>
                                                </Table>
                                            </Stack>
                                        )}
                                    </Stack>
                                </>
                            )}
                        </Paper>
                    )}
        </PageLayout>
    );
}

export default Vehicle;
