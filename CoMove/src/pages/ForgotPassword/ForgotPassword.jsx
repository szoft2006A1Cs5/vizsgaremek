import { useEffect, useState } from "react";
import { useUser } from "../../assets/scripts/hooks/AuthUser";
import { Link, useNavigate } from "react-router-dom";
import { Flex, Paper, Text, Image, Stack, TextInput, NavLink, Button } from "@mantine/core";
import logo from "../../assets/kepek/logo/comove_logo1.png";
import { REGEX } from "../../assets/scripts/Regex";
import { useMutation } from "@tanstack/react-query";
import { fetchAPI } from "../../assets/scripts/Utilities";
import { notifications } from "@mantine/notifications";
import CenteredCard, { blueInput, styles } from "../../components/common/CenteredCard/CenteredCard";

function ForgotPassword() {
    const navigate = useNavigate();
    const { data: authUser, isSuccess, isError } = useUser();
    const [emailData, setEmailData] = useState({ email: "" });

    useEffect(() => {
        if (isSuccess && authUser)
            navigate("/")
    }, [authUser, isSuccess, isError])

    const forgotPasswordMutation = useMutation({
        mutationFn: async () => {
            const resp = await fetchAPI(`/Auth/ForgotPassword?email=${emailData.email.trim()}`, {
                method: "POST",
            })

            if (!resp.ok) {
                const respJson = await resp.json().catch(() => {});
                throw new Error(respJson?.error ?? "Nem sikerült visszaállító e-mail küldeni a megadott e-mail címre!");
            }
        },
        onSuccess: () => notifications.show({ 
            title: "Elküldve", 
            message: "Amennyiben a megadott e-mail cím létezik a rendszerben, elküldtük rá a visszaállító kódot!", 
            color: "green" 
        }),
        onError: (error) => notifications.show({ title: "Hiba!", message: error.message, color: "red" })
    })

    const tryForgot = () => {
        if (!REGEX.email.test(emailData.email.trim())) {
            setEmailData({
                ...emailData,
                error: "Nem megfelelő az e-mail cím formátuma!"
            });
            return;
        }

        setEmailData({
            ...emailData,
            error: null
        });

        forgotPasswordMutation.mutate();
    }

    return (
        <CenteredCard>
            <Stack gap={15}>
                <Text c="var(--background)" fw='bold' fz={30}>Elfelejtetted a jelszavad?</Text>
                <Text c="var(--lightpurple)" fz={15}>
                    Írd be a fiókodhoz tartozó e-mail címet, és küldünk egy visszaállító kódot!
                </Text>
                
                <TextInput 
                    label="E-mail cím"
                    placeholder="pl. teszt@comove.app"
                    value={emailData.email}
                    onInput={(e) => setEmailData({ ...emailData, email: e.target.value })}
                    error={emailData.error}
                    styles={blueInput}
                    required={true} />
                
                <Button 
                    color="var(--button)" 
                    fz={15} 
                    radius='xl'
                    w='100%'
                    onClick={() => tryForgot()}
                    loading={forgotPasswordMutation.isPending}
                >
                    Visszaállító e-mail küldése
                </Button>
                
                <Text 
                    component={Link} 
                    to="/resetpassword" 
                    className={styles.hoverUp} 
                    c="var(--lightpurple)" 
                    fz={12} 
                    ta="center"
                >
                    Már van kódom
                </Text>
            </Stack>
        </CenteredCard>
    )
}

export default ForgotPassword;