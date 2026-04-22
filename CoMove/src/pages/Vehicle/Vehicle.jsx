import { useState } from 'react';
import { useNavigate, useParams, useSearchParams } from 'react-router-dom';
import { useQuery, useMutation } from '@tanstack/react-query';
import {
    Button, Rating, Divider, Alert,
    Paper, Grid, Stack, Group, Center, Loader, Text,
    TextInput, NumberInput,
} from '@mantine/core';
import PageLayout from '../../components/common/PageLayout/PageLayout';
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
import RentalRow from '../../components/common/RentalRow/RentalRow';
import 'dayjs/locale/hu'
import { fetchAPI, formatPic } from '../../assets/scripts/Utilities';

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
    const [pickupError, setPickupError] = useState(null);


    const { data: authUser, isSuccess: userSuccess, isLoading: userLoading } = useUser();

    const { data: vehicle, isLoading, isError } = useQuery({
        queryKey: ['vehicle', carId],
        queryFn: async () => {
            const resp = await fetchAPI(`/Vehicle/${carId}`);

            if (!resp.ok) throw new Error('Not found');
            
            return resp.json();
        },
    });

    const { data: quote, isFetching: quoteFetching } = useQuery({
        queryKey: ['vehicle-quote', carId, start ? new Date(start).toISOString() : null, end ? new Date(end).toISOString() : null],
        queryFn: async () => {
            const params = new URLSearchParams({
                rentalStart: new Date(start).toISOString(),
                rentalEnd: new Date(end).toISOString(),
            });

            const resp = await fetchAPI(`/Vehicle/${carId}/Quote?${params}`);

            if (resp.status === 409) return null;
            if (!resp.ok) throw new Error('Nem sikerült lekérni az ajánlatot!');
            return resp.json();
        },
        enabled: !!(start && end),
    });

    const createRentalMutation = useMutation({
        mutationFn: async () => {
            const resp = await fetchAPI(`/Rental`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    vehicleId: Number(carId),
                    start: new Date(start).toISOString(),
                    end: new Date(end).toISOString(),
                    pickupLocation,
                }),
            });

            if (!resp.ok) {
                const err = await resp.json().catch(() => ({}));
                throw new Error(err?.error ?? 'Hiba történt a bérlés létrehozásakor.');
            }

            return resp.json();
        },
        onSuccess: (rental) => {
            navigate(`/rental/${rental.id}`);
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
                        <Center pt={100}><Loader color="var(--background)" /></Center>
                    ) : isError || !vehicle ? (
                        <Center pt={100}><Text c="dimmed" fz={15}>A jármű nem található.</Text></Center>
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
                                                    src={formatPic(img?.path)}
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
                                        <Text fz={15} c="dimmed">
                                            <Text component="span" fw='bold' c="var(--background)">{vehicle.owner.name}</Text>
                                            {vehicle.owner.addressSettlement ? ` - ${vehicle.owner.addressSettlement}` : ''}
                                        </Text>

                                        {vehicle.rating != null ? (
                                            <Group gap={10}>
                                                <Rating value={Number(vehicle.rating) || 0} fractions={10} readOnly size="sm" />
                                                <Text fz={14} c="var(--background)" fw='bold'>{(Number(vehicle.rating) || 0).toFixed(2)}</Text>
                                            </Group>
                                        ) : <></>}

                                        <Stack gap={5}>
                                            <Group justify="space-between">
                                                <Text fz={15} c="var(--lightpurple)">Évjárat</Text>
                                                <Text fz={15} fw='bold' c="var(--background)">{vehicle.year}</Text>
                                            </Group>
                                            <Group justify="space-between">
                                                <Text fz={15} c="var(--lightpurple)">Évjárat</Text>
                                                <Text fz={15} fw='bold' c="var(--background)">{vehicle.year}</Text>
                                            </Group>
                                            <Group justify="space-between">
                                                <Text fz={15} c="var(--lightpurple)">Teljesítmény</Text>
                                                <Text fz={15} fw='bold' c="var(--background)">{vehicle.horsepower}</Text>
                                            </Group>
                                            <Group justify="space-between">
                                                <Text fz={15} c="var(--lightpurple)">Kilométeróra</Text>
                                                <Text fz={15} fw='bold' c="var(--background)">{vehicle.odometerReading}</Text>
                                            </Group>
                                            <Group justify="space-between">
                                                <Text fz={15} c="var(--lightpurple)">Üzemanyag</Text>
                                                <Text fz={15} fw='bold' c="var(--background)">{vehicle.fuelType}</Text>
                                            </Group>
                                            <Group justify="space-between">
                                                <Text fz={15} c="var(--lightpurple)">Váltó</Text>
                                                <Text fz={15} fw='bold' c="var(--background)">{vehicle.transmission}</Text>
                                            </Group>
                                            <Group justify="space-between">
                                                <Text fz={15} c="var(--lightpurple)">Fogyasztás</Text>
                                                <Text fz={15} fw='bold' c="var(--background)">{vehicle.avgFuelConsumption}</Text>
                                            </Group>
                                        </Stack>

                                        {vehicle.description?.trim() && (
                                            <Stack gap={5}>
                                                <Text fz={14} fw='bold' c="var(--background)">Leírás</Text>
                                                <Text fz={14} c="var(--lightpurple)">{vehicle.description}</Text>
                                            </Stack>
                                        )}

                                        {canEdit ?
                                            <Button 
                                                size="md" 
                                                radius="md" 
                                                style={{ background: 'var(--button)' }}
                                                onClick={() => navigate(`/Vehicle/${vehicle.id}/edit`)}
                                            >
                                                Jármű szerkesztése
                                            </Button>
                                        : (
                                            <Stack gap={15} mt="auto">
                                                <DateTimePicker
                                                    label="Bérlés kezdete"
                                                    placeholder="Válassz időpontot"
                                                    value={start}
                                                    onChange={setStart}
                                                    minDate={new Date()}
                                                    leftSection={<IconCalendar size={15} />}
                                                    locale='hu'
                                                    valueFormat='YYYY. MM. DD. HH:mm'
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
                                                    leftSection={<IconCalendar size={15} />}
                                                    locale='hu'
                                                    valueFormat='YYYY. MM. DD. HH:mm'
                                                    radius="md"
                                                    size="sm"
                                                    clearable
                                                />
                                                {start && end && (
                                                    quoteFetching ? (
                                                        <Center><Loader size="sm" color="var(--background)" /></Center>
                                                    ) : quote ? (
                                                        <>
                                                            {userSuccess && authUser && (
                                                                <TextInput
                                                                    label="Átvételi hely"
                                                                    placeholder="pl. Budapest, Kossuth tér 1."
                                                                    value={pickupLocation}
                                                                    onChange={e => setPickupLocation(e.target.value)}
                                                                    radius="md"
                                                                    size="sm"
                                                                    maxLength={512}
                                                                    error={!pickupLocation.trim() ? "Nem adtál meg átvételi helyet!" : null}
                                                                />
                                                            )}
                                                            <Group justify='space-between' wrap='nowrap'>
                                                                <Text fz={15} c="var(--lightpurple)">Teljes ár</Text>
                                                                <Text fw='bold' c='var(--background)'>
                                                                    {quote.fullPrice?.toLocaleString('hu-HU')} Ft
                                                                </Text>
                                                            </Group>
                                                            {userSuccess && authUser ? (
                                                                <Button
                                                                    fullWidth size="md"
                                                                    radius="md"
                                                                    style={{ background: 'var(--button)' }}
                                                                    loading={createRentalMutation.isPending}
                                                                    onClick={() => createRentalMutation.mutate()}
                                                                >
                                                                    Bérlés megkezdése
                                                                </Button>
                                                            ) : (
                                                                <Button
                                                                    size="md"
                                                                    radius="md"
                                                                    style={{ background: 'var(--button)' }}
                                                                    onClick={() => navigate('/login')}
                                                                >
                                                                    Bejelentkezés szükséges
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
                                    <Stack p={30} gap={15}>
                                        <Group justify='space-between' wrap='wrap'>
                                            {vehicle.vin && (
                                                <Stack gap={5}>
                                                    <Text fz={12} c="var(--lightpurple)">Alvázszám (VIN)</Text>
                                                    <Text fz={15} fw='bold' c="var(--background)" style={{ wordBreak: 'break-all' }}>{vehicle.vin}</Text>
                                                </Stack>
                                            )}
                                            {vehicle.licensePlate && (
                                                <Stack gap={5}>
                                                    <Text fz={12} c="var(--lightpurple)">Rendszám</Text>
                                                    <Text fz={15} fw='bold' c="var(--background)">{vehicle.licensePlate}</Text>
                                                </Stack>
                                            )}
                                            {vehicle.insuranceNumber && (
                                                <Stack gap={5}>
                                                    <Text fz={12} c="var(--lightpurple)">Biztosítási szám</Text>
                                                    <Text fz={15} fw='bold' c="var(--background)">{vehicle.insuranceNumber}</Text>
                                                </Stack>
                                            )}
                                        </Group>

                                        {vehicle.rentals && vehicle.rentals.length > 0 && (
                                            <Stack gap={10}>
                                                <Text fz={15} fw='bold' c="var(--background)">Bérlések</Text>
                                                {vehicle.rentals.map((rental, i) => {
                                                    return <RentalRow rental={rental} key={i} />
                                                })}
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
