import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { notifications } from '@mantine/notifications';
import {
    Paper, Stack, Group, Text, ActionIcon, Modal,
    Button, Center, Loader, NumberInput, Table, Badge,
} from '@mantine/core';
import { DateTimePicker } from '@mantine/dates';
import { IconTrash, IconEdit, IconPlus, IconCalendar } from '@tabler/icons-react';
import { API_URL } from '../../../assets/scripts/Config';
import { fetchAPI, formatDateTime } from '../../../assets/scripts/Utilities';
import '@mantine/dates/styles.css';
import 'dayjs/locale/hu'

function AvailabilityRow({ availability, onEdit, onDelete }) {
    return (
        <Paper radius='md' p='md' withBorder>
            <Group wrap='nowrap' justify='space-between'>
                <Group wrap='wrap' gap='xl' style={{ flex: 1 }}>
                    <Stack gap={5}>
                        <Text fw='bold' fz={13}>Kezdet</Text>
                        <Text fz={12}>{formatDateTime(availability.start)}</Text>
                    </Stack>
                    <Stack gap={5}>
                        <Text fw='bold' fz={13}>Vég</Text>
                        <Text fz={12}>{formatDateTime(availability.end)}</Text>
                    </Stack>
                    <Stack gap={5}>
                        <Text fw='bold' fz={13}>Óradíj</Text>
                        <Text fz={12}>{availability.hourlyRate.toLocaleString('hu-HU')} Ft/óra</Text>
                    </Stack>
                </Group>
                <Stack gap={15} ml='xl'>
                    <ActionIcon variant='light' size='sm' p={1} onClick={() => onEdit()}>
                        <IconEdit />
                    </ActionIcon>
                    <ActionIcon variant='light' color='red' size='sm' p={1} onClick={() => onDelete()}>
                        <IconTrash />
                    </ActionIcon>
                </Stack>
            </Group>
        </Paper>
    )
};

