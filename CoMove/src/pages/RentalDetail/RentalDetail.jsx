import { useState, useRef, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
    Text, Stack, Group, Paper, Loader, Center, Badge, Button, Divider,
    TextInput, NumberInput, Rating, ScrollArea, ActionIcon, Box,
    Grid, Textarea,
} from '@mantine/core';
import { notifications } from '@mantine/notifications';
import { DateTimePicker } from '@mantine/dates';
import {
    IconArrowLeft, IconCalendar, IconMapPin, IconGauge, IconCoin,
    IconSend, IconCheck, IconX, IconCar, IconUser,
} from '@tabler/icons-react';
import PageLayout from '../../components/PageLayout/PageLayout';
import CarCard from '../../components/CarCard/CarCard';
import { useUser } from '../../assets/scripts/AuthUser';
import { API_URL } from '../../assets/scripts/Config';
import '@mantine/dates/styles.css';
import './RentalDetail.css';

const STATUS = {
    renterOffer:          { label: 'Bérlő ajánlata',             mantine: 'blue',   step: 0 },
    ownerOffer:           { label: 'Tulajdonos ajánlata',         mantine: 'cyan',   step: 0 },
    offerAccepted:        { label: 'Ajánlat elfogadva',           mantine: 'teal',   step: 1 },
    renterPickupAccepted: { label: 'Átvétel: bérlő visszaigazolt', mantine: 'indigo', step: 2 },
    ownerPickupAccepted:  { label: 'Átvétel: tulaj visszaigazolt', mantine: 'indigo', step: 2 },
    active:               { label: 'Aktív bérlés',                mantine: 'green',  step: 3 },
    renterFinishAccepted: { label: 'Visszaadás: bérlő kész',      mantine: 'yellow', step: 4 },
    ownerFinishAccepted:  { label: 'Visszaadás: tulaj kész',      mantine: 'yellow', step: 4 },
    finished:             { label: 'Befejezve',                   mantine: 'gray',   step: 5 },
    renterCancelled:      { label: 'Bérlő által lemondva',        mantine: 'red',    step: -1 },
    ownerCancelled:       { label: 'Tulajdonos által lemondva',   mantine: 'red',    step: -1 },
};

const CANCELLED = ['renterCancelled', 'ownerCancelled'];

function fmt(str) {
    if (!str) return '–';
    return new Date(str).toLocaleString('hu-HU', {
        year: 'numeric', month: '2-digit', day: '2-digit',
        hour: '2-digit', minute: '2-digit',
    });
}

function fmtPrice(n) {
    if (n == null) return '–';
    return Number(n).toLocaleString('hu-HU') + ' Ft';
}

function buildBody(rental, overrides) {
    return {
        start:           rental.start,
        end:             rental.end,
        pickupLocation:  rental.pickupLocation ?? '',
        fuelLevel:       rental.fuelLevel,
        vehicleId:       rental.vehicleId,
        ...overrides,
    };
}

