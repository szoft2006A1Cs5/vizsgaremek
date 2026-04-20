import { Grid, Stack, Text } from "@mantine/core";
import { formatDateTime } from "../../../assets/scripts/Utilities";

function InfoDash({ rental }) {
    return (
        <Grid>
            <Grid.Col span={{ base: 12, sm: 4 }}>
                <Stack gap={0}>
                    <Text c='var(--lightpurple)' fz={12}>
                        Bérlés kezdete
                    </Text>
                    <Text c='var(--background)' fz={15}>{formatDateTime(rental.start)}</Text>
                </Stack>
            </Grid.Col>

            <Grid.Col span={{ base: 12, sm: 4 }}>
                <Stack gap={0}>
                    <Text c='var(--lightpurple)' fz={12}>
                        Bérlés vége
                    </Text>
                    <Text c='var(--background)' fz={15}>{formatDateTime(rental.end)}</Text>
                </Stack>
            </Grid.Col>

            <Grid.Col span={{ base: 12, sm: 4 }}>
                <Stack gap={0}>
                    <Text c='var(--lightpurple)' fz={12}>
                        Átvételi hely
                    </Text>
                    <Text c='var(--background)' fz={15}>{rental.pickupLocation}</Text>
                </Stack>
            </Grid.Col>
        </Grid>
    )
} 

export default InfoDash;