import { useState } from 'react';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import {
    Paper, Stack, Group, Text, ActionIcon, Modal,
    Button, Center, Loader, NumberInput, Table, Badge,
} from '@mantine/core';
import { DateTimePicker } from '@mantine/dates';
import { IconTrash, IconEdit, IconPlus, IconCalendar } from '@tabler/icons-react';
import { API_URL } from '../../assets/scripts/Config';
import '@mantine/dates/styles.css';

const EMPTY_FORM = { start: null, end: null, hourlyRate: 0 };

function formatDateTime(str) {
    if (!str) return '–';
    return new Date(str).toLocaleString('hu-HU', { dateStyle: 'short', timeStyle: 'short' });
}

function VehicleAvailabilityManager({ vehicleId, token }) {
    const queryClient = useQueryClient();
    const [modalOpen, setModalOpen] = useState(false);
    const [editTarget, setEditTarget] = useState(null); // availabilityId being edited
    const [deleteTarget, setDeleteTarget] = useState(null);
    const [form, setForm] = useState(EMPTY_FORM);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState(null);

    const { data: availabilities = [], isLoading } = useQuery({
        queryKey: ['vehicle-availabilities', vehicleId],
        queryFn: async () => {
            const resp = await fetch(`${API_URL}/Vehicle/${vehicleId}/Availability`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!resp.ok) throw new Error('Failed');
            return resp.json();
        },
    });

    const sorted = [...availabilities].sort((a, b) => new Date(a.start) - new Date(b.start));

    function openAdd() {
        setEditTarget(null);
        setForm(EMPTY_FORM);
        setError(null);
        setModalOpen(true);
    }

    function openEdit(avail) {
        setEditTarget(avail.availabilityId);
        setForm({
            start: new Date(avail.start),
            end: new Date(avail.end),
            hourlyRate: avail.hourlyRate,
        });
        setError(null);
        setModalOpen(true);
    }

    async function handleSave() {
        const { start, end } = form;
        if (!start || !end) { setError('Add meg a kezdési és befejezési időpontot.'); return; }
        if (new Date(end) <= new Date(start)) { setError('A befejezés időpontjának a kezdés után kell lennie.'); return; }
        if (form.hourlyRate <= 0) { setError('Az óradíjnak pozitívnak kell lennie.'); return; }

        setSaving(true);
        setError(null);
        try {
            const body = {
                start: new Date(form.start).toISOString(),
                end: new Date(form.end).toISOString(),
                hourlyRate: form.hourlyRate,
            };

            let resp;
            if (editTarget !== null) {
                resp = await fetch(`${API_URL}/Vehicle/${vehicleId}/Availability/${editTarget}`, {
                    method: 'PUT',
                    headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
                    body: JSON.stringify(body),
                });
            } else {
                resp = await fetch(`${API_URL}/Vehicle/${vehicleId}/Availability`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
                    body: JSON.stringify(body),
                });
            }

            if (resp.status === 409) {
                setError('Ez az időszak ütközik egy meglévő elérhetőséggel.');
                return;
            }
            if (!resp.ok) {
                setError('Hiba történt a mentés során.');
                return;
            }

            queryClient.invalidateQueries({ queryKey: ['vehicle-availabilities', vehicleId] });
            setModalOpen(false);
        } finally {
            setSaving(false);
        }
    }

    async function handleDelete() {
        await fetch(`${API_URL}/Vehicle/${vehicleId}/Availability/${deleteTarget}`, {
            method: 'DELETE',
            headers: { Authorization: `Bearer ${token}` },
        });
        setDeleteTarget(null);
        queryClient.invalidateQueries({ queryKey: ['vehicle-availabilities', vehicleId] });
    }

    return (
        <>
            <Modal
                opened={deleteTarget !== null}
                onClose={() => setDeleteTarget(null)}
                title="Elérhetőség törlése"
                centered
            >
                <Text fz={14}>Biztosan törli ezt az elérhetőségi időszakot?</Text>
                <Group justify="flex-end" mt={20}>
                    <Button variant="default" onClick={() => setDeleteTarget(null)}>Mégsem</Button>
                    <Button color="red" onClick={handleDelete}>Törlés</Button>
                </Group>
            </Modal>

            <Modal
                opened={modalOpen}
                onClose={() => setModalOpen(false)}
                title={editTarget !== null ? 'Elérhetőség szerkesztése' : 'Elérhetőség hozzáadása'}
                centered
            >
                <Stack gap={16}>
                    <DateTimePicker
                        label="Kezdés"
                        placeholder="Válassz időpontot"
                        value={form.start}
                        onChange={(val) => setForm((f) => ({ ...f, start: val }))}
                        minDate={new Date()}
                        leftSection={<IconCalendar size={16} />}
                        dropdownType="modal"
                        modalProps={{ styles: { inner: { paddingTop: '100px' } } }}
                        radius="md"
                        clearable
                    />
                    <DateTimePicker
                        label="Vége"
                        placeholder="Válassz időpontot"
                        value={form.end}
                        onChange={(val) => setForm((f) => ({ ...f, end: val }))}
                        minDate={form.start ?? new Date()}
                        leftSection={<IconCalendar size={16} />}
                        dropdownType="modal"
                        modalProps={{ styles: { inner: { paddingTop: '100px' } } }}
                        radius="md"
                        clearable
                    />
                    <NumberInput
                        label="Óradíj (Ft)"
                        min={1}
                        value={form.hourlyRate}
                        onChange={(val) => setForm((f) => ({ ...f, hourlyRate: val }))}
                        radius="md"
                    />
                    {error && <Text fz={13} c="red">{error}</Text>}
                    <Group justify="flex-end">
                        <Button variant="default" onClick={() => setModalOpen(false)}>Mégsem</Button>
                        <Button
                            loading={saving}
                            onClick={handleSave}
                            radius="md"
                            style={{ background: 'linear-gradient(135deg, #192570, #0b1f66)' }}
                        >
                            Mentés
                        </Button>
                    </Group>
                </Stack>
            </Modal>

            <Paper shadow="md" radius="md" p={32}>
                <Stack gap={16}>
                    <Group justify="space-between">
                        <Text fz={15} fw={700} c="#060631">Elérhetőség</Text>
                        <Button
                            leftSection={<IconPlus size={16} />}
                            onClick={openAdd}
                            radius="md"
                            style={{ background: 'linear-gradient(135deg, #192570, #0b1f66)' }}
                        >
                            Időszak hozzáadása
                        </Button>
                    </Group>

                    {isLoading ? (
                        <Center py={40}><Loader color="#192570" /></Center>
                    ) : sorted.length === 0 ? (
                        <Center py={40}><Text c="dimmed" fz={14}>Még nincs megadott elérhetőség.</Text></Center>
                    ) : (
                        <Table striped highlightOnHover withTableBorder>
                            <Table.Thead>
                                <Table.Tr>
                                    <Table.Th>Kezdés</Table.Th>
                                    <Table.Th>Vége</Table.Th>
                                    <Table.Th>Óradíj</Table.Th>
                                    <Table.Th />
                                </Table.Tr>
                            </Table.Thead>
                            <Table.Tbody>
                                {sorted.map((avail) => (
                                    <Table.Tr key={avail.availabilityId}>
                                        <Table.Td>{formatDateTime(avail.start)}</Table.Td>
                                        <Table.Td>{formatDateTime(avail.end)}</Table.Td>
                                        <Table.Td>
                                            <Badge variant="light" color="blue">
                                                {avail.hourlyRate.toLocaleString('hu-HU')} Ft/óra
                                            </Badge>
                                        </Table.Td>
                                        <Table.Td>
                                            <Group gap={4} justify="flex-end">
                                                <ActionIcon variant="light" size="sm" onClick={() => openEdit(avail)}>
                                                    <IconEdit size={14} />
                                                </ActionIcon>
                                                <ActionIcon variant="light" color="red" size="sm" onClick={() => setDeleteTarget(avail.availabilityId)}>
                                                    <IconTrash size={14} />
                                                </ActionIcon>
                                            </Group>
                                        </Table.Td>
                                    </Table.Tr>
                                ))}
                            </Table.Tbody>
                        </Table>
                    )}
                </Stack>
            </Paper>
        </>
    );
}

export default VehicleAvailabilityManager;
