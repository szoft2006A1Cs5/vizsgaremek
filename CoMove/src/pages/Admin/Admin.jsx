import { useEffect } from "react";
import { useUser } from "../../assets/scripts/hooks/AuthUser";
import PageLayout from "../../components/common/PageLayout/PageLayout";
import { useNavigate } from "react-router-dom";
import { Badge, Grid, Group, Paper, Stack, Text } from "@mantine/core";
import { useQuery } from "@tanstack/react-query";
import { fetchAPI } from "../../assets/scripts/Utilities";
import AdminList, { AdminListRow } from "../../components/Admin/AdminList";
import RentalRow from "../../components/common/RentalRow/RentalRow";

function Admin() {
    const navigate = useNavigate();
    const { data: authUser, isSuccess: userSuccess } = useUser();

    useEffect(() => {
        if (userSuccess && authUser?.role !== "administrator")
            navigate('/');
    }, [authUser, userSuccess])

    const { data: users, isLoading: usersLoading, error: usersError } = useQuery({
        queryKey: ['adminUsers'],
        queryFn: async () => {
            const resp = await fetchAPI('/Admin/User');

            if (!resp.ok) {
                const respJson = await resp.json().catch(() => {});
                throw new Error(respJson?.error ?? "Nem sikerült betölteni a felhasználókat.");
            }

            return resp.json();
        }
    })

    const { data: vehicles, isLoading: vehiclesLoading, error: vehiclesError } = useQuery({
        queryKey: ['adminVehicles'],
        queryFn: async () => {
            const resp = await fetchAPI('/Admin/Vehicle');

            if (!resp.ok) {
                const respJson = await resp.json().catch(() => {});
                throw new Error(respJson?.error ?? "Nem sikerült betölteni a járműveket.");
            }

            return resp.json();
        }
    })

    const { data: rentals, isLoading: rentalsLoading, error: rentalsError } = useQuery({
        queryKey: ['adminRentals'],
        queryFn: async () => {
            const resp = await fetchAPI('/Admin/Rental?active=true');

            if (!resp.ok) {
                const respJson = await resp.json().catch(() => {});
                throw new Error(respJson?.error ?? "Nem sikerült betölteni a bérléseket.");
            }

            return resp.json();
        }
    })

    return (
        <PageLayout 
            title='Adminisztrációs felület'
            subtitle='Kezelje az oldal felhasználóit, járműveit és bérléseit.'
        >
            <Grid>
                <Grid.Col span={{ base: 12, md: 4 }}>
                    <AdminList 
                        title='Felhasználók'
                        loading={usersLoading} 
                        error={usersError} 
                    >
                        <Stack gap={15}>
                            {users && users.map((user, i) => (
                                <AdminListRow key={i} onClick={() => navigate(`/account/${user?.id}`)}>
                                    <Stack gap={5}>
                                        <Group justify="space-between">
                                            <Group>
                                                <Text fw='bold' fz={14}>{user?.id}</Text>
                                                <Text fw='bold' fz={14}>{user?.name}</Text>
                                            </Group>
                                            { user?.role === "administrator" && (<Badge variant="light" color="red">Admin</Badge>) }
                                        </Group>
                                        <Group justify="flex-start" gap={10}>
                                            <Text c='dimmed' fz={11}>{user?.email}</Text>
                                            <Text c='dimmed' fz={11}>{user?.phone}</Text>
                                            <Text c='dimmed' fz={11}>{user?.addressSettlement}</Text>
                                        </Group>
                                    </Stack>
                                </AdminListRow>
                            ))}
                        </Stack>
                    </AdminList>
                </Grid.Col>
                <Grid.Col span={{ base: 12, md: 4 }}>
                    <AdminList 
                        title='Járművek'
                        loading={vehiclesLoading} 
                        error={vehiclesError} 
                    >
                        <Stack gap={15}>
                            {vehicles && vehicles.map((vehicle, i) =>
                                <AdminListRow key={i} onClick={() => navigate(`/vehicle/${vehicle?.id}`)}>
                                    <Stack gap={5}>
                                        <Group>
                                            <Text fw='bold' fz={14}>{vehicle?.id}</Text>
                                            <Text fw='bold' fz={14}>{vehicle?.manufacturer} {vehicle?.model}</Text>
                                        </Group>
                                        <Group justify="flex-start" gap={10}>
                                            <Text c='dimmed' fz={11}>{vehicle?.owner?.name}</Text>
                                            <Text c='dimmed' fz={11}>{vehicle?.owner?.addressSettlement}</Text>
                                        </Group>
                                    </Stack>
                                </AdminListRow>
                            )}
                        </Stack>
                    </AdminList>
                </Grid.Col>
                <Grid.Col span={{ base: 12, md: 4 }}>
                    <AdminList 
                        title='Aktív bérlések'
                        loading={rentalsLoading} 
                        error={rentalsError} 
                    >
                        <Stack gap={15}>
                            {rentals && rentals.map((rental, i) =>
                                <RentalRow
                                    rental={rental}
                                    forAdmin={true}
                                    key={i}
                                />
                            )}
                        </Stack>
                    </AdminList>
                </Grid.Col>
            </Grid>
        </PageLayout>
    );
}

export default Admin;