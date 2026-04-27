import { Center, Loader, Paper, ScrollArea, Stack, Text } from "@mantine/core";

export function AdminListRow({ children, onClick }) {
    return (
        <Paper 
            radius='md' 
            p='md' 
            withBorder
            onClick={onClick}
            style={{ cursor: "pointer" }}
        >
            {children}
        </Paper>
    )
}

function AdminList({ children, title, loading = true, error }) {
    return (
        <Paper radius='md' p='md'>
            <Stack gap={10}>
                <Text c='var(--background)' fz={15} fw='bold'>{title}</Text>
                { error 
                    ? <Center p={10}><Text color="var(--lightpurple)">{error.message}</Text></Center>
                    : (
                        loading 
                        ? <Center p={10}><Loader color="var(--background)" size={20} /></Center>
                        : (
                            <ScrollArea h={400}>
                                {children}
                            </ScrollArea>
                        )
                    ) 
                }
            </Stack>
        </Paper>
    )
}

export default AdminList;