function Chat({ rentalId, token, authUser, disabled }) {
    const [msg, setMsg] = useState('');
    const qc = useQueryClient();
    const viewportRef = useRef(null);

    const { data: messages = [], isLoading } = useQuery({
        queryKey: ['rental-messages', rentalId],
        queryFn: async () => {
            const resp = await fetch(`${API_URL}/Rental/${rentalId}/Message?limit=100`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!resp.ok) return [];
            const data = await resp.json();
            const arr = Array.isArray(data) ? data : (data.items ?? []);
            return [...arr].reverse();
        },
        refetchInterval: 8000,
    });

    const sendMutation = useMutation({
        mutationFn: async (content) => {
            const resp = await fetch(`${API_URL}/Rental/${rentalId}/Message`, {
                method: 'POST',
                headers: { Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' },
                body: JSON.stringify({ content }),
            });
            if (!resp.ok) throw new Error();
        },
        onSuccess: () => {
            setMsg('');
            qc.invalidateQueries({ queryKey: ['rental-messages', rentalId] });
        },
    });

    useEffect(() => {
        if (viewportRef.current)
            viewportRef.current.scrollTo({ top: viewportRef.current.scrollHeight, behavior: 'smooth' });
    }, [messages.length]);

    return (
        <Stack gap={12}>
            <ScrollArea viewportRef={viewportRef} h={320} className="rd-chat-area" type="auto">
                {isLoading ? (
                    <Center h={80}><Loader size="xs" color="var(--button)" /></Center>
                ) : messages.length === 0 ? (
                    <Center h={80}><Text fz={12} c="dimmed">Még nincs üzenet.</Text></Center>
                ) : (
                    <Stack gap={10} p={16}>
                        {messages.map((m, i) => {
                            const own = (m.sender?.id ?? m.senderId) === authUser?.id;
                            return (
                                <Group key={m.id ?? i} justify={own ? 'flex-end' : 'flex-start'} align="flex-end" gap={8}>
                                    {!own && (
                                        <Stack gap={2} align="flex-start" style={{ maxWidth: '72%' }}>
                                            <Text fz={10} c="dimmed" ml={4}>{m.sender?.name ?? '–'}</Text>
                                            <Box className="rd-bubble rd-bubble-other">
                                                <Text fz={13} lh={1.5}>{m.content}</Text>
                                                <Text fz={10} c="dimmed" ta="right" mt={3}>{fmt(m.timeSent)}</Text>
                                            </Box>
                                        </Stack>
                                    )}
                                    {own && (
                                        <Box className="rd-bubble rd-bubble-own" style={{ maxWidth: '72%' }}>
                                            <Text fz={13} lh={1.5} c="white">{m.content}</Text>
                                            <Text fz={10} c="rgba(255,255,255,0.6)" ta="right" mt={3}>{fmt(m.timeSent)}</Text>
                                        </Box>
                                    )}
                                </Group>
                            );
                        })}
                    </Stack>
                )}
            </ScrollArea>

            {!disabled && (
                <Group gap={8}>
                    <TextInput
                        flex={1}
                        placeholder="Írj üzenetet…"
                        value={msg}
                        onChange={e => setMsg(e.target.value)}
                        onKeyDown={e => {
                            if (e.key === 'Enter' && !e.shiftKey && msg.trim()) {
                                e.preventDefault();
                                sendMutation.mutate(msg.trim());
                            }
                        }}
                        radius="xl"
                        size="sm"
                    />
                    <ActionIcon
                        size={36} radius="xl"
                        style={{ background: msg.trim() ? 'var(--button)' : '#e9ecef', flexShrink: 0 }}
                        onClick={() => msg.trim() && sendMutation.mutate(msg.trim())}
                        loading={sendMutation.isPending}
                        disabled={!msg.trim()}
                    >
                        <IconSend size={15} color={msg.trim() ? 'white' : '#adb5bd'} />
                    </ActionIcon>
                </Group>
            )}
        </Stack>
    );
}

// ─── Offer stage panel ────────────────────────────────────────────────────────

function OfferPanel({ rental, isOwner, token, authUser }) {
    const qc = useQueryClient();
    const navigate = useNavigate();

    // Is it MY turn to respond?
    const myTurn = (isOwner && rental.status === 'renterOffer') ||
                   (!isOwner && rental.status === 'ownerOffer');

    const [start, setStart]               = useState(new Date(rental.start));
    const [end, setEnd]                   = useState(new Date(rental.end));
    const [pickupLocation, setPickup]     = useState(rental.pickupLocation ?? '');
    const [fuelLevel, setFuelLevel]       = useState(rental.fuelLevel ?? 50);

    const counterOfferStatus = isOwner ? 'ownerOffer' : 'renterOffer';

    const counterMutation = useMutation({
        mutationFn: async () => {
            const resp = await fetch(`${API_URL}/Rental/${rental.id}`, {
                method: 'PUT',
                headers: { Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' },
                body: JSON.stringify(buildBody(rental, {
                    start: new Date(start).toISOString(),
                    end:   new Date(end).toISOString(),
                    pickupLocation,
                    fuelLevel,
                    status: counterOfferStatus,
                })),
            });
            if (!resp.ok) throw new Error('Ellenjavaslat sikertelen');
        },
        onSuccess: () => qc.invalidateQueries({ queryKey: ['rental', rental.id] }),
        onError: (err) => notifications.show({ title: 'Hiba', message: err.message, color: 'red' }),
    });

    const acceptMutation = useMutation({
        mutationFn: async () => {
            const resp = await fetch(`${API_URL}/Rental/${rental.id}`, {
                method: 'PUT',
                headers: { Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' },
                body: JSON.stringify(buildBody(rental, { status: 'offerAccepted' })),
            });
            if (!resp.ok) throw new Error('Elfogadás sikertelen');
        },
        onSuccess: () => qc.invalidateQueries({ queryKey: ['rental', rental.id] }),
        onError: (err) => notifications.show({ title: 'Hiba', message: err.message, color: 'red' }),
    });

    const cancelMutation = useMutation({
        mutationFn: async () => {
            const resp = await fetch(`${API_URL}/Rental/${rental.id}`, {
                method: 'DELETE',
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!resp.ok) throw new Error('Lemondás sikertelen');
        },
        onSuccess: () => navigate('/rentals'),
        onError: (err) => notifications.show({ title: 'Hiba', message: err.message, color: 'red' }),
    });

    const busy = counterMutation.isPending || acceptMutation.isPending || cancelMutation.isPending;

    return (
        <Stack gap={16}>
            {myTurn ? (
                <>
                    <Text fz={13} c="dimmed">
                        {isOwner
                            ? 'A bérlő ajánlatot küldött. Elfogadhatod, vagy ellenjavaslatot tehetsz.'
                            : 'A tulajdonos ellenjavaslatot tett. Elfogadhatod, vagy módosíthatod az ajánlatot.'}
                    </Text>

                    <DateTimePicker
                        label="Bérlés kezdete"
                        value={start}
                        onChange={setStart}
                        minDate={new Date()}
                        leftSection={<IconCalendar size={15} />}
                        radius="md" size="sm" clearable
                    />
                    <DateTimePicker
                        label="Bérlés vége"
                        value={end}
                        onChange={setEnd}
                        minDate={start ?? new Date()}
                        leftSection={<IconCalendar size={15} />}
                        radius="md" size="sm" clearable
                    />
                    <TextInput
                        label="Átvételi hely"
                        value={pickupLocation}
                        onChange={e => setPickup(e.target.value)}
                        leftSection={<IconMapPin size={15} />}
                        radius="md" size="sm" maxLength={512}
                    />
                    <NumberInput
                        label="Üzemanyag szint (%)"
                        value={fuelLevel}
                        onChange={setFuelLevel}
                        leftSection={<IconGauge size={15} />}
                        min={0} max={100}
                        radius="md" size="sm"
                    />


                    <Group gap={8} wrap="wrap">
                        <Button
                            size="sm" radius="md"
                            style={{ background: 'var(--button)' }}
                            leftSection={<IconCheck size={14} />}
                            loading={acceptMutation.isPending} disabled={busy}
                            onClick={() => acceptMutation.mutate()}
                        >
                            Ajánlat elfogadása
                        </Button>
                        <Button
                            size="sm" radius="md" variant="light"
                            loading={counterMutation.isPending} disabled={busy || !start || !end}
                            onClick={() => counterMutation.mutate()}
                        >
                            Ellenjavaslat küldése
                        </Button>
                        <Button
                            size="sm" radius="md" variant="light" color="red"
                            leftSection={<IconX size={14} />}
                            loading={cancelMutation.isPending} disabled={busy}
                            onClick={() => cancelMutation.mutate()}
                        >
                            Lemondás
                        </Button>
                    </Group>
                </>
            ) : (
                <>
                    <Text fz={13} c="dimmed">
                        {isOwner
                            ? 'Elküldted az ellenjavaslatodat. Várakozás a bérlőre…'
                            : 'Az ajánlatod el lett küldve. Várakozás a tulajdonosra…'}
                    </Text>

                    <Stack gap={6}>
                        {[
                            ['Kezdés',        fmt(rental.start),        IconCalendar],
                            ['Vége',          fmt(rental.end),          IconCalendar],
                            ['Átvételi hely', rental.pickupLocation,    IconMapPin],
                            ['Üzemanyag',     rental.fuelLevel != null ? `${rental.fuelLevel}%` : '–', IconGauge],
                        ].map(([lbl, val, Icon]) => (
                            <Group key={lbl} gap={8} align="flex-start">
                                <Icon size={14} color="var(--lightpurple)" style={{ marginTop: 2 }} />
                                <div>
                                    <Text fz={11} c="var(--lightpurple)">{lbl}</Text>
                                    <Text fz={13} fw={600} c="var(--background)">{val ?? '–'}</Text>
                                </div>
                            </Group>
                        ))}
                    </Stack>

                    <Paper radius="md" p="sm" className="rd-price-box">
                        <Group justify="space-between">
                            <Text fz={13} fw={700} c="var(--background)">Teljes összeg</Text>
                            <Text fz={13} fw={700} c="var(--background)">{fmtPrice(rental.fullPrice)}</Text>
                        </Group>
                    </Paper>

                    <Button
                        size="sm" radius="md" variant="light" color="red"
                        leftSection={<IconX size={14} />}
                        loading={cancelMutation.isPending}
                        onClick={() => cancelMutation.mutate()}
                        style={{ alignSelf: 'flex-start' }}
                    >
                        Lemondás
                    </Button>
                </>
            )}
        </Stack>
    );
}

function PickupPanel({ rental, isOwner, token }) {
    const qc = useQueryClient();

    const iConfirmed = isOwner
        ? rental.status === 'ownerPickupAccepted' || rental.status === 'active'
        : rental.status === 'renterPickupAccepted' || rental.status === 'active';

    const canConfirm = isOwner
        ? rental.status === 'offerAccepted' || rental.status === 'renterPickupAccepted'
        : rental.status === 'offerAccepted' || rental.status === 'ownerPickupAccepted';

    const newStatus = 'active';

    const confirmMutation = useMutation({
        mutationFn: async () => {
            const resp = await fetch(`${API_URL}/Rental/${rental.id}`, {
                method: 'PUT',
                headers: { Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' },
                body: JSON.stringify(buildBody(rental, { status: newStatus })),
            });
            if (!resp.ok) throw new Error('Visszaigazolás sikertelen');
        },
        onSuccess: () => qc.invalidateQueries({ queryKey: ['rental', rental.id] }),
        onError: (err) => notifications.show({ title: 'Hiba', message: err.message, color: 'red' }),
    });

    return (
        <Stack gap={16}>
            <Stack gap={6}>
                {[
                    ['Átvételi hely', rental.pickupLocation, IconMapPin],
                    ['Üzemanyag',     rental.fuelLevel != null ? `${rental.fuelLevel}%` : '–', IconGauge],
                    ['Kezdés',        fmt(rental.start),       IconCalendar],
                    ['Vége',          fmt(rental.end),         IconCalendar],
                ].map(([lbl, val, Icon]) => (
                    <Group key={lbl} gap={8} align="flex-start">
                        <Icon size={14} color="var(--lightpurple)" style={{ marginTop: 2 }} />
                        <div>
                            <Text fz={11} c="var(--lightpurple)">{lbl}</Text>
                            <Text fz={13} fw={600} c="var(--background)">{val ?? '–'}</Text>
                        </div>
                    </Group>
                ))}
            </Stack>

            <Divider />

            <Group gap={10} align="center">
                <Box className={`rd-check-circle ${iConfirmed ? 'confirmed' : 'pending'}`}>
                    <IconCheck size={14} />
                </Box>
                <Text fz={13} c={iConfirmed ? 'teal.7' : '#060631'}>
                    {isOwner ? 'Te (tulajdonos)' : 'Te (bérlő)'}
                    {iConfirmed ? ' – visszaigazolva' : ' – még nem igazoltad vissza'}
                </Text>
            </Group>
            <Group gap={10} align="center">
                <Box className={`rd-check-circle ${!canConfirm && !iConfirmed ? 'confirmed' : 'pending'}`}>
                    <IconCheck size={14} />
                </Box>
                <Text fz={13} c={(!canConfirm && !iConfirmed) ? 'teal.7' : '#060631'}>
                    {isOwner ? 'Bérlő' : 'Tulajdonos'}
                    {(!canConfirm && !iConfirmed) ? ' – visszaigazolva' : ' – még vár'}
                </Text>
            </Group>

            {canConfirm && (
                <>
                    <Button
                        size="sm" radius="md"
                        style={{ background: 'var(--button)', alignSelf: 'flex-start' }}
                        leftSection={<IconCheck size={14} />}
                        loading={confirmMutation.isPending}
                        onClick={() => confirmMutation.mutate()}
                    >
                        Átvétel visszaigazolása
                    </Button>
                </>
            )}
        </Stack>
    );
}

function ActivePanel({ rental, isOwner, token }) {
    const qc = useQueryClient();

    const iConfirmed = isOwner
        ? rental.status === 'ownerFinishAccepted'
        : rental.status === 'renterFinishAccepted';

    const canConfirm = isOwner
        ? rental.status === 'active' || rental.status === 'renterFinishAccepted'
        : rental.status === 'active' || rental.status === 'ownerFinishAccepted';

    const newStatus = 'finished';

    const returnMutation = useMutation({
        mutationFn: async () => {
            const resp = await fetch(`${API_URL}/Rental/${rental.id}`, {
                method: 'PUT',
                headers: { Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' },
                body: JSON.stringify(buildBody(rental, { status: newStatus })),
            });
            if (!resp.ok) throw new Error('Visszaigazolás sikertelen');
        },
        onSuccess: () => qc.invalidateQueries({ queryKey: ['rental', rental.id] }),
        onError: (err) => notifications.show({ title: 'Hiba', message: err.message, color: 'red' }),
    });

    return (
        <Stack gap={16}>
            <Stack gap={6}>
                {[
                    ['Kezdés', fmt(rental.start), IconCalendar],
                    ['Vége',   fmt(rental.end),   IconCalendar],
                ].map(([lbl, val, Icon]) => (
                    <Group key={lbl} gap={8} align="flex-start">
                        <Icon size={14} color="var(--lightpurple)" style={{ marginTop: 2 }} />
                        <div>
                            <Text fz={11} c="var(--lightpurple)">{lbl}</Text>
                            <Text fz={13} fw={600} c="var(--background)">{val}</Text>
                        </div>
                    </Group>
                ))}
            </Stack>

            <Divider />

            <Group gap={10} align="center">
                <Box className={`rd-check-circle ${iConfirmed ? 'confirmed' : 'pending'}`}>
                    <IconCheck size={14} />
                </Box>
                <Text fz={13} c={iConfirmed ? 'teal.7' : '#060631'}>
                    {isOwner ? 'Te (tulajdonos)' : 'Te (bérlő)'}
                    {iConfirmed ? ' – visszaadást visszaigazoltad' : ' – még nem igazoltad vissza'}
                </Text>
            </Group>

            {canConfirm && !iConfirmed && (
                <>
                    <Button
                        size="sm" radius="md"
                        style={{ background: 'var(--button)', alignSelf: 'flex-start' }}
                        leftSection={<IconCheck size={14} />}
                        loading={returnMutation.isPending}
                        onClick={() => returnMutation.mutate()}
                    >
                        Visszaadás visszaigazolása
                    </Button>
                </>
            )}
        </Stack>
    );
}

function FinishedPanel({ rental, isOwner, token }) {
    const qc = useQueryClient();
    const [ratingValue, setRatingValue] = useState(0);
    const [submitted, setSubmitted] = useState(false);

    const existingRating = isOwner ? rental.ownerRating : rental.renterRating;
    const hasRated = existingRating != null && existingRating > 0;

    const ratingMutation = useMutation({
        mutationFn: async () => {
            const extra = isOwner ? { ownerRating: ratingValue } : { renterRating: ratingValue };
            const resp = await fetch(`${API_URL}/Rental/${rental.id}`, {
                method: 'PUT',
                headers: { Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' },
                body: JSON.stringify(buildBody(rental, extra)),
            });
            if (!resp.ok) throw new Error('Értékelés sikertelen');
        },
        onSuccess: () => {
            setSubmitted(true);
            qc.invalidateQueries({ queryKey: ['rental', rental.id] });
        },
        onError: (err) => notifications.show({ title: 'Hiba', message: err.message, color: 'red' }),
    });

    return (
        <Stack gap={16}>
            <Stack gap={6}>
                {[
                    ['Kezdés',        fmt(rental.start),      IconCalendar],
                    ['Vége',          fmt(rental.end),        IconCalendar],
                    ['Átvételi hely', rental.pickupLocation,  IconMapPin],
                    ['Összeg',        fmtPrice(rental.fullPrice), IconCoin],
                ].map(([lbl, val, Icon]) => (
                    <Group key={lbl} gap={8} align="flex-start">
                        <Icon size={14} color="var(--lightpurple)" style={{ marginTop: 2 }} />
                        <div>
                            <Text fz={11} c="var(--lightpurple)">{lbl}</Text>
                            <Text fz={13} fw={600} c="var(--background)">{val ?? '–'}</Text>
                        </div>
                    </Group>
                ))}
            </Stack>

            <Divider label="Értékelés" labelPosition="left" styles={{ label: { fontSize: 11, color: '#7a7aaa', textTransform: 'uppercase', letterSpacing: '0.06em' } }} />

            {hasRated || submitted ? (
                <Group gap={8} align="center" p="sm" className="rd-rating-done">
                    <Rating value={existingRating ?? ratingValue} readOnly fractions={2} size="sm" color="#ffc219" />
                    <Text fz={12} c="var(--background)" fw={600}>{(existingRating ?? ratingValue).toFixed(1)}</Text>
                </Group>
            ) : (
                <Stack gap={10}>
                    <Text fz={12} c="dimmed">
                        {isOwner ? 'Értékeld a bérlőt' : 'Értékeld a járművet és a tulajdonost'}
                    </Text>
                    <Rating value={ratingValue} onChange={setRatingValue} fractions={2} size="lg" color="#ffc219" />
                    <Button
                        size="sm" radius="md" disabled={ratingValue === 0}
                        loading={ratingMutation.isPending}
                        style={{ background: ratingValue > 0 ? 'var(--button)' : undefined, alignSelf: 'flex-start' }}
                        onClick={() => ratingMutation.mutate()}
                    >
                        Értékelés elküldése
                    </Button>
                </Stack>
            )}
        </Stack>
    );
}

function RentalDetail() {
    const { rentalId } = useParams();
    const navigate     = useNavigate();
    const auth         = JSON.parse(localStorage.getItem('auth'));
    const token        = auth?.token;
    const { data: authUser } = useUser();

    const { data: rental, isLoading, isError } = useQuery({
        queryKey: ['rental', Number(rentalId)],
        queryFn: async () => {
            const resp = await fetch(`${API_URL}/Rental/${rentalId}`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!resp.ok) throw new Error('Not found');
            return resp.json();
        },
        enabled: !!token && !!rentalId,
        refetchInterval: 15000,
    });

    if (isLoading) return (
        <PageLayout title="Bérlés részletei" subtitle="">
            <Center pt={80}><Loader color="var(--button)" /></Center>
        </PageLayout>
    );

    if (isError || !rental) return (
        <PageLayout title="Bérlés részletei" subtitle="">
            <Center pt={80}><Text c="dimmed">A bérlés nem található.</Text></Center>
        </PageLayout>
    );

    const isOwner = authUser?.id === rental.vehicle?.ownerId;
    const isAdmin = authUser?.role === 'Administrator';
    const vehicle = rental.vehicle;
    const statusInfo = STATUS[rental.status] ?? { label: rental.status, mantine: 'gray' };
    const isCancelled = CANCELLED.includes(rental.status);

    // Which panel to show based on status
    const stage = statusInfo.step;
    const showOffer   = stage === 0;
    const showPickup  = stage === 1 || stage === 2;
    const showActive  = rental.status === 'active' || rental.status === 'renterFinishAccepted' || rental.status === 'ownerFinishAccepted';
    const showFinish  = rental.status === 'finished';

    const panelTitle = showOffer  ? 'Egyeztetés'
                     : showPickup ? 'Átvétel visszaigazolása'
                     : showActive ? 'Visszaadás visszaigazolása'
                     : showFinish ? 'Bérlés lezárva'
                     : 'Lemondva';

    return (
        <PageLayout
            title={vehicle ? `${vehicle.manufacturer} ${vehicle.model}` : `Bérlés #${rentalId}`}
            subtitle={vehicle ? `${vehicle.year} · ${vehicle.fuelType} · ${vehicle.transmission}` : ''}
            heroContent={
                <Button
                    variant="subtle"
                    color="gray"
                    leftSection={<IconArrowLeft size={16} />}
                    onClick={() => navigate(isOwner ? `/vehicle/${vehicle?.id}` : '/rentals')}
                >
                    Vissza
                </Button>
            }
        >
            <Grid gutter="xl">
                <Grid.Col span={{ base: 12, md: 4 }}>
                    <Stack gap={16}>
                        {vehicle && (
                            <CarCard
                                car={vehicle}
                                onClick={() => navigate(`/Vehicle/${vehicle.id}`)}
                            />
                        )}
                        <Paper radius="md" withBorder p="md">
                            <Group justify="space-between" align="center">
                                <Text fz={12} c="dimmed">Állapot</Text>
                                <Badge variant="light" color={statusInfo.mantine} size="sm">
                                    {statusInfo.label}
                                </Badge>
                            </Group>
                        </Paper>

                        <Paper radius="md" withBorder p="md">
                            <Stack gap={12}>
                                <Text fz={11} fw={700} c="var(--lightpurple)" tt="uppercase" style={{ letterSpacing: '0.06em' }}>Résztvevők</Text>
                                <Group gap={10}>
                                    <Box className="rd-avatar"><IconUser size={16} /></Box>
                                    <div>
                                        <Text fz={11} c="var(--lightpurple)">Bérlő</Text>
                                        <Text fz={13} fw={600} c="var(--background)">{rental.renter?.name ?? '–'}</Text>
                                    </div>
                                </Group>
                                <Group gap={10}>
                                    <Box className="rd-avatar"><IconCar size={16} /></Box>
                                    <div>
                                        <Text fz={11} c="var(--lightpurple)">Tulajdonos</Text>
                                        <Text fz={13} fw={600} c="var(--background)">{vehicle?.owner?.name ?? '–'}</Text>
                                    </div>
                                </Group>
                            </Stack>
                        </Paper>

                        <Paper radius="md" withBorder p="md">
                            <Stack gap={8}>
                                <Text fz={11} fw={700} c="var(--lightpurple)" tt="uppercase" style={{ letterSpacing: '0.06em' }}>Pénzügy</Text>
                                {[
                                    ['Bérleti díj', fmtPrice(rental.rentalPrice)],
                                    ['Jutalék',      fmtPrice(rental.commission)],
                                    ['Teljes',       fmtPrice(rental.fullPrice)],
                                ].map(([l, v]) => (
                                    <Group key={l} justify="space-between">
                                        <Text fz={12} c="dimmed">{l}</Text>
                                        <Text fz={12} fw={700} c="var(--background)">{v}</Text>
                                    </Group>
                                ))}
                            </Stack>
                        </Paper>
                    </Stack>
                </Grid.Col>
                
                <Grid.Col span={{ base: 12, md: 8 }}>
                    <Stack gap={16}>
                        <Paper radius="md" withBorder p="md">
                            <Stack gap={16}>
                                <Text fz={13} fw={700} c="var(--background)" tt="uppercase" style={{ letterSpacing: '0.04em' }}>
                                    {panelTitle}
                                </Text>

                                {isCancelled ? (
                                    <Group gap={8}>
                                        <Box className="rd-cancelled-icon"><IconX size={14} /></Box>
                                        <Text fz={13} c="red.7">{statusInfo.label}</Text>
                                    </Group>
                                ) : showOffer ? (
                                    <OfferPanel rental={rental} isOwner={isOwner} token={token} authUser={authUser} />
                                ) : showPickup ? (
                                    <PickupPanel rental={rental} isOwner={isOwner} token={token} />
                                ) : showActive ? (
                                    <ActivePanel rental={rental} isOwner={isOwner} token={token} />
                                ) : showFinish ? (
                                    <FinishedPanel rental={rental} isOwner={isOwner} token={token} />
                                ) : null}
                            </Stack>
                        </Paper>

                        {!isCancelled && (
                            <Paper radius="md" withBorder p="md">
                                <Stack gap={12}>
                                    <Text fz={13} fw='bold' c="var(--background)" tt="uppercase">
                                        Üzenetek
                                    </Text>
                                    <Chat
                                        rentalId={rental.id}
                                        token={token}
                                        authUser={authUser}
                                        disabled={rental.status === 'finished'}
                                    />
                                </Stack>
                            </Paper>
                        )}
                    </Stack>
                </Grid.Col>
            </Grid>
        </PageLayout>
    );
}

export default RentalDetail;
