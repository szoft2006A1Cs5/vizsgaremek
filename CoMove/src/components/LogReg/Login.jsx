import { Stack, Text, TextInput, PasswordInput, Button, Group } from "@mantine/core";
import CenteredCard, { blueInput, styles } from "../../components/common/CenteredCard/CenteredCard";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { useForm } from "@mantine/form";
import { REGEX } from "../../assets/scripts/Regex";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { notifications } from "@mantine/notifications";
import { fetchAPI, trimForm } from "../../assets/scripts/Utilities";

function Login({ style }) {
    const queryClient = useQueryClient();
    const navigate = useNavigate();
    const loginFrom = useForm({
        mode: "controlled",
        initialValues: {
            email: "",
            password: ""
        },
        validate: {
            email: ((v) => REGEX.email.test(v) ? null : "Hibás az e-mail cím formátuma!")
        }
    });

    const loginMutation = useMutation({
        mutationFn: async (credentials) => {
            const resp = await fetchAPI(`/Auth/Login`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(trimForm(credentials))
            });

            if (resp.status === 401) throw new Error("Hibás e-mail cím vagy jelszó!");
            if (!resp.ok) throw new Error("Váratlan hiba történt a bejelentkezéskor!");
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['authUser'] });
            navigate("/");
        },
        onError: (err) => notifications.show({ title: 'Hiba', message: err.message, color: 'red' }),
    });

    return (
        <form onSubmit={loginFrom.onSubmit((creds) => loginMutation.mutate(creds))}>
            <Stack style={{ ...style }} gap={15}>
                <Stack gap={5}>
                    <Text c="var(--background)" fw='bold' fz={30}>Bejelentkezés</Text>
                    <Text c="var(--lightpurple)" fz={15}>
                        Lépj be fiókodba, és folytasd, ahol abbahagytad.
                    </Text>
                </Stack>

                <TextInput
                    label="E-mail cím"
                    placeholder="pl. tesztelek@comove.app"
                    styles={blueInput} 
                    required={true}
                    {...loginFrom.getInputProps("email")}
                />

                <PasswordInput
                    label="Jelszó"
                    placeholder="********"
                    styles={blueInput} 
                    required={true}
                    {...loginFrom.getInputProps("password")}
                />

                <Group justify="center">
                    <Text 
                        component={Link} 
                        to="/forgotpassword" 
                        className={styles.hoverUp} 
                        c="var(--lightpurple)" 
                        fz={14} 
                        ta="center"
                    >
                        Elfelejtetted a jelszavad?
                    </Text>

                    <Button 
                        color="var(--button)" 
                        fz={17} 
                        radius='xl'
                        w='100%'
                        size='lg'
                        type="submit"
                        loading={loginMutation.isPending}
                    >
                        Bejelentkezés
                    </Button>

                    <Text 
                        component={Link} 
                        to="/register" 
                        className={styles.hoverUp} 
                        c="var(--button)" 
                        fz={14} 
                        ta="center"
                    >
                        Még nincs fiókod? Regisztálj!
                    </Text>
                </Group>
            </Stack>
        </form>
    )
}

export default Login;