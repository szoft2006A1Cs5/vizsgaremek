import { useForm } from "@mantine/form";
import { checkOver18, fetchAPI, trimForm } from "../../assets/scripts/Utilities";
import { useDateInputProps } from "../../assets/scripts/hooks/Hooks";
import { Button, Checkbox, Divider, Group, PasswordInput, Stack, TextInput, Text } from "@mantine/core";
import { useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { API_URL } from "../../assets/scripts/Config";
import { DatePickerInput } from "@mantine/dates";
import 'dayjs/locale/hu'
import { notifications } from "@mantine/notifications";
import { REGEX } from "../../assets/scripts/Regex";

function AccountForm({ user }) {
    const queryClient = useQueryClient();
    const [newPassword, setNewPassword] = useState(false);
    const datePickerInputProps = useDateInputProps('date');

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
            idCardNumber: ((v) => REGEX.idCardNumber.test(v) ? null : "Érvénytelen személyi szám!"),
            name: ((v) => REGEX.name.test(v) ? null : "Helytelen név!"),
            phone: ((v) => REGEX.phone.test(v) ? null : "Érvénytelen telefonszám!"),
            dateOfBirth: ((v) => checkOver18(v) ? null : "El kell múlnod 18 évesnek!"),
            email: ((v) => REGEX.email.test(v) ? null : "Érvénytelen e-mail formátum!"),
            password: ((v) => !newPassword || REGEX.password.test(v) ? null : "A 8 karakteres jelszónak tartalmaznia kell nagybetűt, kisbetűt és számot!"),
            passwordAgain: ((v, values) => !newPassword || v.trim() === values.password.trim() ? null : "A két új jelszó nem egyezik!"),
            driversLicenseNumber: ((v) => !v.trim() || REGEX.driversLicenseNumber.test(v) ? null : "Érvénytelen jogosítványszám!"),
            addressZipcode: ((v) => REGEX.addressZipcode.test(v) ? null : "Érvénytelen irányítószám!"),
            addressSettlement: ((v) => !!v.trim() ? null : "Nem adtál meg települést!"),
            addressStreetHouse: ((v) => !!v.trim() ? null : "Nem adtál meg utcát, házszámot!"),
            previousPassword: ((v) => !!v.trim() ? null : "Kötelező megadni a jelszót a beállítások frissítéséhez!")
        }
    });

    const saveMutation = useMutation({
        mutationFn: async (formSent) => {
            const resp = await fetchAPI(`/User/${user.id}`, {
                method: "PUT",
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
                const respJson = await resp.json().catch(() => {});
                throw new Error(respJson?.error ?? "Nem sikerült menteni az adatokat!");
            }
        },
        onSuccess: () => queryClient.invalidateQueries({ queryKey: ["user", String(user.id)] }),
        onError: (error) => notifications.show({ title: "Hiba!", message: error.message, color: "red" }),
    })

    const handleSave = (values) => {
        saveMutation.mutate(trimForm(values));
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

                <DatePickerInput
                    label="Születési dátum"
                    {...datePickerInputProps}
                    {...form.getInputProps("dateOfBirth")}
                />

                <TextInput
                    label="Személyi igazolvány szám"
                    placeholder="pl. 123456AB"
                    {...form.getInputProps("idCardNumber")}
                />

                <TextInput
                    label="Vezetői engedély száma (opcionális)"
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