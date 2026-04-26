import { Divider, Grid, Group, NavLink, Paper, Stack, Text } from "@mantine/core";
import { IconBrandFacebook, IconBrandInstagram, IconBrandTiktok, IconContract, IconLocationPin, IconMail, IconMapPin, IconMapPin2, IconPhone, IconPin } from "@tabler/icons-react";

function Footer() {
    return (
        <Paper w='100%' p={50} radius={0} style={{
            backgroundColor: 'var(--background)'
        }}>
            <Stack gap={15}>
                <Grid mb={15}>
                    <Grid.Col span={{ base: 12, sm: 4 }}>
                        <Stack>
                            <Text c='white' fw='bold' fz={25}>
                                Kapcsolat
                            </Text>
                            <Group justify="flex-start" wrap="nowrap">
                                <IconMapPin color="white" />
                                <Text c='dimmed'>9700 Szombathely, Zrínyi Ilona utca 12.</Text>
                            </Group>
                            <Group justify="flex-start" wrap="nowrap">
                                <IconPhone color="white" />
                                <Text c='dimmed'>+36 70 123 4567</Text>
                            </Group>
                            <Group justify="flex-start" wrap="nowrap">
                                <IconMail color="white" />
                                <Text 
                                    component="a" 
                                    c='dimmed' 
                                    href="mailto:contact@comove.app"
                                >
                                    contact@comove.app
                                </Text>
                            </Group>
                        </Stack>
                    </Grid.Col>
                    <Grid.Col span={{ base: 12, sm: 4 }}>
                        <Stack>
                            <Text c='white' fw='bold' fz={25}>
                                Kövess minket
                            </Text>
                            <Group 
                                component='a' 
                                justify="flex-start" 
                                wrap="nowrap" 
                                href="https://www.facebook.com/profile.php?id=61586242866516"
                                target="_blank"
                                td='none'
                            >
                                <IconBrandFacebook color="white" />
                                <Text c='dimmed'>
                                    Facebook
                                </Text>
                            </Group>
                            <Group 
                                component='a' 
                                justify="flex-start" 
                                wrap="nowrap" 
                                href="https://instagram.com"
                                target="_blank"
                                td='none'
                            >
                                <IconBrandInstagram color="white" />
                                <Text c='dimmed'>
                                    Instagram
                                </Text>
                            </Group>
                            <Group 
                                component='a' 
                                justify="flex-start" 
                                wrap="nowrap" 
                                href="https://tiktok.com"
                                target="_blank"
                                td='none'
                            >
                                <IconBrandTiktok color="white" />
                                <Text c='dimmed'>
                                    TikTok
                                </Text>
                            </Group>
                        </Stack>
                    </Grid.Col>
                    <Grid.Col span={{ base: 12, sm: 4 }}>
                        <Stack>
                            <Text c='white' fw='bold' fz={25}>
                                Bérlési szerződés
                            </Text>
                            <Group 
                                component='a' 
                                justify="flex-start" 
                                wrap="nowrap" 
                                href="/CoMoveBerlesiSzerzodes.pdf"
                                target="_blank"
                                td='none'
                            >
                                <IconContract color="white" />
                                <Text c='dimmed'>
                                    Kitölthető bérlési szerződés a felek közt
                                </Text>
                            </Group>
                        </Stack>
                    </Grid.Col>
                </Grid>

                <Divider color="var(--button)" />

                <Group gap={10} mt={15} justify="flex-start" align="center">
                    <Text c='dimmed' fz={12}>
                        © {new Date().getFullYear()}
                    </Text>
                    <Text c='white' fw='bold'>
                        CoMove
                    </Text>
                    <Text c='dimmed' fz={12}>
                        Minden jog fenntartva. - Ez nem egy valódi cég/szolgáltatás, hanem egy technikumi vizsgaremek.
                    </Text>
                </Group>
            </Stack>
        </Paper>
    )
}

export default Footer;