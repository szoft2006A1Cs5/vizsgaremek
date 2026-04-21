import { Button, Group, NumberInput, Paper, Stack, Text } from "@mantine/core";
import { fetchAPI, formatPrice } from "../../assets/scripts/Utilities";
import { IconWallet } from "@tabler/icons-react";
import { useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { notifications } from "@mantine/notifications";
import { API_URL } from "../../assets/scripts/Config";

function AccountBalance({ user }) {
    const queryClient = useQueryClient();
    const [addedBalance, setAddedBalance] = useState(0);

    const addBalanceMutation = useMutation({
        mutationFn: async () => {
            const resp = await fetchAPI(`/User/${user.id}/Deposit`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                },
                body: addedBalance
            });

            if (!resp.ok) {
                const respJson = await resp.json().catch(() => {});
                throw new Error(respJson?.error ?? "Nem sikerült feltölteni az egyenleget!");
            }
        },
        onSuccess: () => {
            setAddedBalance(0);
            queryClient.invalidateQueries({ queryKey: ["user", String(user.id)] });
        },
        onError: (error) => notifications.show({ title: "Hiba!", message: error.message, color: "red" })
    })
    
    return (
        <Paper p='md' radius='md'>
            <Group justify="space-between" wrap="wrap">
                <Stack gap={0}>
                    <Text c='var(--background)' fz={15} fw='bold'>Egyenleg</Text>
                    <Text c='var(--background)' fz={25} fw='bold'>{formatPrice(user.balance)}</Text>
                </Stack>

                <Group>
                    <NumberInput 
                        placeholder="pl. 12000"
                        rightSection={<Text fz={14}>Ft</Text>}
                        min={1}
                        value={addedBalance}
                        onInput={(e) => setAddedBalance(Number(e.target.value))}
                    />
                    <Button
                        variant="light"
                        color="green"
                        leftSection={<IconWallet />}
                        loading={addBalanceMutation.isPending}
                        onClick={() => addBalanceMutation.mutate()}
                    >
                        Fizetés és feltöltés
                    </Button>
                </Group>
            </Group>
        </Paper>
    )
}

export default AccountBalance;