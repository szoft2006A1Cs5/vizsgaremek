import { Button, Group, Stack, Text, TextInput } from "@mantine/core";
import { API_URL } from "../../../assets/scripts/Config";
import { DateTimePicker } from "@mantine/dates";
import { useState } from "react";
import { IconCalendar, IconCheck, IconMapPin, IconX } from "@tabler/icons-react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { notifications } from "@mantine/notifications";
import { useNavigate } from "react-router-dom";

function OfferDash({ rental, me }) {
    const queryClient = useQueryClient();
    const navigate = useNavigate();
    const isRenter = rental.renterId === me.id;
    const isRenterOffer = rental.status === "renterOffer";
    const isMine = (isRenter && isRenterOffer) || (!isRenter && !isRenterOffer);
    const [rentalForm, setRentalForm] = useState({
        start: rental.start,
        end: rental.end,
        pickupLocation: rental.pickupLocation
    });

    const counterOfferStatus = isRenter ? "renterOffer" : "ownerOffer";

    const updateMutation = useMutation({
        mutationFn: async (newStatus) => {
            const resp = await fetch(`${API_URL}/Rental/${rental.id}`, {
                method: newStatus === "cancelled" ? "DELETE" : "PUT",
                credentials: "include",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    ...rentalForm,
                    status: newStatus
                })
            })

            if (!resp.ok) {
                const respJson = await resp.json();
                throw new Error(respJson?.error ?? "Nem sikerült frissíteni a bérlést!");
            }

            if (newStatus === "cancelled")
                navigate("/rentals")
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["rental", String(rental.id)] });
        },
        onError: (error) => notifications.show({ title: "Hiba!", message: error.message, color: "red" })
    })

    return (<>
        <Stack gap={10}>
            <Text c='var(--background)' fz={18} fw='bold'>
                {isMine ? "Ajánlatom" : "Ajánlat"}
            </Text>

            <Text c='var(--lightpurple)' fz={14}>
                {
                    isMine 
                    ? "Elküldted az ajánlatod, ami jelenleg válaszra vár! Alább módosíthatod." 
                    : "Ajánlatot kaptál, alább megerősítheted, vagy ellenajánlatot tehetsz."
                }
            </Text>

            <DateTimePicker
                label="Bérlés kezdete"
                placeholder="Válassz időpontot"
                leftSection={<IconCalendar size={15} />}
                value={new Date(rentalForm.start)}
                onChange={(date) => setRentalForm({ ...rentalForm, start: new Date(date).toISOString() })}
                minDate={new Date()}
                radius="md"
                size="sm"
                maxLength={512}
            />

            <DateTimePicker
                label="Bérlés vége"
                placeholder="Válassz időpontot"
                leftSection={<IconCalendar size={15} />}
                value={new Date(rentalForm.end)}
                onChange={(date) => setRentalForm({ ...rentalForm, end: new Date(date).toISOString() })}
                minDate={rentalForm?.start ? new Date(rentalForm.start) : new Date()}
                radius="md"
                size="sm"
                maxLength={512}
            />

            <TextInput
                label="Átvételi hely"
                placeholder="pl. Budapest, Kossuth tér 1."
                leftSection={<IconMapPin /> }
                value={rentalForm.pickupLocation}
                onInput={(e) => setRentalForm({ ...rentalForm, pickupLocation: e.target.value })}
                radius="md"
                size="sm"
                maxLength={512}
                error={!rentalForm.pickupLocation.trim() ? "Nem adtál meg átvételi helyet!" : null}
            />

            <Group gap={10} wrap="wrap">
                { !isMine && 
                    (
                        <Button 
                            radius='md'
                            variant="light" 
                            color="green"
                            leftSection={<IconCheck />}
                            loading={updateMutation.isPending}
                            onClick={() => updateMutation.mutate("offerAccepted")}
                        >
                            Elfogadás
                        </Button>
                    )
                }

                <Button 
                    radius='md'
                    color="var(--button)"
                    loading={updateMutation.isPending}
                    onClick={() => updateMutation.mutate(counterOfferStatus)}
                >
                    {isMine ? `Módosítás küldése` : `Ellenajánlat küldése`}
                </Button>

                <Button 
                    radius='md'
                    variant="light" 
                    color="red"
                    leftSection={<IconX />}
                    loading={updateMutation.isPending}
                    onClick={() => updateMutation.mutate("cancelled")}
                >
                    Visszamondás
                </Button>
            </Group>
        </Stack>
    </>)
}

export default OfferDash;