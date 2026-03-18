import { useState, useEffect } from 'react'
import { Popover, Button, ActionIcon, Drawer, Modal, ScrollArea, Stack, Center, Text } from "@mantine/core"
import BellIcon from '../icons/Bell';
import { useDisclosure, useMediaQuery } from '@mantine/hooks';
import Notification from '../Notification/Notification'
import { useUser } from '../../assets/scripts/UseUser';

function NotificationMenu() {
    const [opened, { open, close }] = useDisclosure(false);
    const isMobile = useMediaQuery('(max-width: 50em)');

    const authUser = useUser();

    function Delete(id) {
        const auth = JSON.parse(localStorage.getItem("auth"));
        fetch(`https://localhost:7245/api/User/${auth.userId}/Notification/${id}`, {
            method: "DELETE",
            headers: { 
                "Content-Type": "application/json",
                "Authorization": `Bearer ${auth.token}` 
            },
        })
        .then((resp) => {
            if (resp.status === 204)
                authUser.refetch();
        })
        .catch(() => {});
    }

    if (!authUser.data)
        return <></>

    return (
        <>
            <ActionIcon onClick={open} color='transparent'>
                <BellIcon />
            </ActionIcon>

            <Modal 
                opened={opened} 
                onClose={close} 
                yOffset={isMobile ? 0 : 150}
                fullScreen={isMobile}
                styles={{
                    content: {
                        marginTop: isMobile ? 80 : 0
                    }
                }}
            >
                <Modal.Header>
                    <Button color='red' disabled={authUser.data.notifications.length <= 0} fullWidth>
                        Összes törlése
                    </Button>
                </Modal.Header>
                <Modal.Stack>
                    {
                    0 < authUser.data.notifications.length ?
                     authUser.data.notifications.map(x => {
                        return (
                            <Notification key={x.notificationId} notification={x} onDelete={() => Delete(x.notificationId)} />
                        )
                    }) :
                    <Center>
                        <Text>
                            Nincsenek értesítései
                        </Text>
                    </Center>
                    }
                </Modal.Stack>
            </Modal>
        </>
    )
}

export default NotificationMenu;
