import { useEffect, useState } from "react";
import { useUser } from "../../assets/scripts/AuthUser";
import { Link, useNavigate } from "react-router-dom";
import { Flex, Paper, Text, Image, Stack, TextInput, NavLink, Button } from "@mantine/core";
import logo from "../../assets/kepek/logo/comove_logo1.png";
import style from './ForgotPassword.module.css'
import { REGEX } from "../../assets/scripts/Regex";
import { useMutation } from "@tanstack/react-query";
import { fetchAPI } from "../../assets/scripts/Utilities";
import { notifications } from "@mantine/notifications";

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
            const resp = await fetchAPI(`/Auth/ForgotPassword?email=${emailData.email}`, {
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
        if (!REGEX.email.test(emailData.email)) {
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
        <Flex mih='100vh' justify='center' align='center' bg='var(--lightbackground)'>
            <Paper p={40} radius='xl' w='100%' maw={500} shadow='md' m='5%'>
                <Image src={logo} w={45} h={45} onClick={() => navigate("/")} style={{ cursor: "pointer" }} mb={25} />
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
                        styles={{ input: { backgroundColor: "var(--lightbackground)" } }} />
                    
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
                        className={style.hasCode} 
                        c="var(--lightpurple)" 
                        fz={12} 
                        ta="center"
                    >
                        Már van kódom
                    </Text>
                </Stack>
            </Paper>
        </Flex>
    )
}

export default ForgotPassword;