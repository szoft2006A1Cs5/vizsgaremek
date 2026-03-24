import { useQuery } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { Badge, Text, Stack, Group, Paper, Loader, Center, Box, ActionIcon } from '@mantine/core';
import { IconCalendar, IconCoin, IconChevronRight } from '@tabler/icons-react';
import PageLayout from '../../components/PageLayout/PageLayout';
import { API_URL, BACKEND_URL } from '../../assets/scripts/Config';
import defaultImage from '../../assets/kepek/egyeb/default.png';
import './Rentals.css';

const RENTAL_STATUS = {
    renterOffer:          { label: 'Ajánlat elküldve',          mantine: 'blue'   },
    ownerOffer:           { label: 'Tulajdonos ajánlata',       mantine: 'cyan'   },
    offerAccepted:        { label: 'Ajánlat elfogadva',         mantine: 'teal'   },
    renterPickupAccepted: { label: 'Bérlő átvétele: kész',      mantine: 'indigo' },
    ownerPickupAccepted:  { label: 'Tulaj átvétele: kész',      mantine: 'indigo' },
    active:               { label: 'Aktív bérlés',              mantine: 'green'  },
    renterFinishAccepted: { label: 'Bérlő visszaadása: kész',   mantine: 'yellow' },
    ownerFinishAccepted:  { label: 'Tulaj visszaadása: kész',   mantine: 'yellow' },
    finished:             { label: 'Befejezve',                 mantine: 'gray'   },
    renterCancelled:      { label: 'Bérlő által lemondva',      mantine: 'red'    },
    ownerCancelled:       { label: 'Tulajdonos által lemondva', mantine: 'red'    },
};

function formatDate(str) {
    if (!str) return '–';
    return new Date(str).toLocaleDateString('hu-HU', { year: 'numeric', month: '2-digit', day: '2-digit' });
}

function formatPrice(n) {
    if (n == null) return '–';
    return Number(n).toLocaleString('hu-HU') + ' Ft';
}

function RentalRow({ rental }) {
    const navigate = useNavigate();
    const statusInfo = RENTAL_STATUS[rental.status] ?? { label: rental.status, mantine: 'gray' };
    const vehicle = rental.vehicle;
    const imgSrc = vehicle?.images?.[0]?.path
        ? `${BACKEND_URL}/${vehicle.images[0].path}`
        : defaultImage;

    const needsAction =
        rental.status === 'ownerOffer' ||
        rental.status === 'offerAccepted' ||
        rental.status === 'ownerPickupAccepted' ||
        rental.status === 'ownerFinishAccepted';

    return (
        <Paper
            radius="md"
            withBorder
            className={`rentals-row ${needsAction ? 'rentals-row-action' : ''}`}
            onClick={() => navigate(`/rentals/${rental.id}`)}
        >
            <Group gap={14} wrap="nowrap" p="md" justify="space-between">
                <Group gap={14} wrap="nowrap" style={{ minWidth: 0, flex: 1 }}>
                    <img src={imgSrc} alt="Jármű" className="rentals-row-thumb" />
                    <Stack gap={3} style={{ minWidth: 0 }}>
                        <Group gap={8} align="center">
                            <Text fz={14} fw={700} c="var(--background)" truncate>
                                {vehicle ? `${vehicle.manufacturer} ${vehicle.model}` : `Bérlés #${rental.id}`}
                            </Text>
                            {needsAction && (
                                <Box className="rentals-action-dot" />
                            )}
                        </Group>
                        <Group gap={12} wrap="nowrap">
                            <Group gap={4}>
                                <IconCalendar size={11} color="var(--lightpurple)" />
                                <Text fz={11} c="dimmed">{formatDate(rental.start)} – {formatDate(rental.end)}</Text>
                            </Group>
                            {rental.fullPrice != null && (
                                <Group gap={4}>
                                    <IconCoin size={11} color="var(--lightpurple)" />
                                    <Text fz={11} c="dimmed">{formatPrice(rental.fullPrice)}</Text>
                                </Group>
                            )}
                        </Group>
                    </Stack>
                </Group>
                <Group gap={10} wrap="nowrap">
                    <Badge variant="light" color={statusInfo.mantine} size="sm" style={{ flexShrink: 0 }}>
                        {statusInfo.label}
                    </Badge>
                    <ActionIcon variant="subtle" color="gray" size="sm">
                        <IconChevronRight size={14} />
                    </ActionIcon>
                </Group>
            </Group>
        </Paper>
    );
}

function Rentals() {
    const auth  = JSON.parse(localStorage.getItem('auth'));
    const token = auth?.token;

    const { data: rentals, isLoading, isError } = useQuery({
        queryKey: ['rentals-renter'],
        queryFn: async () => {
            const resp = await fetch(`${API_URL}/Rental`, { headers: { Authorization: `Bearer ${token}` } });
            if (!resp.ok) throw new Error('Failed');
            return resp.json();
        },
        enabled: !!token,
    });

    const active   = (rentals ?? []).filter(r => r.status !== 'finished' && r.status !== 'renterCancelled' && r.status !== 'ownerCancelled');
    const archived = (rentals ?? []).filter(r => r.status === 'finished' || r.status === 'renterCancelled' || r.status === 'ownerCancelled');

    return (
        <PageLayout title="Bérléseim" subtitle="Az Ön által indított bérlések">
            {isLoading ? (
                <Center pt={80}><Loader color="var(--button)" /></Center>
            ) : isError ? (
                <Center pt={80}><Text c="dimmed" fz={14}>Hiba történt a bérlések betöltésekor.</Text></Center>
            ) : !rentals || rentals.length === 0 ? (
                <Center pt={80}><Text c="dimmed" fz={14}>Még nincs bérlésed. Keress egy járművet és küldj ajánlatot!</Text></Center>
            ) : (
                <Stack gap={24}>
                    {active.length > 0 && (
                        <Stack gap={8}>
                            <Text fz={11} fw={700} c="#7a7aaa" tt="uppercase" style={{ letterSpacing: '0.06em' }}>
                                Folyamatban ({active.length})
                            </Text>
                            {active.map(r => <RentalRow key={r.id} rental={r} />)}
                        </Stack>
                    )}
                    {archived.length > 0 && (
                        <Stack gap={8}>
                            <Text fz={11} fw={700} c="#7a7aaa" tt="uppercase" style={{ letterSpacing: '0.06em' }}>
                                Archív ({archived.length})
                            </Text>
                            {archived.map(r => <RentalRow key={r.id} rental={r} />)}
                        </Stack>
                    )}
                </Stack>
            )}
        </PageLayout>
    );
}

export default Rentals;
