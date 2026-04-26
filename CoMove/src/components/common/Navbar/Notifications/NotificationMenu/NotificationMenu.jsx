import { Button, ActionIcon, Modal, Center, Text, ScrollArea } from "@mantine/core"
import { useDisclosure, useMediaQuery } from '@mantine/hooks';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import Notification from '../Notification/Notification'
import { useUser } from '../../../../../assets/scripts/hooks/AuthUser';
import { API_URL } from '../../../../../assets/scripts/Config';
import { FaBell } from 'react-icons/fa';
import { notifications } from "@mantine/notifications";
import { fetchAPI } from "../../../../../assets/scripts/Utilities";
import { useEffect } from "react";
import { useLocation } from "react-router-dom";

function NotificationMenu() {
    const [opened, { open, close }] = useDisclosure(false);
    const isMobile = useMediaQuery('(max-width: 50em)');
    const qc = useQueryClient();
    const location = useLocation();

    useEffect(() => {
        close();
    }, [location])

    const { data: authUser, isLoading: isLoading } = useUser();

    const deleteMutation = useMutation({
        mutationFn: async (id) => {
            const resp = await fetchAPI(`/User/${authUser.id}/Notification/${id}`, {
                method: "DELETE",
            });

            if (!resp.ok) throw new Error("Nem sikerült törölni az értesítést!");
        },
        onSuccess: () => qc.invalidateQueries({ queryKey: ['authUser'] }),
        onError: (err) => notifications.show({ title: 'Hiba', message: err.message, color: 'red' }),
    });

    const deleteAllMutation = useMutation({
        mutationFn: async () => {
            const resp = await fetchAPI(`/User/${authUser.id}/Notification`, {
                method: "DELETE",
            });

            if (!resp.ok) throw new Error("Nem sikerült törölni az értesítéseket!");
        },
        onSuccess: () => qc.invalidateQueries({ queryKey: ['authUser'] }),
        onError: (err) => notifications.show({ title: 'Hiba', message: err.message, color: 'red' }),
    });

    if (!authUser)
        return <></>

    return (
        <>
            <ActionIcon 
                onClick={open} 
                color={!isLoading && authUser && authUser.notifications.length ? 'red' : 'transparent'}
                bd='1px solid white'
                bdrs={10}
            >
                <FaBell />
            </ActionIcon>

            <Modal 
                opened={opened} 
                onClose={close} 
                yOffset={isMobile ? 0 : 150}
                fullScreen={isMobile}
                styles={{
                    content: {
                        marginTop: isMobile ? 80 : 0,
                    },
                    body: {
                        overflow: 'hidden',
                        marginBottom: isMobile ? 50 : 0
                    }
                }}
            >
                <Modal.Body>
                    <Button
                        color='red'
                        onClick={() => deleteAllMutation.mutate()}
                        loading={deleteAllMutation.isPending}
                        disabled={authUser.notifications.length <= 0}
                        mb={30}
                        fullWidth
                    >
                        Összes törlése
                    </Button>
                    { 0 < authUser.notifications.length ?
                        authUser.notifications.map(x => {
                            return (
                                <Notification 
                                    key={x.notificationId} 
                                    notification={x} 
                                    onDelete={() => deleteMutation.mutate(x.notificationId)}
                                />
                            )
                        })
                    :
                        <Center>
                            <Text>
                                Nincsenek értesítései
                            </Text>
                        </Center>
                    }
                </Modal.Body>
            </Modal>
        </>
    )
}

export default NotificationMenu;
