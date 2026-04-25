import { ActionIcon, Button, Center, FileButton, Group, Loader, Paper, ScrollArea, Stack, Text, TextInput, Image } from "@mantine/core";
import style from './RentalChat.module.css'
import { IconCamera, IconSend } from "@tabler/icons-react";
import { useEffect, useRef, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { API_URL, BACKEND_URL } from "../../../assets/scripts/Config";
import { useUser } from "../../../assets/scripts/hooks/AuthUser";
import { fetchAPI, formatDateTime, formatPic } from "../../../assets/scripts/Utilities";
import { notifications } from "@mantine/notifications";

function Message({ message, me }) {
    const isMe = me.id === message?.senderId;

    return (
        <Group justify={isMe ? "flex-end" : "flex-start"} style={{ maxWidth: "50%" }} ml={isMe ? "auto" : null}>
            <Stack gap={3} p={10}>
                <Text c='var(--lightpurple)' fz={10} ml={5}>{message.sender.name}</Text>
                <Group bg={isMe ? 'var(--button)' : "lightgray"} w="100%" p={10} bdrs={10}>
                    <Stack gap={1}>
                        { message?.isImage ? 
                            <Image src={formatPic(message.content)} />
                        : 
                            <Text c={isMe ? 'white' : "var(--background)"} fz={12}>{message.content}</Text>
                        }
                        <Text c={isMe ? 'dimmed' : "var(--lightpurple)"} ta={isMe ? "right" : "left"} fz={10}>{formatDateTime(message.timeSent)}</Text>
                    </Stack>
                </Group>
            </Stack>
        </Group>
    );
}

function RentalChat({ rentalId }) {
    const queryClient = useQueryClient();
    const [msgSent, setMsgSent] = useState("");
    const { data: authUser } = useUser();
    const viewportRef = useRef(null);

    const { data: messages, isLoading, error, isError } = useQuery({
        queryKey: ["rental-messages", rentalId],
        queryFn: async () => {
            const resp = await fetchAPI(`/Rental/${rentalId}/Message`);

            if (!resp.ok)
                throw new Error("Nem sikerült betölteni az üzeneteket!");

            return resp.json();
        },
        refetchInterval: 5000
    })

    const sendMessageMutation = useMutation({
        mutationFn: async () => {
            const resp = await fetchAPI(`/Rental/${rentalId}/Message`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    content: msgSent.trim(),
                    isComplaint: false
                })
            })

            if (!resp.ok)
                throw new Error("Hiba az üzenet küldésekor!");
        },
        onSuccess: () => {
            setMsgSent("");
            queryClient.invalidateQueries({ queryKey: ["rental-messages", rentalId] });
        },
        onError: (error) => notifications.show({ title: "Hiba!", message: error.message, color: "red" })
    })

    const sendImageMessageMutation = useMutation({
        mutationFn: async (file) => {
            const formData = new FormData();
            formData.append('file', file);

            const resp = await fetchAPI(`/Rental/${rentalId}/Message/Image`, {
                method: "POST",
                body: formData,
            });

            if (!resp.ok)
                throw new Error("Nem sikerült feltölteni a képet!");
        },
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ["rental-messages", rentalId] }),
        onError: (error) => notifications.show({ title: "Hiba!", message: error.message, color: "red" })
    });

    useEffect(() => {
        viewportRef.current?.scrollTo({ top: viewportRef.current.scrollHeight })
    }, [messages])

    return (
        <Paper p='md' radius='md' shadow="0 8px 10px rgba(0, 0, 0, 0.48)">
            <Stack gap={15}>
                <Text c='var(--background)' fz={18} fw='bold'>Üzenetek</Text>

                <Stack>
                    <ScrollArea viewportRef={viewportRef} className={style.chatArea} bdrs='sm'>
                        {isLoading ? (
                            <Center p={50}><Loader color="var(--background)" /></Center>
                        )
                        : (
                            !isError ? (
                                (messages.length === 0 ?
                                    <Group justify="center" align="center" p={30}>
                                        <Text c='dimmed' fz={14}>Itt egyeztethetitek a bérléssel kapcsolatos információkat...</Text>
                                    </Group>
                                :
                                    <Stack gap={1} p={10}>
                                        {messages.map((msg, i) => <Message message={msg} me={authUser} key={i} />)}
                                    </Stack>
                                )
                            ) : (
                                <Group justify="center" align="center" p={50}>
                                    <Text c='var(--background)' fw='bold'>{error?.message ?? "Nem sikerült betölteni az üzeneteket!"}</Text>
                                </Group>
                            )
                        )
                        }
                    </ScrollArea>

                    <Group justify="space-around">
                        <FileButton onChange={(file) => file && sendImageMessageMutation.mutate(file)} accept="image/*">
                            {(props) => (
                                <ActionIcon
                                    {...props}
                                    size='lg'
                                    radius='xl'
                                    color="var(--button)"
                                    style={{ flexShrink: 0 }}
                                    loading={sendImageMessageMutation.isPending}
                                >
                                    <IconCamera size={20} style={{ flexShrink: 0 }} />
                                </ActionIcon>
                            )}
                        </FileButton>

                        <TextInput 
                            flex={1}
                            radius='xl'
                            placeholder="Írj üzenetet!" 
                            value={msgSent}
                            onInput={(e) => setMsgSent(e.target.value)} 
                            onKeyDown={(e) => {
                                if (e.key === "Enter" && !e.shiftKey && msgSent.trim()) {
                                    e.preventDefault();
                                    sendMessageMutation.mutate();
                                }
                            }}
                        />

                        <ActionIcon
                            size='lg'
                            radius='xl'
                            color="var(--button)"
                            disabled={!msgSent.trim()}
                            style={{ flexShrink: 0 }}
                            loading={sendMessageMutation.isPending}
                            onClick={() => sendMessageMutation.mutate()}
                        >
                            <IconSend size={20} style={{ flexShrink: 0 }} />
                        </ActionIcon>
                    </Group>
                </Stack>
            </Stack>
        </Paper>
    )
}

export default RentalChat;
