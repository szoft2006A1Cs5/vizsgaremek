import { Button, ActionIcon, Modal, Center, Text, ScrollArea } from "@mantine/core"
import { useDisclosure, useMediaQuery } from '@mantine/hooks';
import Notification from '../Notification/Notification'
import { useUser } from '../../assets/scripts/AuthUser';
import { API_URL } from '../../assets/scripts/Config';
import { FaBell } from 'react-icons/fa';
import { notifications } from "@mantine/notifications";

function NotificationMenu() {
    const [opened, { open, close }] = useDisclosure(false);
    const isMobile = useMediaQuery('(max-width: 50em)');

    const { data: authUser, isLoading: isLoading, isSuccess: isSuccess} = useUser();

    function deleteNotificaton(id) {
        const auth = JSON.parse(localStorage.getItem("auth"));
        fetch(`${API_URL}/User/${auth.userId}/Notification/${id}`, {
            method: "DELETE",
            headers: { 
                "Content-Type": "application/json",
                "Authorization": `Bearer ${auth.token}` 
            },
        })
        .then((resp) => {
            if (resp.status === 204)
                authUser.refetch();
            else
                throw new Error("Nem sikerült törölni az értesítést!")
        })
        .catch(err => {
            notifications.show({
                title: "Hiba!",
                message: err,
                style: {
                    backgroundColor: 'red'
                }
            })
        });
    }

    function deleteAll() {
        const auth = JSON.parse(localStorage.getItem("auth"));
        fetch(`${API_URL}/User/${auth.userId}/Notification`, {
            method: "DELETE",
            headers: { 
                "Content-Type": "application/json",
                "Authorization": `Bearer ${auth.token}` 
            },
        })
        .then((resp) => {
            if (resp.status === 204)
                authUser.refetch();
            else
                throw new Error("Nem sikerült törölni az értesítéseket!")
        })
        .catch(err => {
            notifications.show({
                title: "Hiba!",
                message: err,
                style: {
                    backgroundColor: 'red'
                }
            })
        });
    }

    if (!isSuccess)
        return <></>

    return (
        <>
            <ActionIcon onClick={open} color='transparent'>
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
                        onClick={() => deleteAll()} 
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
                                    onDelete={() => deleteNotificaton(x.notificationId)} 
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
