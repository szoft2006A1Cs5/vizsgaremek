import { useEffect } from "react";
import { useUser } from "../../assets/scripts/hooks/AuthUser";
import { Link, useNavigate, useSearchParams } from "react-router-dom";
import { Flex, Paper, Text, Image, Stack, TextInput, Button, PasswordInput } from "@mantine/core";
import logo from "../../assets/kepek/logo/comove_logo1.png";
import { REGEX } from "../../assets/scripts/Regex";
import { useMutation } from "@tanstack/react-query";
import { fetchAPI } from "../../assets/scripts/Utilities";
import { notifications } from "@mantine/notifications";
import { useForm } from "@mantine/form";
import CenteredCard, { blueInput, styles } from "../../components/common/CenteredCard/CenteredCard";

function ResetPassword() {
    const [queryParams] = useSearchParams();
    const navigate = useNavigate();
    const { data: authUser, isSuccess, isError } = useUser();
    const resetForm = useForm({
        mode: "controlled",
        initialValues: {
            email: queryParams.get("email") ?? "",
            token: queryParams.get("code") ?? "",
            password: "",
            passwordAgain: "",
        },
        validate: {
            email: ((v) => REGEX.email.test(v.trim()) ? null : "Nem megfelelő az e-mail cím formátuma!"),
            token: ((v) => 
                v.trim() &&
                !isNaN(Number(v.trim())) && 
                v.trim().length === 6 
                    ? null 
                    : "Nem megfelelő a kód formátuma!"
            ),
            password: ((v) =>
                REGEX.password.test(v.trim())
                    ? null 
                    : "A jelszónak legalább 8 karakter hosszúnak kell lennie, tartalmaznia kell legalább 1 nagybetűt és 1 számot."
            ),
            passwordAgain: ((v, values) => v.trim() === values.password.trim() ? null : "A megadott jelszavak nem egyeznek!")
        }
    })

    useEffect(() => {
        if (isSuccess && authUser)
            navigate("/")
    }, [authUser, isSuccess, isError])

    const resetPasswordMutation = useMutation({
        mutationFn: async (formData) => {
            const resp = await fetchAPI('/Auth/ResetPassword', {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    email: formData.email.trim(),
                    token: formData.token.trim(),
                    password: formData.password.trim()
                })
            });

            if (!resp.ok) {
                const respJson = await resp.json().catch(() => {});
                throw new Error(respJson?.error ?? "Nem sikerült visszaállítani a jelszót!");
            }
        },
        onSuccess: () => {
            notifications.show({ title: "Sikeres visszaállítás!", message: "Az új jelszó sikeresen be lett állítva!", color: "green" })
            setTimeout(() => navigate("/"), 3000);
        },
        onError: (error) => notifications.show({ title: "Hiba!", message: error.message, color: "red" })
    })

    return (
        <CenteredCard>
            <form onSubmit={resetForm.onSubmit((formData) => resetPasswordMutation.mutate(formData))} >
                <Stack gap={15}>
                    <Text c="var(--background)" fw='bold' fz={30}>Új jelszó beállítása</Text>
                    <Text c="var(--lightpurple)" fz={15}>
                        Az új jelszó beállításához adja meg az e-mail címét és az arra kapott visszaállító kódot!
                    </Text>

                        <TextInput 
                            label="E-mail cím"
                            placeholder="pl. teszt@comove.app" 
                            styles={blueInput} 
                            required={true}
                            {...resetForm.getInputProps("email")}
                        />

                        <TextInput 
                            label="Visszaállító kód"
                            placeholder="pl. 123456" 
                            maxLength={6}
                            styles={blueInput} 
                            required={true}
                            {...resetForm.getInputProps("token")}
                        />

                        <PasswordInput 
                            label="Új jelszó"
                            placeholder="········" 
                            styles={blueInput} 
                            required={true}
                            {...resetForm.getInputProps("password")}
                        />

                        <PasswordInput 
                            label="Új jelszó megerősítése"
                            placeholder="········" 
                            styles={blueInput} 
                            required={true}
                            {...resetForm.getInputProps("passwordAgain")}
                        />

                        <Button 
                            color="var(--button)" 
                            fz={15} 
                            radius='xl'
                            type="submit"
                            w='100%'
                            loading={resetPasswordMutation.isPending}
                        >
                            Új jelszó beállítása
                        </Button>
                    
                    <Text 
                        component={Link} 
                        to="/forgotpassword" 
                        c="var(--lightpurple)" 
                        fz={12} 
                        ta="center"
                        className={styles.hoverUp}
                    >
                        Még nincs visszaállító kódom
                    </Text>
                </Stack>
            </form>
        </CenteredCard>
    )
}

export default ResetPassword;