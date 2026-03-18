import { useState, useEffect } from 'react'
import { Stack, ActionIcon, Group, Text, Box, Tooltip } from '@mantine/core';
import { IconCheck, IconTrash } from '@tabler/icons-react';

function Notification({ notification, onDelete }) {
    const [notif, setNotif] = useState(notification);

    return (
        <Box p="md" m={1} bg='lightgray' >
            <Group wrap="nowrap" align="center" gap={0}>
                <Stack gap={2} style={{ flex: 3 }}>
                    <Text 
                        fw={600} 
                        fz={18} 
                        size="sm"
                        style={{ 
                            wordBreak: 'break-word',
                            whiteSpace: 'normal'
                        }}
                    >
                        {notif.content}
                    </Text>
                    <Text size='xs' variant='dimmed'>
                        {new Intl.DateTimeFormat(navigator.language).format(new Date(notif.timeSent))}
                    </Text>
                </Stack>

                <Group gap={5} style={{ flex: 1 }} justify="flex-end" wrap="nowrap">
                    <ActionIcon variant="light" color="red" onClick={onDelete} size="lg">
                        <IconTrash size={18} />
                    </ActionIcon>
                </Group>
            </Group>
        </Box>
  );
}

export default Notification;