import { Badge, Group, Stack, Text, Paper } from "@mantine/core";
import { useNavigate } from "react-router-dom";
import { STATUS_DICT } from "../../../assets/scripts/Config";
import { formatDate, formatPrice } from "../../../assets/scripts/Utilities";

function RentalRow({ rental, forAdmin = false }) {
    const navigate = useNavigate();
    const statusInfo = STATUS_DICT[rental.status] ?? { text: rental.status, color: 'gray' };
    const vehicle = rental.vehicle;

    return (
        <Paper
            radius="md"
            withBorder
            p="md"
            onClick={() => navigate(`/rental/${rental.id}`)}
            style={{ cursor: 'pointer' }}
        >
            <Stack gap={5}>
                { forAdmin
                    ? (
                        <Group justify="flex-start">
                            <Text fw="bold" fz={14}>{rental.id}</Text>
                            <Text fw="bold" fz={14}>{rental.renter?.name}</Text>
                            <Text fw="bold" fz={14}>{rental.vehicle?.owner?.name}</Text>
                        </Group>
                    )
                    : (
                        <Group justify="space-between" wrap="nowrap">
                            <Text fw="bold" fz={14}>
                                {vehicle ? `${vehicle.manufacturer} ${vehicle.model} - ${vehicle.owner.name}` : `Bérlés ${rental.id}`}
                            </Text>
                            <Badge variant="light" color={statusInfo.color} size="sm" style={{ flexShrink: 0 }}>
                                {statusInfo.text}
                            </Badge>
                        </Group>
                    )
                }
                <Text fz={11} c="dimmed">
                    {formatDate(rental.start)} - {formatDate(rental.end)}{rental.fullPrice ? ` - ${formatPrice(rental.fullPrice)}` : ''}
                </Text>
            </Stack>
        </Paper>
    );
}

export default RentalRow;
