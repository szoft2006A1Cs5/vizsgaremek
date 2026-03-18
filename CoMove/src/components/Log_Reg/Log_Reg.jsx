import { useEffect, useMemo, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import "./Log_Reg.css";
import bluelogo from "../../assets/kepek/logo/comove_logo1.png";
import whitelogo from "../../assets/kepek/logo/comove_logo4.png";

function Registration() {
    const navigate = useNavigate();
    const location = useLocation();
    const routeIsRegister = useMemo(
        () => location.pathname === "/register",
        [location.pathname]
    )

    useEffect(() => {
        if (localStorage.getItem("auth"))
            navigate("/");
    }, []);

    const nameRegex = /^[A-ZÁÉÍÓÖŐÚÜŰ][a-záéíóöőúüű]+(?: [A-ZÁÉÍÓÖŐÚÜŰ][a-záéíóöőúüű]+)+$/
    const idcardRegex = /^\d{6}[A-Z]{2}$/
    const passwordRegex = /^(?=.*[A-ZÁÉÍÓÖŐÚÜŰ])(?=.*\d).{8,}$/
    const drivlicenseRegex = /^[A-Z]{2}\d{6}$/
    const addresssettlementRegex = /^[A-ZÁÉÍÓÖŐÚÜŰ][a-záéíóöőúüű]{2,100}$/
    const addresszipcodeRegex = /^\d{4}$/
    const phoneRegex = /^(36|06)(94|70|30|20)\d{7}$/
    const emailRegex = /^[A-z0-9.-]+@([A-z0-9-]+\.)+([A-z]{2,3})$/
    const today = new Date()
    const maxBirthDate = new Date(today.setFullYear(today.getFullYear() - 18))
    const [isRegisterUI, setIsRegisterUI] = useState(routeIsRegister)
    const [leaving, setLeaving] = useState(null)
    const [regStep, setRegStep] = useState(0)
    const [errors, setErrors] = useState({})
    const [showLoginPass, setShowLoginPass] = useState(false)
    const [showRegPass1, setShowRegPass1] = useState(false)
    const [showRegPass2, setShowRegPass2] = useState(false)
    const [formData, setFormData] = useState({
        teljes_nev: "",
        szuletesi_datum: "",
        nem: "",
        iranyitoszam: "",
        telepules: "",
        utca_hazszam: "",
        telefonszam: "",
        szemelyi_szam: "",
        email: "",
        jelszo: "",
        jelszo2: "",
    });
    const [loginData, setLoginData] = useState({
        email: "",
        password: "",
    })


    const isValidAdultBirthDate = (birthDate) => {
        const today = new Date();
        const birth = new Date(birthDate);
        let age = today.getFullYear() - birth.getFullYear();
        const m = today.getMonth() - birth.getMonth();

        if (m < 0 || (m === 0 && today.getDate() < birth.getDate())) {
            age--;
        }
        return age >= 18;
    };


    const validateStep = () => {
        const newErrors = {};
        if (regStep === 0) {
            if (!formData.teljes_nev.trim()) {
                newErrors.teljes_nev = "A név megadása kötelező.";
            }
            else if (!nameRegex.test(formData.teljes_nev.trim())) {
                newErrors.teljes_nev = "A név legalább két szóból álljon és nagybetűvel kezdődjön.";
            }

            if (!formData.szuletesi_datum) {
                newErrors.szuletesi_datum = "A születési dátum megadása kötelező.";
            }
            else if (!isValidAdultBirthDate(formData.szuletesi_datum)) {
                newErrors.szuletesi_datum = "A regisztrációhoz legalább 18 évesnek kell lenned.";
            }

            if (!formData.szemelyi_szam.trim()) {
                newErrors.szemelyi_szam = "A személyi szám megadása kötelező.";
            } else if (!idcardRegex.test(formData.szemelyi_szam.trim())) {
                newErrors.szemelyi_szam = "A személyi szám csak számokat tartalmazhat.";
            }
        }

        if (regStep === 1) {
            if (!formData.iranyitoszam.trim()) {
                newErrors.iranyitoszam = "Add meg az irányítószámot.";
            } else if (!addresszipcodeRegex.test(formData.iranyitoszam.trim())) {
                newErrors.iranyitoszam = "Az irányítószám csak 4 számjegyből állhat.";
            }

            if (!formData.telepules.trim()) {
                newErrors.telepules = "Add meg a település nevét.";
            } else if (!addresssettlementRegex.test(formData.telepules.trim())) {
                newErrors.telepules = "A település nem tartalmazhat számot.";
            }

            if (!formData.utca_hazszam.trim()) {
                newErrors.utca_hazszam = "Add meg az utca nevet és házszámot.";
            } 

            if (!formData.telefonszam.trim()) {
                newErrors.telefonszam = "A telefonszám megadása kötelező.";
            } else if (!phoneRegex.test(formData.telefonszam.trim())) {
                newErrors.telefonszam = "A telefonszám csak számokat tartalmazhat.";
            }

            
        }
        if (regStep === 2) {
            if (!formData.email.trim()) {
                newErrors.email = "Az email cím megadása kötelező.";
            } else if (!emailRegex.test(formData.email.trim())) {
                newErrors.email = "Nem megfelelő email formátum.";
            }

            if (!formData.jelszo.trim()) {
                newErrors.jelszo = "A jelszó megadása kötelező.";
            } else if (!passwordRegex.test(formData.jelszo)) {
                newErrors.jelszo =
                    "A jelszónak legalább 8 karakter hosszúnak kell lennie, tartalmaznia kell legalább 1 nagybetűt és 1 számot.";
            }

            if (!formData.jelszo2.trim()) {
                newErrors.jelszo2 = "A jelszó megerősítése kötelező.";
            } else if (formData.jelszo !== formData.jelszo2) {
                newErrors.jelszo2 = "A két jelszó nem egyezik.";
            }
        }
        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const validateLogin = () => {
        if (!emailRegex.test(loginData.email)) {
            setErrors({ loginEmail: "Nem megfelelő email formátum." });
            return false;
        }

        return true;
    };

    useEffect(() => {
        if (leaving) return;
        setIsRegisterUI(routeIsRegister);
    }, [routeIsRegister, leaving]);

    useEffect(() => {
        if (!leaving) return;
        if (location.pathname === leaving) setLeaving(null);
    }, [location.pathname, leaving]);

    const goLogin = () => {
        if (!isRegisterUI) return;
        setLeaving("/login");
        setIsRegisterUI(false);
        window.setTimeout(() => navigate("/login"), 950);
    };

    const goRegister = () => {
        if (isRegisterUI) return;
        setLeaving("/register");
        setIsRegisterUI(true);
        setRegStep(0);
        window.setTimeout(() => navigate("/register"), 950);
    };

    const handleChange = (e) => {
        const { name, value } = e.target;

        // TELJES NÉV AUTOMATIKUS FORMÁZÁSA
        if (name === "teljes_nev") {
            const formatted = value
                .split(" ")
                .map(word =>
                    word.charAt(0).toUpperCase() + word.slice(1).toLowerCase()
                )
                .join(" ");

            setFormData(prev => ({
                ...prev,
                [name]: formatted
            }));

            setErrors(prev => ({
                ...prev,
                [name]: false
            }));

            return;
        }

        // MINDEN MÁS INPUT
        setFormData((prev) => ({
            ...prev,
            [name]: value,
        }));

        setErrors((prev) => ({
            ...prev,
            [name]: false,
        }));
    };

    const handleNextStep = () => {
        const valid = validateStep();
        if (!valid) return;

        setRegStep((s) => Math.min(2, s + 1));
    };

    const handleRegister = () => {
        const valid = validateStep();
        if (!valid) return;

        fetch("https://localhost:7245/api/Auth/register", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                email: loginData.email,
                password: loginData.password,
            })
        })
        .then(resp => {
            if (resp.status !== 200)
                return null;

            return resp.json();
        })
        .then(data => {
            if (!data)
                return;

            localStorage.setItem("auth", JSON.stringify(data));
            navigate("/");
        })
    };

    const handleLogin = () => {
        const valid = validateLogin();
        if (!valid) return;

        fetch("https://localhost:7245/api/Auth/login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                email: loginData.email,
                password: loginData.password,
            })
        })
        .then(resp => {
            if (resp.status !== 200)
                return null;

            return resp.json();
        })
        .then(data => {
            if (!data)
                return;

            localStorage.setItem("auth", JSON.stringify(data));
            navigate("/");
        })
    }

    return (
        <div className="auth_page">
            <div className={`auth_box ${isRegisterUI ? "isRegister" : "isLogin"}`}>
                <div className="auth_brand">
                    <button
                        className="auth_brand_btn"
                        type="button"
                        aria-label="Vissza a főoldalra"
                        onClick={() => navigate("/")}
                    >
                        <img src={isRegisterUI ? bluelogo : whitelogo} alt="CoMove" />
                    </button>
                </div>

                <aside className="auth_side">
                    <div className="auth_dots" aria-hidden="true">
                        <span></span><span></span><span></span><span></span>
                        <span></span><span></span><span></span><span></span>
                    </div>

                    <h2>{isRegisterUI ? "Van már fiókod?" : "Új vagy itt?"}</h2>
                    <p>
                        {isRegisterUI
                            ? "Jelentkezz be, és folytasd ott, ahol abbahagytad."
                            : "Hozd létre fiókodat pár lépésben, és csatlakozz a CoMove közösségéhez!"}
                    </p>

                    <button
                        className="auth_ghost"
                        type="button"
                        onClick={isRegisterUI ? goLogin : goRegister}
                    >
                        {isRegisterUI ? "Bejelentkezés" : "Regisztráció"}
                    </button>
                </aside>

                <section className="auth_forms">
                    <div
                        className={[
                            "auth_panel",
                            "auth_login",
                            !isRegisterUI ? "active" : "",
                            leaving === "/login" ? "leaving" : "",
                        ].join(" ")}
                    >
                        <h1>Üdv újra!</h1>
                        <div className="auth_sub">Jelentkezz be a fiókodba:</div>

                        <form className="auth_form" onSubmit={(e) => e.preventDefault()}>
                            <label className={`auth_field ${errors.loginEmail ? "input_error" : ""}`}>
                                <svg className="auth_icon" viewBox="0 0 24 24" aria-hidden="true">
                                    <path d="M20 4H4a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2V6a2 2 0 0 0-2-2zm0 4-8 5L4 8V6l8 5 8-5z" />
                                </svg>
                                <input 
                                    type="email" 
                                    name="email" 
                                    placeholder="user@gmail.com" 
                                    autoComplete="email"
                                    value={loginData.email}
                                    onInput={(e) => setLoginData({ ...loginData, email: e.target.value })}
                                />
                            </label>

                            {errors.loginEmail && (
                                <div className="error_text">{errors.loginEmail}</div>
                            )}

                            <label className="auth_field">
                                <svg className="auth_icon" viewBox="0 0 24 24" aria-hidden="true">
                                    <path d="M17 10h-1V8a4 4 0 0 0-8 0v2H7a2 2 0 0 0-2 2v7a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2v-7a2 2 0 0 0-2-2zM10 8a2 2 0 0 1 4 0v2h-4V8z" />
                                </svg>

                                <input
                                    type={showLoginPass ? "text" : "password"}
                                    name="password"
                                    placeholder="Jelszó"
                                    autoComplete="current-password"
                                    value={loginData.password}
                                    onInput={(e) => setLoginData({ ...loginData, password: e.target.value })}
                                />

                                <button
                                    type="button"
                                    className="auth_eye"
                                    aria-label={showLoginPass ? "Jelszó elrejtése" : "Jelszó megjelenítése"}
                                    onClick={() => setShowLoginPass((v) => !v)}
                                >
                                    <svg viewBox="0 0 24 24" aria-hidden="true">
                                        <path d="M12 5c-7 0-10 7-10 7s3 7 10 7 10-7 10-7-3-7-10-7zm0 11a4 4 0 1 1 4-4 4 4 0 0 1-4 4z" />
                                    </svg>
                                </button>
                            </label>

                            <div className="auth_forgot">
                                <button
                                    type="button"
                                    className="auth_forgot_link"
                                    onClick={() => navigate("/forgot-password")}
                                >
                                    Elfelejtetted a jelszavad?
                                </button>
                            </div>

                            <button className="auth_primary" type="button" onClick={handleLogin}>
                                Bejelentkezés
                            </button>
                        </form>
                    </div>

                    <div
                        className={[
                            "auth_panel",
                            "auth_register",
                            isRegisterUI ? "active" : "",
                            leaving === "/register" ? "leaving" : "",
                        ].join(" ")}
                    >
                        <h1>Fiók létrehozás</h1>
                        <div className="auth_sub">Hozzd létre új fiókodat:</div>

                        <div className="reg_steps" aria-hidden="true">
                            <span className={regStep === 0 ? "on" : ""}></span>
                            <span className={regStep === 1 ? "on" : ""}></span>
                            <span className={regStep === 2 ? "on" : ""}></span>
                        </div>

                        <form className="auth_form" onSubmit={(e) => e.preventDefault()}>
                            {regStep === 0 && (
                                <>
                                    <label className={`auth_field ${errors.teljes_nev ? "input_error" : ""}`}>
                                        <svg className="auth_icon" viewBox="0 0 24 24">
                                            <path d="M12 12a4.5 4.5 0 1 0-4.5-4.5A4.5 4.5 0 0 0 12 12zm0 2c-4.4 0-8 2.2-8 5v1h16v-1c0-2.8-3.6-5-8-5z" />
                                        </svg>

                                        <input
                                            type="text"
                                            name="teljes_nev"
                                            placeholder="Teljes név"
                                            value={formData.teljes_nev}
                                            onChange={handleChange}
                                        />
                                    </label>
                                    {errors.teljes_nev && (
                                        <div className="error_text">{errors.teljes_nev}</div>
                                    )}

                                    <label className={`auth_field ${errors.szuletesi_datum ? "input_error" : ""}`}>
                                        <svg className="auth_icon" viewBox="0 0 24 24" aria-hidden="true">
                                            <path d="M7 2a1 1 0 0 1 1 1v1h8V3a1 1 0 1 1 2 0v1h1a3 3 0 0 1 3 3v12a3 3 0 0 1-3 3H5a3 3 0 0 1-3-3V7a3 3 0 0 1 3-3h1V3a1 1 0 0 1 1-1zm12 8H5v9a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1v-9zM6 8h14V7a1 1 0 0 0-1-1h-1v1a1 1 0 1 1-2 0V6H8v1a1 1 0 1 1-2 0V6H5a1 1 0 0 0-1 1v1z" />
                                        </svg>
                                        <input
                                            type="date"
                                            name="szuletesi_datum"
                                            value={formData.szuletesi_datum}
                                            onChange={handleChange}
                                            min="1900-01-01"
                                            max={new Date().toISOString().split("T")[0]}
                                        />
                                    </label>
                                    {errors.szuletesi_datum && (
                                        <div className="error_text">{errors.szuletesi_datum}</div>
                                    )}

                                    <label className={`auth_field ${errors.szemelyi_szam ? "input_error" : ""}`}>
                                        <svg className="auth_icon" viewBox="0 0 24 24" aria-hidden="true">
                                            <path d="M3 7h18v10H3V7zm2 2v6h14V9H5z" />
                                        </svg>
                                        <input
                                            type="text"
                                            name="szemelyi_szam"
                                            placeholder="Személyi szám"
                                            value={formData.szemelyi_szam}
                                            onChange={handleChange}
                                        />
                                    </label>
                                    {errors.szemelyi_szam && <div className="error_text">{errors.szemelyi_szam}</div>}
                                </>
                            )}

                            {regStep === 1 && (
                                <>
                                    <label className={`auth_field ${errors.iranyitoszam ? "input_error" : ""}`}>
                                        <svg className="auth_icon" viewBox="0 0 24 24" aria-hidden="true">
                                            <path d="M12 2 3 10v12h6v-7h6v7h6V10l-9-8z" />
                                        </svg>
                                        <input
                                            type="text"
                                            name="iranyitoszam"
                                            placeholder="Irányítószám"
                                            autoComplete="address-ZipCode"
                                            value={formData.iranyitoszam}
                                            onChange={handleChange}
                                        />
                                    </label>
                                    {errors.iranyitoszam && <div className="error_text">{errors.iranyitoszam}</div>}

                                    <label className={`auth_field ${errors.telepules ? "input_error" : ""}`}>
                                        <svg className="auth_icon" viewBox="0 0 24 24" aria-hidden="true">
                                            <path d="M12 2 3 10v12h6v-7h6v7h6V10l-9-8z" />
                                        </svg>
                                        <input
                                            type="text"
                                            name="telepules"
                                            placeholder="Település"
                                            autoComplete="address-settlement"
                                            value={formData.telepules}
                                            onChange={handleChange}
                                        />
                                    </label>
                                    {errors.telepules && <div className="error_text">{errors.telepules}</div>}

                                    <label className={`auth_field ${errors.utca_hazszam ? "input_error" : ""}`}>
                                        <svg className="auth_icon" viewBox="0 0 24 24" aria-hidden="true">
                                            <path d="M12 2 3 10v12h6v-7h6v7h6V10l-9-8z" />
                                        </svg>
                                        <input
                                            type="text"
                                            name="utca_hazszam"
                                            placeholder="Utca, házszám"
                                            autoComplete="street-address"
                                            value={formData.utca_hazszam}
                                            onChange={handleChange}
                                        />
                                    </label>
                                    {errors.lakcim && <div className="error_text">{errors.lakcim}</div>}

                                    <label className={`auth_field ${errors.telefonszam ? "input_error" : ""}`}>
                                        <svg className="auth_icon" viewBox="0 0 24 24" aria-hidden="true">
                                            <path d="M6.6 10.8c1.5 3 3.9 5.4 6.9 6.9l2.3-2.3c.4-.4 1-.5 1.5-.3 1 .4 2.1.6 3.2.6.6 0 1 .4 1 1V20c0 .6-.4 1-1 1C11.8 21 3 12.2 3 1.5 3 .9 3.4.5 4 .5H7c.6 0 1 .4 1 1 0 1.1.2 2.2.6 3.2.2.5.1 1.1-.3 1.5L6.6 10.8z" />
                                        </svg>
                                        <input
                                            type="tel"
                                            name="telefonszam"
                                            placeholder="Telefonszám"
                                            autoComplete="tel"
                                            inputMode="tel"
                                            value={formData.telefonszam}
                                            onChange={handleChange}
                                        />
                                    </label>
                                    {errors.telefonszam && <div className="error_text">{errors.telefonszam}</div>}

                                    
                                </>
                            )}

                            {regStep === 2 && (
                                <>
                                    <label className={`auth_field ${errors.email ? "input_error" : ""}`}>
                                        <svg className="auth_icon" viewBox="0 0 24 24" aria-hidden="true">
                                            <path d="M20 4H4a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2V6a2 2 0 0 0-2-2zm0 4-8 5L4 8V6l8 5 8-5z" />
                                        </svg>
                                        <input
                                            type="email"
                                            name="email"
                                            placeholder="user@gmail.com"
                                            autoComplete="email"
                                            value={formData.email}
                                            onChange={handleChange}
                                        />
                                    </label>
                                    {errors.email && <div className="error_text">{errors.email}</div>}

                                    <label className={`auth_field ${errors.jelszo ? "input_error" : ""}`}>
                                        <svg className="auth_icon" viewBox="0 0 24 24" aria-hidden="true">
                                            <path d="M17 10h-1V8a4 4 0 0 0-8 0v2H7a2 2 0 0 0-2 2v7a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2v-7a2 2 0 0 0-2-2zM10 8a2 2 0 0 1 4 0v2h-4V8z" />
                                        </svg>

                                        <input
                                            type={showRegPass1 ? "text" : "password"}
                                            name="jelszo"
                                            placeholder="Jelszó"
                                            autoComplete="new-password"
                                            value={formData.jelszo}
                                            onChange={handleChange}
                                        />

                                        <button
                                            type="button"
                                            className="auth_eye"
                                            aria-label={showRegPass1 ? "Jelszó elrejtése" : "Jelszó megjelenítése"}
                                            onClick={() => setShowRegPass1((v) => !v)}
                                        >
                                            <svg viewBox="0 0 24 24" aria-hidden="true">
                                                <path d="M12 5c-7 0-10 7-10 7s3 7 10 7 10-7 10-7-3-7-10-7zm0 11a4 4 0 1 1 4-4 4 4 0 0 1-4 4z" />
                                            </svg>
                                        </button>
                                    </label>
                                    {errors.jelszo && <div className="error_text">{errors.jelszo}</div>}

                                    <label className={`auth_field ${errors.jelszo2 ? "input_error" : ""}`}>
                                        <svg className="auth_icon" viewBox="0 0 24 24" aria-hidden="true">
                                            <path d="M17 10h-1V8a4 4 0 0 0-8 0v2H7a2 2 0 0 0-2 2v7a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2v-7a2 2 0 0 0-2-2zM10 8a2 2 0 0 1 4 0v2h-4V8z" />
                                        </svg>

                                        <input
                                            type={showRegPass2 ? "text" : "password"}
                                            name="jelszo2"
                                            placeholder="Jelszó újra"
                                            autoComplete="new-password"
                                            value={formData.jelszo2}
                                            onChange={handleChange}
                                        />

                                        <button
                                            type="button"
                                            className="auth_eye"
                                            aria-label={showRegPass2 ? "Jelszó elrejtése" : "Jelszó megjelenítése"}
                                            onClick={() => setShowRegPass2((v) => !v)}
                                        >
                                            <svg viewBox="0 0 24 24" aria-hidden="true">
                                                <path d="M12 5c-7 0-10 7-10 7s3 7 10 7 10-7 10-7-3-7-10-7zm0 11a4 4 0 1 1 4-4 4 4 0 0 1-4 4z" />
                                            </svg>
                                        </button>
                                    </label>
                                    {errors.jelszo2 && <div className="error_text">{errors.jelszo2}</div>}
                                </>
                            )}

                            <div className="reg_nav">
                                <button
                                    type="button"
                                    className="reg_btn reg_secondary"
                                    onClick={() => setRegStep((s) => Math.max(0, s - 1))}
                                    disabled={regStep === 0}
                                >
                                    Vissza
                                </button>

                                {regStep < 2 ? (
                                    <button
                                        type="button"
                                        className="reg_btn reg_primary"
                                        onClick={handleNextStep}
                                    >
                                        Következő
                                    </button>
                                ) : (
                                    <button
                                        type="button"
                                        className="reg_btn reg_primary"
                                        onClick={handleRegister}
                                    >
                                        Fiók létrehozása
                                    </button>
                                )}
                            </div>
                        </form>
                    </div>
                </section>
            </div>
        </div>
    );
}

export default Registration;