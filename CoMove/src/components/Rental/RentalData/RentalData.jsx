import { Avatar, Badge, Divider, Group, Paper, Rating, Stack, Text } from "@mantine/core";
import { formatDateTime, formatPrice, formatPic } from "../../../assets/scripts/Utilities";
import { STATUS_DICT } from "../../../assets/scripts/Config";

function ProfileRow({ profile, owner = false }) {
    return (
        <Group justify="space-between">
            <Group>
                <Avatar src={formatPic(profile?.profilePicPath)} color="var(--background)" />
                <Stack gap={0}>
                    <Text fz={11} c='var(--lightpurple)'>{owner ? 'Bérbeadó' : 'Bérlő'}</Text>
                    <Text fz={14} fw='bold'>{profile?.name}</Text>
                    {profile?.phone && <Text fz={12}>{profile?.phone}</Text>}
                </Stack>
            </Group>

            <Rating value={Number(owner ? profile?.ownerRating : profile?.renterRating) || 0} fractions={10} readOnly />
        </Group>
    );
}

function RentalData({ rental }) {
    return (
        <Paper p='md' radius='md' shadow="0 8px 10px rgba(0, 0, 0, 0.48)" >
            <Text fz={18} fw='bold' pb={15} c="var(--background)">A bérlés adatai</Text>

            <Stack gap={15}>
                <Group justify="space-between">
                    <Text c='var(--background)' fz={15} fw='bold'>Állapot</Text>
                    <Badge variant="light" color={STATUS_DICT[rental.status]?.color} >{STATUS_DICT[rental.status]?.text}</Badge>
                </Group>

                <Divider />

                <Stack gap={15}>
                    <ProfileRow profile={rental?.renter} />
                    <ProfileRow profile={rental?.vehicle?.owner} owner={true} />
                </Stack>

                <Divider />

                <Stack gap={15}>
                    <Group justify="space-between">
                        <Text c='var(--lightpurple)' fz={14}>Kezdet</Text>
                        <Text fz={14} c="var(--background)" fw='bold'>{formatDateTime(rental?.start)}</Text>
                    </Group>

                    <Group justify="space-between">
                        <Text c='var(--lightpurple)' fz={14}>Vég</Text>
                        <Text fz={14} c="var(--background)" fw='bold'>{formatDateTime(rental?.end)}</Text>
                    </Group>
                </Stack>                

                <Divider />

                <Stack gap={15}>
                    <Text fz={15} fw='bold' c="var(--background)">Pénzügyek</Text>

                    <Group justify="space-between">
                        <Text c='var(--lightpurple)' fz={14}>Bérlési ár</Text>
                        <Text fz={14} c="var(--background)" fw={500}>{formatPrice(rental?.rentalPrice)}</Text>
                    </Group>

                    <Group justify="space-between">
                        <Text c='var(--lightpurple)' fz={14}>Szolgáltatási díj</Text>
                        <Text fz={14} c="var(--background)" fw={500}>{formatPrice(rental?.commission)}</Text>
                    </Group>

                    <Divider />

                    <Group justify="space-between">
                        <Text c='var(--background)' fw='bold' fz={16}>Teljes ár</Text>
                        <Text fz={16} c="var(--background)" fw='bold'>{formatPrice(rental?.fullPrice)}</Text>
                    </Group>
                </Stack>
            </Stack>
        </Paper>
    )
}

export default RentalData;