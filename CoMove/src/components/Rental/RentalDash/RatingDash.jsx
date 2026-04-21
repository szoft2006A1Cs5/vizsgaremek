import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Text, Rating, Divider, Stack } from "@mantine/core";
import { notifications } from "@mantine/notifications";
import { API_URL } from "../../../assets/scripts/Config";
import InfoDash from "./InfoDash";
import { fetchAPI } from "../../../assets/scripts/Utilities";

function RatingDash({ rental, me }) {
    const queryClient = useQueryClient();
    const isRenter = rental.renterId === me.id;

    const ratingMutation = useMutation({
        mutationFn: async (rating) => {
            const resp = await fetchAPI(`/Rental/${rental.id}`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    start: rental.start,
                    end: rental.end,
                    pickupLocation: rental.pickupLocation,
                    status: "finished",
                    renterRating: !isRenter ? rating : null,
                    ownerRating: isRenter ? rating : null,
                })
            });

            if (!resp.ok)
                throw new Error("Nem sikerült menteni az értékelést!")
        },
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ["rental", String(rental.id)] }),
        onError: (error) => notifications.show({ title: "Hiba!", message: error.message, color: "red" }) 
    })

    return (
        <Stack gap={10}>
            <Text c={status.color} fz={18} fw='bold'>Értékelje a bérlést!</Text>
                
            <Rating 
                size='lg' 
                value={
                    isRenter 
                        ? Number(rental.ownerRating) || 0 
                        : Number(rental.renterRating) || 0
                } 
                fractions={2} 
                onChange={(rating) => ratingMutation.mutate(rating)} readOnly={ratingMutation.isPending} />

            <Divider />

            <InfoDash rental={rental} />
        </Stack>
    )
}

export default RatingDash;