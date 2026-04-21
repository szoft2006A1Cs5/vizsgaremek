import { useMutation, useQueryClient } from "@tanstack/react-query";
import { API_URL, nextStatus, requiresActionFromMe, STATUS_DICT } from "../../../assets/scripts/Config";
import InfoDash from "./InfoDash";
import { notifications } from "@mantine/notifications";
import { IconCheck, IconX } from "@tabler/icons-react";
import { Button, Group, Stack, Stepper, Text } from "@mantine/core";
import { fetchAPI } from "../../../assets/scripts/Utilities";

function StatusDash({ rental, me }) {
    const queryClient = useQueryClient();
    const status = STATUS_DICT[rental?.status];
    const isRenter = rental.renterId === me.id
    const role = isRenter ? "renter" : "owner";
    const isMine = requiresActionFromMe(rental.status, role);

    const acceptTexts = [['Átvétel megerősítése', 'Átadás megerősítése'],
                         ['Visszavitel megerősítése', 'Visszahozatal megerősítése']];

    const updateMutation = useMutation({
        mutationFn: async (newStatus) => {
            const resp = await fetchAPI(`/Rental/${rental.id}`, {
                method: newStatus === "cancelled" ? "DELETE" : "PUT",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    start: rental.start,
                    end: rental.end,
                    pickupLocation: rental.pickupLocation,
                    status: newStatus
                })
            })

            if (!resp.ok) {
                const respJson = await resp.json().catch(() => {});
                throw new Error(respJson?.error ?? "Nem sikerült frissíteni a bérlést!");
            }
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["rental", String(rental.id)] });
        },
        onError: (error) => notifications.show({ title: "Hiba!", message: error.message, color: "red" })
    })

    return (
        <Stack>
            <Text c={status.color} fz={18} fw='bold'>{status.text}</Text>
            <InfoDash rental={rental} />
            <Stepper color={status.num % 3 === 2 ? "green" : "yellow"} iconSize={32} active={Math.floor((status.num + 1) / 3)}>
                <Stepper.Step label='Ajánlat elfogadva'></Stepper.Step>
                <Stepper.Step label={`${isRenter ? 'Átvétel' : 'Átadás'} megerősítve`}></Stepper.Step>
                <Stepper.Step label={`${isRenter ? 'Visszavitel' : 'Visszahozatal'} megerősítve`}></Stepper.Step>
            </Stepper>

            {!isMine && <Text c="var(--lightpurple)" fz={15}>Várakozás a másik félre...</Text> }

            <Group gap={10} wrap="wrap">
                { isMine && 
                    (
                        <Button 
                            radius='md'
                            variant="light" 
                            color="green"
                            leftSection={<IconCheck />}
                            onClick={() => updateMutation.mutate(nextStatus(status))}
                            loading={updateMutation.isPending}
                        >
                            {
                                acceptTexts[Math.floor((status.num + 1) / 3) - 1][isRenter ? 0 : 1]
                            }
                        </Button>
                    )
                }

                { status.num < 5 && 
                (
                    <Button 
                        radius='md'
                        variant="light" 
                        color="red"
                        leftSection={<IconX />}
                        onClick={() => updateMutation.mutate("cancelled")}
                        loading={updateMutation.isPending}
                    >
                        Visszamondás
                    </Button>
                )
                }
            </Group>
        </Stack>
    )
}

export default StatusDash;