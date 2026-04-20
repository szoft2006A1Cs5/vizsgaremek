import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { notifications } from '@mantine/notifications';
import {
    Paper, Stack, SimpleGrid, Group, Text,
    ActionIcon, Modal, Button, Center, Loader, FileButton, Image,
} from '@mantine/core';
import { IconTrash, IconUpload, IconChevronRight, IconChevronLeft } from '@tabler/icons-react';
import { API_URL, BACKEND_URL } from '../../../assets/scripts/Config';
import defaultImage from '../../../assets/kepek/egyeb/default.png';

function VehicleImage({ vehicleImage, onLeft, leftDisabled = false, onRight, rightDisabled = false, onDelete }) {
    return (
        <Stack gap={10}>
            <Image
                src={`${BACKEND_URL}/${vehicleImage.path}`}
                h={120}
                fit="cover"
                radius="md"
                fallbackSrc={defaultImage}
            />
            <Group justify="space-between" gap={5}>
                <Group gap={5}>
                    <ActionIcon variant="light" size="sm" disabled={leftDisabled} onClick={() => onLeft()}>
                        <IconChevronLeft size={15} />
                    </ActionIcon>
                    <ActionIcon variant="light" size="sm" disabled={rightDisabled} onClick={() => onRight()}>
                        <IconChevronRight size={15} />
                    </ActionIcon>
                </Group>
                <ActionIcon variant="light" color="red" size="sm" onClick={() => onDelete()}>
                    <IconTrash size={15} />
                </ActionIcon>
            </Group>
        </Stack>
    )
}

function VehicleImageMenu({ vehicleId }) {
    const queryClient = useQueryClient();
    const [deleteTarget, setDeleteTarget] = useState(null);

    const { data: images = [], isLoading } = useQuery({
        queryKey: ['vehicle-images', vehicleId],
        queryFn: async () => {
            const resp = await fetch(`${API_URL}/Vehicle/${vehicleId}/Image`, {
                credentials: "include",
            });

            if (!resp.ok)
                throw new Error('Hiba a képek betöltése során!');

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
                credentials: "include",
                body: formData,
            });

            if (!resp.ok)
                throw new Error('Feltöltés sikertelen');
        },
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ['vehicle-images', vehicleId] }),
        onError: (err) => notifications.show({ title: 'Hiba', message: err.message, color: 'red' }),
    });

    const deleteMutation = useMutation({
        mutationFn: async (imageId) => {
            const resp = await fetch(`${API_URL}/Vehicle/${vehicleId}/Image/${imageId}`, {
                method: 'DELETE',
                credentials: "include",
            });

            if (!resp.ok)
                throw new Error('Törlés sikertelen');
        },
        onSuccess: () => {
            setDeleteTarget(null);
            queryClient.invalidateQueries({ queryKey: ['vehicle-images', vehicleId] });
        },
        onError: (err) => notifications.show({ title: 'Hiba', message: err.message, color: 'red' }),
    });

    const reorderMutation = useMutation({
        mutationFn: async ({ imageId, sortIndex }) => {
            const resp = await fetch(`${API_URL}/Vehicle/${vehicleId}/Image/${imageId}`, {
                method: 'PUT',
                credentials: "include",
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(sortIndex),
            });

            if (!resp.ok)
                throw new Error('Átrendezés sikertelen');
        },
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ['vehicle-images', vehicleId] }),
        onError: (err) => notifications.show({ title: 'Hiba', message: err.message, color: 'red' }),
    });

    async function moveLeft(index) {
        if (index === 0) return;

        const curr = sorted[index];
        const prev = sorted[index - 1];

        await reorderMutation.mutateAsync({ imageId: curr.imageId, sortIndex: prev.sortIndex });
        await reorderMutation.mutateAsync({ imageId: prev.imageId, sortIndex: curr.sortIndex });
    }

    async function moveRight(index) {
        if (index === sorted.length - 1) return;

        const curr = sorted[index];
        const next = sorted[index + 1];

        await reorderMutation.mutateAsync({ imageId: curr.imageId, sortIndex: next.sortIndex });
        await reorderMutation.mutateAsync({ imageId: next.imageId, sortIndex: curr.sortIndex });
    }

    return (
        <>
            <Modal
                opened={deleteTarget !== null}
                onClose={() => setDeleteTarget(null)}
                title="Kép törlése"
                centered
            >
                <Text fz={15}>Biztosan törli ezt a képet?</Text>
                <Group justify="flex-end" mt={20}>
                    <Button variant="default" onClick={() => setDeleteTarget(null)}>Mégsem</Button>
                    <Button color="red" loading={deleteMutation.isPending} onClick={() => deleteMutation.mutate(deleteTarget)}>Törlés</Button>
                </Group>
            </Modal>

            <Paper shadow="md" radius="md" p={30}>
                <Stack gap={15}>
                    <Group justify="space-between">
                        <Text fz={18} fw='bold' c="var(--background)">Képek</Text>
                        <FileButton onChange={(file) => file && uploadMutation.mutate(file)} accept="image/*">
                            {(props) => (
                                <Button
                                    {...props}
                                    loading={uploadMutation.isPending}
                                    leftSection={<IconUpload size={15} />}
                                    radius="md"
                                    style={{ background: 'var(--button)' }}
                                >
                                    Kép hozzáadása
                                </Button>
                            )}
                        </FileButton>
                    </Group>

                    {isLoading || reorderMutation.isPending ? (
                        <Center py={40}><Loader color="var(--button)" /></Center>
                    ) : sorted.length === 0 ? (
                        <Center py={40}><Text c="dimmed" fz={15}>Még nincs feltöltött kép.</Text></Center>
                    ) : (
                        <SimpleGrid cols={{ base: 2, sm: 3, md: 4 }} spacing="md">
                            {sorted.map((image, i) => (
                                <VehicleImage 
                                    vehicleImage={image}
                                    onLeft={() => moveLeft(i)}
                                    onRight={() => moveRight(i)}
                                    leftDisabled={i === 0}
                                    rightDisabled={i === sorted.length - 1}
                                    onDelete={() => setDeleteTarget(image.imageId)}
                                    key={i}
                                />
                            ))}
                        </SimpleGrid>
                    )}
                </Stack>
            </Paper>
        </>
    );
}

export default VehicleImageMenu;
