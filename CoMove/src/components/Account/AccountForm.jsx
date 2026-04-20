import { useForm } from "@mantine/form";
import { checkOver18, getRespJsonError } from "../../assets/scripts/Utilities";
import { Button, Checkbox, Divider, Group, PasswordInput, Stack, TextInput, Text } from "@mantine/core";
import { useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { API_URL } from "../../assets/scripts/Config";
import { DateInput, DatePicker, DatePickerInput } from "@mantine/dates";
import 'dayjs/locale/hu'
import { notifications } from "@mantine/notifications";

function AccountForm({ user }) {
    const queryClient = useQueryClient();
    const [newPassword, setNewPassword] = useState(false);

    const form = useForm({
        mode: "controlled",
        initialValues: {
            ...user,
            driversLicenseNumber: user.driversLicenseNumber ?? "",
            password: "",
            passwordAgain: "",
            previousPassword: ""
        },
        validate: {
            idCardNumber: ((v) => /^\d{6}[A-Z]{2}$/.test(v) ? null : "Érvénytelen személyi szám!"),
            name: ((v) => /^[A-ZÁÉÍÓÚÜŰÖŐ][a-záéíóúüűöő]+( [A-ZÁÉÍÓÚÜŰÖŐ][a-záéíóúüűöő]+)+$/.test(v) ? null : "Helytelen név!"),
            phone: ((v) => /^(36|06)(94|70|30|20)\d{7}$/.test(v) ? null : "Érvénytelen telefonszám!"),
            dateOfBirth: ((v) => checkOver18(v) ? null : "El kell múlnod 18 évesnek!"),
            email: ((v) => /^[A-z0-9.-]+@([A-z0-9-]+\.)+([A-z]{2,3})$/.test(v) ? null : "Érvénytelen e-mail formátum!"),
            password: ((v) => !newPassword || /^(?=.*[a-z])(?=.*\d)(?=.*[A-Z]).{8,}$/.test(v) ? null : "A 8 karakteres jelszónak tartalmaznia kell nagybetűt, kisbetűt és számot!"),
            passwordAgain: ((v, values) => !newPassword || v.trim() === values.password.trim() ? null : "A két új jelszó nem egyezik!"),
            driversLicenseNumber: ((v) => !v.trim() || /^[A-Z]{2}\d{6}$/.test(v) ? null : "Érvénytelen jogosítványszám!"),
            addressZipcode: ((v) => /^\d{4}$/.test(v) ? null : "Érvénytelen irányítószám!"),
            addressSettlement: ((v) => !!v.trim() ? null : "Nem adtál meg települést!"),
            addressStreetHouse: ((v) => !!v.trim() ? null : "Nem adtál meg utcát, házszámot!"),
            previousPassword: ((v) => !!v.trim() ? null : "Kötelező megadni a jelszót a beállítások frissítéséhez!")
        }
    });

    const saveMutation = useMutation({
        mutationFn: async (formSent) => {
            const resp = await fetch(`${API_URL}/User/${user.id}`, {
                method: "PUT",
                credentials: "include",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    ...formSent,
                    driversLicenseNumber: !formSent.driversLicenseNumber.trim() ? null : formSent.driversLicenseNumber.trim(),
                    password: newPassword ? formSent.password : null,
                })
            });

            if (resp.status === 403) {
                throw new Error("Hibás jelszó!");
            } else if (resp.status === 409) {
                throw new Error("Az adatok ütköznek más felhasználói adatokkal!");
            } else if (!resp.ok) {
                const respJson = await getRespJsonError(resp);
                throw new Error(respJson?.error ?? "Nem sikerült menteni az adatokat!");
            }
        },
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ["user", String(user.id)] }),
        onError: (error) => notifications.show({ title: "Hiba!", message: error.message, color: "red" }),
    })

    const handleSave = (values) => {
        const trimmedForm = Object.fromEntries(
            Object.entries(values).map(([key, val]) => {
                return [key, typeof val === "string" ? val.trim() : val]
            })
        );

        saveMutation.mutate(trimmedForm);
    }

    return (
        <form onSubmit={form.onSubmit(handleSave)}>
            <Stack align="right">
                <TextInput
                    label="Név"
                    placeholder="pl. Teszt Elek"
                    {...form.getInputProps("name")}
                />

                <TextInput
                    label="E-mail cím"
                    placeholder="pl. teszt@comove.app"
                    {...form.getInputProps("email")}
                />

                <TextInput
                    label="Telefonszám"
                    placeholder="pl. 36123456789"
                    {...form.getInputProps("phone")}
                />

                <DateInput
                    label="Születési dátum"
                    locale="hu"
                    valueFormat="YYYY. MM. DD."
                    {...form.getInputProps("dateOfBirth")}
                />

                <TextInput
                    label="Személyi igazolvány szám"
                    placeholder="pl. 123456AB"
                    {...form.getInputProps("idCardNumber")}
                />

                <TextInput
                    label="Jogosítványszám"
                    placeholder="pl. AB123456"
                    {...form.getInputProps("driversLicenseNumber")}
                />

                <Group justify="space-between" wrap="nowrap">
                    <TextInput
                        label="Irányítószám"
                        placeholder="pl. 9700"
                        w="30%"
                        {...form.getInputProps("addressZipcode")}
                    />

                    <TextInput
                        label="Település"
                        placeholder="pl. Szombathely"
                        {...form.getInputProps("addressSettlement")}
                        w="70%"
                    />
                </Group>

                <TextInput
                    label="Utca, házszám"
                    placeholder="pl. Zrínyi Ilona utca 12."
                    {...form.getInputProps("addressStreetHouse")}
                />

                <Stack gap={0}>
                    <Text fz="var(--mantine-font-size-sm)" fw={500}>Új jelszó</Text>
                    <Group justify="space-between" align="center" wrap="nowrap">
                        <Checkbox 
                            checked={newPassword} 
                            size={34}
                            onChange={(e) => setNewPassword(e.target.checked)} 
                            style={{ flexShrink: 0, cursor: "pointer" }}
                        />

                        <PasswordInput
                            flex={1}
                            disabled={!newPassword}
                            {...form.getInputProps("password")}
                        />
                    </Group>
                </Stack>

                { newPassword && (
                    <PasswordInput
                        label="Új jelszó megerősítése"
                        {...form.getInputProps("passwordAgain")}
                    />
                 )
                }

                <Divider />

                <PasswordInput
                    label="Régi jelszó"
                    {...form.getInputProps("previousPassword")}
                />

                <Group justify="flex-end">
                    <Button 
                        type="submit"
                        color="var(--button)"
                        radius='md'
                        loading={saveMutation.isPending}
                    >
                        Mentés
                    </Button>
                </Group>
            </Stack>
        </form>
    )
}

export default AccountForm;