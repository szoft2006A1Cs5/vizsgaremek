import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { notifications } from '@mantine/notifications';
import {
    Paper, Stack, SimpleGrid, Group, Text,
    ActionIcon, Modal, Button, Center, Loader, FileButton, Image,
} from '@mantine/core';
import { IconTrash, IconChevronUp, IconChevronDown, IconUpload } from '@tabler/icons-react';
import { API_URL, BACKEND_URL } from '../../assets/scripts/Config';
import defaultImage from '../../assets/kepek/egyeb/default.png';

function VehicleImageManager({ vehicleId, token }) {
    const queryClient = useQueryClient();
    const [deleteTarget, setDeleteTarget] = useState(null);

    const { data: images = [], isLoading } = useQuery({
        queryKey: ['vehicle-images', vehicleId],
        queryFn: async () => {
            const resp = await fetch(`${API_URL}/Vehicle/${vehicleId}/Image`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!resp.ok) throw new Error('Failed');
            return resp.json();
        },
    });

    const sorted = [...images].sort((a, b) => a.sortIndex - b.sortIndex);

    const uploadMutation = useMutation({
        mutationFn: async (file) => {
            const formData = new FormData();
            formData.append('file', file);
            const resp = await fetch(`${API_URL}/Vehicle/${vehicleId}/Image`, {
                method: 'POST',
                headers: { Authorization: `Bearer ${token}` },
                body: formData,
            });
            if (!resp.ok) throw new Error('Feltöltés sikertelen');
        },
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ['vehicle-images', vehicleId] }),
        onError: (err) => notifications.show({ title: 'Hiba', message: err.message, color: 'red' }),
    });

    const deleteMutation = useMutation({
        mutationFn: async (id) => {
            const resp = await fetch(`${API_URL}/Vehicle/${vehicleId}/Image/${id}`, {
                method: 'DELETE',
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!resp.ok) throw new Error('Törlés sikertelen');
        },
        onSuccess: () => {
            setDeleteTarget(null);
            queryClient.invalidateQueries({ queryKey: ['vehicle-images', vehicleId] });
        },
        onError: (err) => notifications.show({ title: 'Hiba', message: err.message, color: 'red' }),
    });

    const reorderMutation = useMutation({
        mutationFn: async ({ imageId, newSortIndex }) => {
            const resp = await fetch(`${API_URL}/Vehicle/${vehicleId}/Image/${imageId}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
                body: JSON.stringify(newSortIndex),
            });
            if (!resp.ok) throw new Error('Átrendezés sikertelen');
        },
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ['vehicle-images', vehicleId] }),
        onError: (err) => notifications.show({ title: 'Hiba', message: err.message, color: 'red' }),
    });

    async function moveUp(index) {
        if (index === 0) return;
        const curr = sorted[index];
        const prev = sorted[index - 1];
        await Promise.all([
            reorderMutation.mutateAsync({ imageId: curr.imageId, newSortIndex: prev.sortIndex }),
            reorderMutation.mutateAsync({ imageId: prev.imageId, newSortIndex: curr.sortIndex }),
        ]);
    }

    async function moveDown(index) {
        if (index === sorted.length - 1) return;
        const curr = sorted[index];
        const next = sorted[index + 1];
        await Promise.all([
            reorderMutation.mutateAsync({ imageId: curr.imageId, newSortIndex: next.sortIndex }),
            reorderMutation.mutateAsync({ imageId: next.imageId, newSortIndex: curr.sortIndex }),
        ]);
    }

    return (
        <>
            <Modal
                opened={deleteTarget !== null}
                onClose={() => setDeleteTarget(null)}
                title="Kép törlése"
                centered
            >
                <Text fz={14}>Biztosan törli ezt a képet?</Text>
                <Group justify="flex-end" mt={20}>
                    <Button variant="default" onClick={() => setDeleteTarget(null)}>Mégsem</Button>
                    <Button color="red" loading={deleteMutation.isPending} onClick={() => deleteMutation.mutate(deleteTarget)}>Törlés</Button>
                </Group>
            </Modal>

            <Paper shadow="md" radius="md" p={32}>
                <Stack gap={16}>
                    <Group justify="space-between">
                        <Text fz={15} fw={700} c="var(--background)">Képek</Text>
                        <FileButton onChange={(f) => f && uploadMutation.mutate(f)} accept="image/*">
                            {(props) => (
                                <Button
                                    {...props}
                                    loading={uploadMutation.isPending}
                                    leftSection={<IconUpload size={16} />}
                                    radius="md"
                                    style={{ background: 'var(--button)' }}
                                >
                                    Kép hozzáadása
                                </Button>
                            )}
                        </FileButton>
                    </Group>

                    {isLoading ? (
                        <Center py={40}><Loader color="var(--button)" /></Center>
                    ) : sorted.length === 0 ? (
                        <Center py={40}><Text c="dimmed" fz={14}>Még nincs feltöltött kép.</Text></Center>
                    ) : (
                        <SimpleGrid cols={{ base: 2, sm: 3, md: 4 }} spacing="md">
                            {sorted.map((img, i) => (
                                <Stack key={img.imageId} gap={8}>
                                    <Image
                                        src={`${BACKEND_URL}/${img.path}`}
                                        h={120}
                                        fit="cover"
                                        radius="md"
                                        fallbackSrc={defaultImage}
                                    />
                                    <Group justify="space-between" gap={4}>
                                        <Group gap={4}>
                                            <ActionIcon variant="light" size="sm" disabled={i === 0} onClick={() => moveUp(i)}>
                                                <IconChevronUp size={14} />
                                            </ActionIcon>
                                            <ActionIcon variant="light" size="sm" disabled={i === sorted.length - 1} onClick={() => moveDown(i)}>
                                                <IconChevronDown size={14} />
                                            </ActionIcon>
                                        </Group>
                                        <ActionIcon variant="light" color="red" size="sm" onClick={() => setDeleteTarget(img.imageId)}>
                                            <IconTrash size={14} />
                                        </ActionIcon>
                                    </Group>
                                </Stack>
                            ))}
                        </SimpleGrid>
                    )}
                </Stack>
            </Paper>
        </>
    );
}

export default VehicleImageManager;
