import { Stack, Text } from "@mantine/core";
import CenteredCard from "../../components/common/CenteredCard/CenteredCard";
import { useNavigate } from "react-router-dom";
import { useEffect } from "react";

function Page404() {
    const navigate = useNavigate();

    useEffect(() => {
        setTimeout(() => {
            navigate('/')
        }, 3000);
    }, [])

    return (
        <CenteredCard>
            <Stack>
                <Text c="var(--background)" fw='bold' fz={30}>404</Text>
                <Text c="var(--lightpurple)" fz={15}>
                    Az oldal nem található. Hamarosan visszairányítjuk a főoldalra...
                </Text>
            </Stack>
        </CenteredCard>
    )
}

export default Page404;