function VehicleAvailabilityMenu({ vehicleId }) {
    const queryClient = useQueryClient();
    const [addEditOpen, setAddEditOpen] = useState(false);
    const [editTarget, setEditTarget] = useState(null);
    const [deleteTarget, setDeleteTarget] = useState(null);
    const [fields, setFields] = useState({});
    const [error, setError] = useState(null);

    const { data: availabilities = [], isLoading } = useQuery({
        queryKey: ['vehicle-availabilities', vehicleId],
        queryFn: async () => {
            const resp = await fetchAPI(`/Vehicle/${vehicleId}/Availability`);

            if (!resp.ok) throw new Error('Nem sikerült lekérni az elérhetőségeket!');

            return resp.json();
        },
    });

    const sorted = [...availabilities].sort((a, b) => new Date(a.start) - new Date(b.start));

    function openAdd() {
        setEditTarget(null);
        setFields({});
        setError(null);
        setAddEditOpen(true);
    }

    function openEdit(avail) {
        setEditTarget(avail.availabilityId);
        setFields({
            start: new Date(avail.start),
            end: new Date(avail.end),
            hourlyRate: avail.hourlyRate,
        });
        setError(null);
        setAddEditOpen(true);
    }

    const saveMutation = useMutation({
        mutationFn: async (body) => {
            const url = editTarget !== null
                ? `/Vehicle/${vehicleId}/Availability/${editTarget}`
                : `/Vehicle/${vehicleId}/Availability`;
            
            const resp = await fetchAPI(url, {
                method: editTarget !== null ? 'PUT' : 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(body),
            });

            if (resp.status === 409) throw new Error('Ez az időszak ütközik egy meglévő elérhetőséggel.');
            if (!resp.ok) throw new Error('Hiba történt a mentés során.');
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['vehicle-availabilities', vehicleId] });
            setFields({});
            setEditTarget(null);
            setAddEditOpen(false);
        },
        onError: (err) => notifications.show({ title: 'Hiba', message: err.message, color: 'red' }),
    });

    const deleteMutation = useMutation({
        mutationFn: async (id) => {
            const resp = await fetchAPI(`/Vehicle/${vehicleId}/Availability/${id}`, {
                method: 'DELETE',
            });

            if (!resp.ok) throw new Error('Törlés sikertelen');
        },
        onSuccess: () => {
            setDeleteTarget(null);
            queryClient.invalidateQueries({ queryKey: ['vehicle-availabilities', vehicleId] });
        },
        onError: (err) => notifications.show({ title: 'Hiba', message: err.message, color: 'red' }),
    });

    function handleSave() {
        if (!fields.start || !fields.end) { 
            setError('Add meg a kezdési és befejezési időpontot.'); 
            return; 
        }

        if (new Date(fields.end) <= new Date(fields.start)) { 
            setError('A befejezés időpontjának a kezdés után kell lennie.');
            return; 
        }

        if (!fields.hourlyRate || fields.hourlyRate <= 0) { 
            setError('Az óradíjnak pozitívnak kell lennie.'); 
            return;
        }

        setError(null);

        saveMutation.mutate({
            start: new Date(fields.start).toISOString(),
            end: new Date(fields.end).toISOString(),
            hourlyRate: fields.hourlyRate,
        });
    }

    return (
        <>
            <Modal
                opened={deleteTarget !== null}
                onClose={() => setDeleteTarget(null)}
                title="Elérhetőség törlése"
                centered
            >
                <Text fz={15} fw='bold'>Biztosan törli ezt az elérhetőségi időszakot?</Text>
                <Group justify="flex-end" mt={20}>
                    <Button variant="default" onClick={() => setDeleteTarget(null)}>Mégsem</Button>
                    <Button color="red" loading={deleteMutation.isPending} onClick={() => deleteMutation.mutate(deleteTarget)}>Törlés</Button>
                </Group>
            </Modal>

            <Modal
                opened={addEditOpen}
                onClose={() => setAddEditOpen(false)}
                title={editTarget !== null ? 'Elérhetőség szerkesztése' : 'Elérhetőség hozzáadása'}
                centered
            >
                <Stack gap={15}>
                    <DateTimePicker
                        label="Kezdés"
                        placeholder="Válassz időpontot"
                        value={fields.start}
                        onChange={(val) => setFields((fields) => ({ ...fields, start: val }))}
                        minDate={new Date()}
                        leftSection={<IconCalendar size={15} />}
                        dropdownType="modal"
                        locale='hu'
                        valueFormat='YYYY. MM. DD. HH:mm'
                        modalProps={{ styles: { inner: { paddingTop: '100px' } } }}
                        radius="md"
                        clearable
                    />
                    <DateTimePicker
                        label="Vége"
                        placeholder="Válassz időpontot"
                        value={fields.end}
                        onChange={(val) => setFields((fields) => ({ ...fields, end: val }))}
                        minDate={fields.start ?? new Date()}
                        leftSection={<IconCalendar size={15} />}
                        dropdownType="modal"
                        locale='hu'
                        valueFormat='YYYY. MM. DD. HH:mm'
                        modalProps={{ styles: { inner: { paddingTop: '100px' } } }}
                        radius="md"
                        clearable
                    />
                    <NumberInput
                        label="Óradíj (Ft)"
                        value={fields.hourlyRate ?? 0}
                        onChange={(val) => setFields((f) => ({ ...f, hourlyRate: val }))}
                        radius="md"
                    />
                    {error && <Text fz={15} fw='bold' c="red">{error}</Text>}
                    <Group justify="flex-end">
                        <Button variant="default" onClick={() => setAddEditOpen(false)}>Mégsem</Button>
                        <Button
                            loading={saveMutation.isPending}
                            onClick={handleSave}
                            radius="md"
                            style={{ background: 'var(--button)' }}
                        >
                            Mentés
                        </Button>
                    </Group>
                </Stack>
            </Modal>

            <Paper shadow="md" radius="md" p={30}>
                <Stack gap={15}>
                    <Group justify="space-between">
                        <Text fz={18} fw='bold' c="var(--background)">Elérhetőségek</Text>
                        <Button
                            leftSection={<IconPlus size={16} />}
                            onClick={openAdd}
                            radius="md"
                            style={{ background: 'var(--button)' }}
                        >
                            Időszak hozzáadása
                        </Button>
                    </Group>

                    {isLoading ? (
                        <Center py={40}><Loader color="var(--button)" /></Center>
                    ) : sorted.length === 0 ? (
                        <Center py={40}><Text c="dimmed" fz={14}>Még nincs megadott elérhetőség.</Text></Center>
                    ) : (
                        <Stack gap={15}>
                            {sorted.map((avail, i) => {
                                return (
                                    <AvailabilityRow 
                                        availability={avail} 
                                        key={i} 
                                        onEdit={() => openEdit(avail)} 
                                        onDelete={() => setDeleteTarget(avail.availabilityId)} 
                                    />
                                )
                            })}
                        </Stack>
                    )}
                </Stack>
            </Paper>
        </>
    );
}

export default VehicleAvailabilityMenu;
