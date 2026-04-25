import { Stack, Text, TextInput, PasswordInput, Button, Group, Stepper } from "@mantine/core";
import CenteredCard, { blueInput, styles } from "../../components/common/CenteredCard/CenteredCard";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { useState } from "react";
import { useForm } from "@mantine/form";
import { REGEX } from "../../assets/scripts/Regex";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { notifications } from "@mantine/notifications";
import { checkOver18, fetchAPI, trimForm } from "../../assets/scripts/Utilities";
import { useDateInputProps } from "../../assets/scripts/hooks/Hooks";
import { IconHome, IconMail, IconPhone, IconUser } from "@tabler/icons-react";
import { DatePickerInput } from "@mantine/dates";
import 'dayjs/locale/hu'
import dayjs from "dayjs";

function Register({ style }) {
    const queryClient = useQueryClient();
    const navigate = useNavigate();
    const [step, setStep] = useState(0);
    const datePickerProps = useDateInputProps('date');
    const iconVerticalMiddleStyle = { display: 'block', margin: 'auto' };
    const registerForm = useForm({
        mode: "controlled",
        initialValues: {
            name: "",
            dateOfBirth: dayjs().format("YYYY-MM-DD"),
            idCardNumber: "",
            driversLicenseNumber: "",
            addressZipcode: "",
            addressSettlement: "",
            addressStreetHouse: "",
            phone: "",
            email: "",
            password: "",
            passwordAgain: ""
        },
        validate: {
            name: ((v) => REGEX.name.test(v.trim()) ? null : "Nem megfelelő a név formátuma!"),
            dateOfBirth: ((v) => checkOver18(v) ? null : "Nem múltál el 18 éves!"),
            idCardNumber: ((v) => 
                REGEX.idCardNumber.test(v.trim().toUpperCase()) 
                    ? null 
                    : "Nem megfelelő a személyi igazolvány számának formátuma!"
            ),
            driversLicenseNumber: ((v) => 
                !v.trim() || REGEX.driversLicenseNumber.test(v.trim().toUpperCase()) 
                    ? null 
                    : "Nem megfelelő a vezetői engedély számának formátuma!"
            ),
            addressZipcode: ((v) => REGEX.addressZipcode.test(v.trim()) ? null : "Nem megfelelő az irányítószám formátuma!"),
            addressSettlement: ((v) => v.trim(v) ? null : "Nem adtál meg települést!"),
            addressStreetHouse: ((v) => v.trim(v) ? null : "Nem adtál meg címet!"),
            phone: ((v) => REGEX.phone.test(v.trim()) ? null : "Nem megfelelő a telefonszám formátuma!"),
            email: ((v) => REGEX.email.test(v.trim()) ? null : "Nem megfelelő az e-mail cím formátuma!"),
            password: ((v) => 
                REGEX.password.test(v.trim()) 
                    ? null 
                    : "A 8 karakteres jelszónak tartalmaznia kell nagybetűt, kisbetűt és számot!"
            ),
            passwordAgain: ((v, values) => v.trim() === values.password.trim() ? null : "A két jelszó nem egyezik!")
        },
        transformValues: (values) => ({
            ...values,
            idCardNumber: values.idCardNumber.toUpperCase(),
            driversLicenseNumber: values.driversLicenseNumber.toUpperCase()
        })
    })

    const registerMutation = useMutation({
        mutationFn: async (registerData) => {
            const resp = await fetchAPI(`/Auth/Register`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(trimForm(registerData))
            });

            if (resp.status === 409) 
                throw new Error("A megadott adatok (pl. személyi szám, jogosítványszám, telefonszám, email) ütköznek más, " +
                                "a rendszerben lévő, adatokkal!");
            
            if (!resp.ok) {
                const respJson = await resp.json().catch(() => {})
                throw new Error(respJson?.error ?? "Váratlan hiba történt a regisztráció során!");
            }
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['authUser'] });
            navigate("/");
        },
        onError: (err) => notifications.show({ title: 'Hiba', message: err.message, color: 'red' }),
    })

    const fieldsPerStep = [['name', 'dateOfBirth', 'idCardNumber', 'driversLicenseNumber'],
                           ['addressZipcode', 'addressSettlement', 'addressStreetHouse'],
                           ['phone', 'email', 'password', 'passwordAgain']]

    const tryNext = () => {
        if (fieldsPerStep[step]
            .map(x => registerForm.validateField(x))
            .every(x => !x.hasError)) {
                step === 2 ? registerMutation.mutate(registerForm.getTransformedValues()) : setStep(step + 1);
            }
    }

    const trySet = (i) => {
        if (step < i)
            return;

        setStep(i);
    }

    return (
        <Stack style={{ ...style }} gap={15}>
            <Stack gap={5}>
                <Text c="var(--background)" fw='bold' fz={30}>Regisztráció</Text>
                <Text c="var(--lightpurple)" fz={15}>
                    Hozd létre pár lépésben fiókod, és csatlakozz a CoMove közösségéhez!
                </Text>
            </Stack>

            <Stepper 
                size="md" 
                iconSize={30} 
                active={step} 
                color="var(--button)"
                onStepClick={(i) => trySet(i)}
            >
                <Stepper.Step 
                    icon={<IconUser color="var(--button)" size={15} style={iconVerticalMiddleStyle} />}
                    completedIcon={<IconUser color="white" size={15} />}
                >
                    <Stack gap={5}>
                        <TextInput 
                            label="Név"
                            placeholder="pl. Teszt Elek"
                            required={true}
                            styles={blueInput}
                            maxLength={64}
                            {...registerForm.getInputProps("name")}
                        />

                        <DatePickerInput 
                            label="Születési dátum"
                            placeholder="pl. Teszt Elek"
                            required={true}
                            styles={blueInput}
                            {...datePickerProps}
                            {...registerForm.getInputProps("dateOfBirth")}
                        />

                        <TextInput 
                            label="Személyi igazolvány száma"
                            placeholder="pl. 123456AA"
                            required={true}
                            styles={{
                                input: { 
                                    ...blueInput.input,
                                    textTransform: 'uppercase' 
                                }
                            }}
                            maxLength={8}
                            {...registerForm.getInputProps("idCardNumber")}
                        />

                        <TextInput 
                            label="Vezetői engedély száma (opcionális)"
                            placeholder="pl. AA123456"
                            styles={{
                                input: { 
                                    ...blueInput.input,
                                    textTransform: 'uppercase' 
                                }
                            }}
                            maxLength={8}
                            {...registerForm.getInputProps("driversLicenseNumber")}
                        />
                    </Stack>
                </Stepper.Step>
                <Stepper.Step
                    icon={<IconHome color="var(--button)" size={15} style={iconVerticalMiddleStyle} />}
                    completedIcon={<IconHome color="white" size={15} />}
                >
                    <Stack gap={5}>
                        <TextInput 
                            label="Irányítószám"
                            placeholder="pl. 9700"
                            required={true}
                            styles={blueInput}
                            maxLength={4}
                            {...registerForm.getInputProps("addressZipcode")}
                        />

                        <TextInput 
                            label="Település"
                            placeholder="pl. Szombathely"
                            required={true}
                            styles={blueInput}
                            maxLength={64}
                            {...registerForm.getInputProps("addressSettlement")}
                        />

                        <TextInput 
                            label="Utca, házszám"
                            placeholder="pl. Zrínyi Ilona utca 12."
                            required={true}
                            styles={blueInput}
                            maxLength={64}
                            {...registerForm.getInputProps("addressStreetHouse")}
                        />
                    </Stack>
                </Stepper.Step>
                <Stepper.Step
                    icon={<IconMail color="var(--button)" size={15} style={iconVerticalMiddleStyle} />}
                    completedIcon={<IconMail color="white" size={15} />}
                >
                    <Stack gap={5}>
                        <TextInput 
                            label="Telefonszám"
                            placeholder="pl. 36123456789"
                            required={true}
                            styles={blueInput}
                            maxLength={11}
                            {...registerForm.getInputProps("phone")}
                        />

                        <TextInput 
                            label="E-mail cím"
                            placeholder="pl. tesztelek@comove.app"
                            required={true}
                            styles={blueInput}
                            maxLength={64}
                            {...registerForm.getInputProps("email")}
                        />

                        <PasswordInput
                            label="Jelszó"
                            placeholder="········"
                            required={true}
                            styles={blueInput}
                            {...registerForm.getInputProps("password")}
                        />

                        <PasswordInput
                            label="Jelszó mégegyszer"
                            placeholder="········"
                            required={true}
                            styles={blueInput}
                            {...registerForm.getInputProps("passwordAgain")}
                        />
                    </Stack>
                </Stepper.Step>
            </Stepper>

            <Group justify="center" mt={10}>
                <Button 
                    color="var(--button)" 
                    fz={17} 
                    radius='xl'
                    w='100%'
                    size='lg'
                    onClick={() => tryNext()}
                    loading={registerMutation.isPending}
                >
                    {step === 2 ? "Regisztráció" : "Következő"}
                </Button>

                <Text 
                    component={Link} 
                    to="/login" 
                    className={styles.hoverUp} 
                    c="var(--button)" 
                    fz={14} 
                    ta="center"
                >
                    Már van fiókod? Jelentkezz be!
                </Text>
            </Group>
        </Stack>
    )
}

export default Register;