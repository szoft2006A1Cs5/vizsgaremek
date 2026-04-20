import { ActionIcon, Avatar, FileButton, Grid, Group, Paper, Stack } from "@mantine/core";
import { API_URL, BACKEND_URL } from "../../assets/scripts/Config";
import { getRespJsonError } from "../../assets/scripts/Utilities";
import { IconUpload, IconX } from "@tabler/icons-react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { notifications } from "@mantine/notifications";
import AccountForm from "./AccountForm";

function AccountEdit({ user }) {
    const queryClient = useQueryClient();

    const updateProfilePicMutation = useMutation({
        mutationFn: async (file) => {
            const formData = new FormData();

            if (file)
                formData.append('file', file);

            const resp = await fetch(`${API_URL}/User/${user.id}/Image`, {
                method: "PUT",
                credentials: "include",
                body: formData
            });

            if (!resp.ok) {
                const respJson = await getRespJsonError(resp);
                throw new Error(respJson?.error ?? "Nem sikerült frissíteni a profilképet!");
            }
        },
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ["user", String(user.id)] }),
        onError: (error) => notifications.show({ title: "Hiba!", message: error.message, color: "red" })
    });

    return (
        <Paper p='md' radius='md'>
            <Grid>
                <Grid.Col span={{ base: 12, md: 6 }} p={50}>
                    <Group justify="center" align="center">
                        <Stack gap={25}>
                            <Avatar 
                                src={user.profilePicPath ? `${BACKEND_URL}/${user.profilePicPath}` : null}
                                w={150}
                                h={150}
                            />
                            
                            <Group justify="center" gap={10}>
                                <FileButton onChange={(file) => file && updateProfilePicMutation.mutate(file)} accept="image/*">
                                    {(props) => (
                                        <ActionIcon 
                                            {...props}
                                            variant="light" 
                                            color="var(--button)"
                                            loading={updateProfilePicMutation.isPending}
                                            style={{ flexShrink: 0 }}
                                        >
                                            <IconUpload />
                                        </ActionIcon>
                                    )}
                                </FileButton>

                                <ActionIcon 
                                    variant="light" 
                                    color="red"
                                    loading={updateProfilePicMutation.isPending}
                                    onClick={() => updateProfilePicMutation.mutate(null)}
                                    style={{ flexShrink: 0 }}
                                >
                                    <IconX />
                                </ActionIcon>
                            </Group>
                        </Stack>
                    </Group>
                </Grid.Col>

                <Grid.Col span={{ base: 12, md: 6 }}>
                    <AccountForm user={user} />
                </Grid.Col>
            </Grid>
        </Paper>
    )
}

export default AccountEdit;