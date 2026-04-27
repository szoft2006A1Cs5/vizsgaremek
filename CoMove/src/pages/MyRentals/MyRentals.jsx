import { useQuery } from '@tanstack/react-query';
import { Text, Stack, Loader, Center, SimpleGrid } from '@mantine/core';
import PageLayout from '../../components/common/PageLayout/PageLayout';
import { API_URL } from '../../assets/scripts/Config';
import RentalRow from '../../components/common/RentalRow/RentalRow';
import { fetchAPI } from '../../assets/scripts/Utilities';
import { useNavigate } from 'react-router-dom';
import { useUser } from '../../assets/scripts/hooks/AuthUser';
import { useEffect } from 'react';

function MyRentals() {
    const navigate = useNavigate();
    const { data: authUser, isSuccess: userSuccess } = useUser();

    useEffect(() => {
        if (userSuccess && authUser?.role === "administrator")
            navigate("/");
    }, [authUser, userSuccess])

    const { data: rentals, isLoading, isError } = useQuery({
        queryKey: ['myrentals'],
        queryFn: async () => {
            const resp = await fetchAPI('/Rental');

            if (!resp.ok) throw new Error('Nem sikerült lekérni a bérléseket!');

            return resp.json();
        },
    });

    const active  = (rentals ?? []).filter(r => r.status !== 'finished' && r.status !== 'cancelled');
    const archive = (rentals ?? []).filter(r => r.status === 'finished' || r.status === 'cancelled');

    return (
        <PageLayout title="Bérléseim" subtitle="Az általad indított bérlések">
            {isLoading ? (
                <Center pt={100}><Loader color="var(--button)" /></Center>
            ) : isError ? (
                <Center pt={100}><Text c="var(--lightpurple)" fz={14}>Hiba történt a bérlések betöltésekor.</Text></Center>
            ) : !rentals || rentals.length === 0 ? (
                <Center pt={100}><Text c="var(--lightpurple)" fz={14}>Még nincs bérlésed. Keress egy járművet és küldj ajánlatot!</Text></Center>
            ) : (
                <Stack gap={25}>
                    {active.length > 0 && (
                        <Stack gap={5}>
                            <Text fz={15} fw='bold' c="var(--lightpurple)">
                                Folyamatban ({active.length})
                            </Text>
                            <Stack gap={10}>
                                {active.map(r => <RentalRow key={r.id} rental={r} />)}
                            </Stack>
                        </Stack>
                    )}
                    {archive.length > 0 && (
                        <Stack gap={5}>
                            <Text fz={15} fw='bold' c="var(--lightpurple)">
                                Archív ({archive.length})
                            </Text>
                            <Stack gap={10}>
                                {archive.map(r => <RentalRow key={r.id} rental={r} />)}
                            </Stack>
                        </Stack>
                    )}
                </Stack>
            )}
        </PageLayout>
    );
}

export default MyRentals;
