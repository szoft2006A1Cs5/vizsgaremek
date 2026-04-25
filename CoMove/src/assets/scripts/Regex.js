
export const REGEX = {
    idCardNumber: /^\d{6}[A-Z]{2}$/,
    driversLicenseNumber: /^[A-Z]{2}\d{6}$/,
    name: /^[A-ZÁÉÍÓÚÜŰÖŐ][a-záéíóúüűöő]+( [A-ZÁÉÍÓÚÜŰÖŐ][a-záéíóúüűöő]+)+$/,
    email: /^[A-z0-9.-]+@([A-z0-9-]+\.)+([A-z]{2,3})$/,
    phone: /^(36|06)\d{8,9}$/,
    password: /^(?=.*[a-z])(?=.*\d)(?=.*[A-Z]).{8,}$/,
    addressZipcode: /^\d{4}$/
}