import { useState } from 'react'
import { Stack, ActionIcon, Group, Text, Box } from '@mantine/core';
import { IconTrash } from '@tabler/icons-react';
import style from './Notification.module.css'

function Notification({ notification, onDelete }) {
    const [notif] = useState(notification);

    return (
        <Box p="md" className={style.notifBox} >
            <Group wrap="nowrap" align="center" gap={0}>
                <Stack gap={2} style={{ flex: 3 }}>
                    <Text className={style.notifText}>
                        {notif.content}
                    </Text>
                    <Text className={style.notifDate}>
                        {new Intl.DateTimeFormat(navigator.language).format(new Date(notif.timeSent))}
                    </Text>
                </Stack>

                <Group gap={5} style={{ flex: 1 }} justify="flex-end" wrap="nowrap">
                    <ActionIcon color="red" onClick={onDelete}>
                        <IconTrash size={18} />
                    </ActionIcon>
                </Group>
            </Group>
        </Box>
  );
}

export default Notification;