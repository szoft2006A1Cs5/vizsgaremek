import { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import "./Registration.css";
import logo from "../../assets/kepek/logo/comove_logo4.png";

function Registration() {
    const navigate = useNavigate();
    const location = useLocation();

    if (localStorage.getItem("token"))
        window.location.href = "/";

    const isRegister = location.pathname === "/register";

    const [showPass1, setShowPass1] = useState(false);
    const [showPass2, setShowPass2] = useState(false);

    const [showPage2, setShowPage2] = useState(false);
    const [error, setError] = useState("");

    const [regData, setRegData] = useState({
        name: "",
        phone: "",
        email: "",
        pass: "",
        passConfirm: "",
        birthdate: new Date().toDateString(),

        // 2. oldal
        idNumber: "",
        driverNumber: "",
        driverIssued: "",
        zipcode: "",
        address: "",
        settlement: "",
        accepted: false
    });

    function registerCheck() {
        let birthdate = new Date(regData.birthdate);

        if (!showPage2) {
            if (!regData.name.match(/^[A-ZÁÉÍÓÚÜŰÖŐ][a-záéíóúüűöő]+( [A-ZÁÉÍÓÚÜŰÖŐ][a-záéíóúüűöő]+)$/)) {
                setError("Nem megfelelő a név formátuma!");
                return;
            }

            birthdate.setHours(0, 0, 0, 0);
            birthdate.setFullYear(birthdate.getFullYear() + 18)
            let now = new Date();
            now.setHours(0, 0, 0, 0)
            console.log(now < birthdate)
            if (now < birthdate) {
                setError("Nem múltál el 18!");
                return;
            }

            if (!regData.phone.match(/^(36|06)(94|70|30|20)\d{7}$/)) {
                setError("Nem megfelelő a telefonszám formátuma!");
                return;
            }

            if (!regData.email.match(/^[A-z0-9.-]+@([A-z0-9-]+\.)+(com|hu)$/)) {
                setError("Nem megfelelő a e-mail cím formátuma!");
                return;
            }

            if (regData.pass.trim() === "") {
                setError("Nem adtál meg jelszót!");
                return;
            }

            if (regData.pass !== regData.passConfirm) {
                setError("A jelszavak nem egyeznek!");
                return;
            }

            setError("");
            setShowPage2(true);
            return;
        }

        if (!regData.idNumber.match(/^\d{6}[A-Z]{2}$/)) {
            setError("Nem megfelelő a személyi igazolványszám formátuma!");
            return;
        }

        if (!regData.driverNumber.match(/^[A-Z]{2}\d{6}$/)) {
            setError("Nem megfelelő a vezetői engedély számának formátuma!");
            return;
        }

        if (regData.address.trim() === "") {
            setError("Nem adtál meg címet!");
            return;
        }

        if (regData.settlement.trim() === "") {
            setError("Nem adtál meg települést!");
            return;
        }

        if (!regData.zipcode.match(/^\d{4}$/)) {
            setError("Nem megfelelő az irányítószám!");
            return;
        }

        if (!regData.accepted) {
            setError("Nem fogadtad el a felhasználói feltételeket!");
            return;
        }

        setError("");

        fetch("https://localhost:7245/api/Auth/register", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                "idCardNumber": regData.idNumber,
                "name": regData.name,
                "phone": regData.phone,
                "dateOfBirth": `${birthdate.getFullYear()}-${birthdate.getMonth()}-${birthdate.getDate()}`,
                "email": regData.email,
                "password": regData.pass,
                "driversLicenseNumber": regData.driverNumber,
                "driversLicenseDate": "2026-02-17",
                "addressZipcode": regData.zipcode,
                "addressSettlement": regData.settlement,
                "addressStreetHouse": regData.address
            })
        })
        .then(resp => {
            if (resp.status === 409) {
                setError("Már van ezekkel a felhasználói adatokkal fiók!");
                return null;
            }

            if (resp.status === 400) {
                setError("A megadott adatok hibásak!");
                return null;
            }

            if ((resp.status / 100) === 5) {
                setError("Szerverhiba!");
                return null;
            }

            return resp.json();
        }).then(data => {
            if (data === null) return;

            if (!data.token) {
                setError("A szerver nem várt válasszal tért vissza!");
                return;
            }
            
            localStorage.setItem("token", data.token);
            navigate("/");
        }).catch(_ => {
            setError("Váratlan hiba lépett fel!");
        })
    }

    return (
        <>
            <main className="reg_frame">
                <section className="reg_card">
                    <header className="reg_hero">
                        <div className="reg_topbar">
                            <button
                                className="reg_back"
                                type="button"
                                aria-label="Vissza"
                                onClick={() => navigate(-1)}
                            >
                                <svg width="18" height="18" viewBox="0 0 24 24" fill="none">
                                    <path
                                        d="M14.5 5l-7 7 7 7"
                                        stroke="white"
                                        strokeWidth="2.2"
                                        strokeLinecap="round"
                                        strokeLinejoin="round"
                                    />
                                </svg>
                            </button>
                        </div>

                        <div className="reg_hero-logo" aria-hidden="true">
                            <img src={logo} alt="CoMove" />
                        </div>
                    </header>

                    <div className="reg_content">
                        <div className="reg_title">
                            <h1 id="headline">Regisztráció</h1>
                            <div className="reg_subtitle" id="subline">
                                Hozza létre új fiókját
                            </div>
                        </div>
                    </div>

                    <div className="reg_tabs" role="tablist" aria-label="Belépés / Regisztráció">
                        <button
                            className={`reg_tab ${!isRegister ? "reg_active" : ""}`}
                            id="tab-login"
                            type="button"
                            role="tab"
                            onClick={() => navigate("/login")}
                        >
                            Bejelentkezés
                        </button>
                        <button
                            className={`reg_tab ${isRegister ? "reg_active" : ""}`}
                            id="tab-register"
                            type="button"
                            role="tab"
                            onClick={() => navigate("/register")}
                        >
                            Regisztráció
                        </button>
                    </div>

                    <div className="reg_panel reg_active" id="panel-register">
                        <form className="reg_form" action="#">
                            {showPage2 ? 
                                <button 
                                    type="button" 
                                    className="reg_btn"
                                    style={{marginTop: "0px", marginBottom: "10px"}}
                                    onClick={() => {
                                        setError("");
                                        setShowPage2(false)
                                    }}>
                                        Vissza
                                </button>
                            : <></>}

                            { !showPage2 ? <>
                                <label className="reg_field">
                                    <svg className="reg_icon" viewBox="0 0 24 24">
                                        <path d="M12 12a4.5 4.5 0 1 0-4.5-4.5A4.5 4.5 0 0 0 12 12zm0 2c-4.4 0-8 2.2-8 5v1h16v-1c0-2.8-3.6-5-8-5z" />
                                    </svg>
                                    <input type="text" name="teljes_name" placeholder="Teljes név" value={regData.name} onInput={e => setRegData({ ...regData, name: e.target.value })} />
                                </label>

                                <label className="reg_field">
                                    <svg className="reg_icon" viewBox="0 0 24 24" aria-hidden="true">
                                        <path d="M7 2a1 1 0 0 1 1 1v1h8V3a1 1 0 1 1 2 0v1h1a3 3 0 0 1 3 3v12a3 3 0 0 1-3 3H5a3 3 0 0 1-3-3V7a3 3 0 0 1 3-3h1V3a1 1 0 0 1 1-1zm12 8H5v9a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1v-9zM6 8h14V7a1 1 0 0 0-1-1h-1v1a1 1 0 1 1-2 0V6H8v1a1 1 0 1 1-2 0V6H5a1 1 0 0 0-1 1v1z" />
                                    </svg>
                                    <input type="date" name="szuletesi_datum" placeholder="Születési dátum" value={regData.birthdate} onInput={e => setRegData({ ...regData, birthdate: e.target.value })} />
                                </label>

                                <label className="reg_field">
                                    <svg className="reg_icon" viewBox="0 0 24 24" aria-hidden="true">
                                        <path d="M6.6 10.8c1.5 3 3.9 5.4 6.9 6.9l2.3-2.3c.4-.4 1-.5 1.5-.3 1 .4 2.1.6 3.2.6.6 0 1 .4 1 1V20c0 .6-.4 1-1 1C11.8 21 3 12.2 3 1.5 3 .9 3.4.5 4 .5H7c.6 0 1 .4 1 1 0 1.1.2 2.2.6 3.2.2.5.1 1.1-.3 1.5L6.6 10.8z" />
                                    </svg>
                                    <input
                                        type="tel"
                                        name="telefonszam"
                                        placeholder="Telefonszám"
                                        autoComplete="tel"
                                        inputMode="tel"
                                        value={regData.phone}
                                        onInput={e => setRegData({ ...regData, phone: e.target.value })}
                                    />
                                </label>

                                <label className="reg_field">
                                    <svg className="reg_icon" viewBox="0 0 24 24">
                                        <path d="M20 4H4a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2V6a2 2 0 0 0-2-2zm0 4-8 5L4 8V6l8 5 8-5z" />
                                    </svg>
                                    <input 
                                        type="email" 
                                        name="email" 
                                        placeholder="user@gmail.com"
                                        value={regData.email}
                                        onInput={e => setRegData({ ...regData, email: e.target.value })}    
                                    />
                                </label>

                                {/* Jelszó 1 */}
                                <label className="reg_field">
                                    <svg className="reg_icon" viewBox="0 0 24 24">
                                        {showPass1 ? (
                                            <path d="M17 10H8V8a4 4 0 1 1 8 0h2a6 6 0 1 0-12 0v2H5a2 2 0 0 0-2 2v7a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2v-7a2 2 0 0 0-2-2z" />
                                        ) : (
                                            <path d="M17 10h-1V8a4 4 0 0 0-8 0v2H7a2 2 0 0 0-2 2v7a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2v-7a2 2 0 0 0-2-2zM10 8a2 2 0 0 1 4 0v2h-4V8z" />
                                        )}
                                    </svg>

                                    <input 
                                        type={showPass1 ? "text" : "password"} 
                                        name="jelszo" 
                                        placeholder="Jelszó" 
                                        value={regData.pass}
                                        onInput={e => setRegData({ ...regData, pass: e.target.value })}
                                    />

                                    <svg
                                        className={`reg_szem ${showPass1 ? "reg_active" : ""}`}
                                        viewBox="0 0 24 24"
                                        aria-hidden="true"
                                        onClick={() => setShowPass1((v) => !v)}
                                    >
                                        <path d="M12 5c-7 0-10 7-10 7s3 7 10 7 10-7 10-7-3-7-10-7zm0 11a4 4 0 1 1 4-4 4 4 0 0 1-4 4z" />
                                    </svg>
                                </label>

                                {/*<label className="reg_field">
                                    <svg className="reg_icon" viewBox="0 0 24 24">
                                        <path d="M17 10h-1V8a4 4 0 0 0-8 0v2H7a2 2 0 0 0-2 2v7a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2v-7a2 2 0 0 0-2-2zM10 8a2 2 0 0 1 4 0v2h-4V8z" />
                                    </svg>
                                    <input type={showPass1 ? "text" : "password"} name="jelszo" placeholder="Jelszó" />
                                    <svg
                                        className={`reg_szem ${showPass1 ? "reg_active" : ""}`}
                                        viewBox="0 0 24 24"
                                        aria-hidden="true"
                                        onClick={() => setShowPass1((v) => !v)}
                                    >
                                        <path d="M12 5c-7 0-10 7-10 7s3 7 10 7 10-7 10-7-3-7-10-7zm0 11a4 4 0 1 1 4-4 4 4 0 0 1-4 4z" />
                                    </svg>
                                </label>

                                <label className="reg_field">
                                    <svg className="reg_icon" viewBox="0 0 24 24">
                                        <path d="M17 10h-1V8a4 4 0 0 0-8 0v2H7a2 2 0 0 0-2 2v7a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2v-7a2 2 0 0 0-2-2zM10 8a2 2 0 0 1 4 0v2h-4V8z" />
                                    </svg>
                                    <input type={showPass2 ? "text" : "password"} name="jelszo2" placeholder="Jelszó újra" />
                                    <svg
                                        className={`reg_szem ${showPass2 ? "reg_active" : ""}`}
                                        viewBox="0 0 24 24"
                                        aria-hidden="true"
                                        onClick={() => setShowPass2((v) => !v)}
                                    >
                                        <path d="M12 5c-7 0-10 7-10 7s3 7 10 7 10-7 10-7-3-7-10-7zm0 11a4 4 0 1 1 4-4 4 4 0 0 1-4 4z" />
                                    </svg>
                                </label>*/}

                                {/* Jelszó 2 */}
                                <label className="reg_field">
                                    <svg className="reg_icon" viewBox="0 0 24 24">
                                        {showPass2 ? (
                                            <path d="M17 10H8V8a4 4 0 1 1 8 0h2a6 6 0 1 0-12 0v2H5a2 2 0 0 0-2 2v7a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2v-7a2 2 0 0 0-2-2z" />
                                        ) : (
                                            <path d="M17 10h-1V8a4 4 0 0 0-8 0v2H7a2 2 0 0 0-2 2v7a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2v-7a2 2 0 0 0-2-2zM10 8a2 2 0 0 1 4 0v2h-4V8z" />
                                        )}
                                    </svg>

                                    <input 
                                        type={showPass2 ? "text" : "password"} 
                                        name="jelszo2" 
                                        placeholder="Jelszó újra" 
                                        value={regData.passConfirm}
                                        onInput={e => setRegData({ ...regData, passConfirm: e.target.value })}
                                    />

                                    <svg
                                        className={`reg_szem ${showPass2 ? "reg_active" : ""}`}
                                        viewBox="0 0 24 24"
                                        aria-hidden="true"
                                        onClick={() => setShowPass2((v) => !v)}
                                    >
                                        <path d="M12 5c-7 0-10 7-10 7s3 7 10 7 10-7 10-7-3-7-10-7zm0 11a4 4 0 1 1 4-4 4 4 0 0 1-4 4z" />
                                    </svg>
                                </label>
                            </> : <></> }

                            { showPage2 ? <>
                                <label className="reg_field">
                                    <svg className="reg_icon" viewBox="0 0 24 24">
                                        <path d="M12 12a4.5 4.5 0 1 0-4.5-4.5A4.5 4.5 0 0 0 12 12zm0 2c-4.4 0-8 2.2-8 5v1h16v-1c0-2.8-3.6-5-8-5z" />
                                    </svg>
                                    <input type="text" name="idNum" placeholder="Személyi igazolványszám" value={regData.idNumber} onInput={e => setRegData({ ...regData, idNumber: e.target.value })} />
                                </label>

                                <label className="reg_field">
                                    <svg className="reg_icon" viewBox="0 0 24 24">
                                        <path d="M12 12a4.5 4.5 0 1 0-4.5-4.5A4.5 4.5 0 0 0 12 12zm0 2c-4.4 0-8 2.2-8 5v1h16v-1c0-2.8-3.6-5-8-5z" />
                                    </svg>
                                    <input type="text" name="driversNum" placeholder="Vezetői engedély száma" value={regData.driverNumber} onInput={e => setRegData({ ...regData, driverNumber: e.target.value })} />
                                </label>

                                <label className="reg_field">
                                    <svg className="reg_icon" viewBox="0 0 24 24">
                                        <path d="M12 12a4.5 4.5 0 1 0-4.5-4.5A4.5 4.5 0 0 0 12 12zm0 2c-4.4 0-8 2.2-8 5v1h16v-1c0-2.8-3.6-5-8-5z" />
                                    </svg>
                                    <input type="text" name="address" placeholder="Utca, házszám" value={regData.address} onInput={e => setRegData({ ...regData, address: e.target.value })} />
                                </label>

                                <label className="reg_field">
                                    <svg className="reg_icon" viewBox="0 0 24 24">
                                        <path d="M12 12a4.5 4.5 0 1 0-4.5-4.5A4.5 4.5 0 0 0 12 12zm0 2c-4.4 0-8 2.2-8 5v1h16v-1c0-2.8-3.6-5-8-5z" />
                                    </svg>
                                    <input type="text" name="settlement" placeholder="Település" value={regData.settlement} onInput={e => setRegData({ ...regData, settlement: e.target.value })} />
                                </label>

                                <label className="reg_field">
                                    <svg className="reg_icon" viewBox="0 0 24 24">
                                        <path d="M12 12a4.5 4.5 0 1 0-4.5-4.5A4.5 4.5 0 0 0 12 12zm0 2c-4.4 0-8 2.2-8 5v1h16v-1c0-2.8-3.6-5-8-5z" />
                                    </svg>
                                    <input type="text" name="settlement" placeholder="Irányítószám" value={regData.zipcode} onInput={e => setRegData({ ...regData, zipcode: e.target.value })} />
                                </label>

                                <div className="reg_row reg_terms-row" style={{ marginTop: "6px" }}>
                                    <label className="reg_remember">
                                        <input 
                                            type="checkbox" 
                                            id="reg_feltetelek" 
                                            checked={regData.accepted}
                                            onChange={e => setRegData({ ...regData, accepted: e.target.checked })}
                                        />
                                        <span>
                                            Elfogadom a felhasználási<br />
                                            feltételeket
                                        </span>
                                    </label>
                                    <a
                                        href="https://njt.hu/felhasznalasi_feltetelek"
                                        id="reg_feltetelek-link"
                                        target="_blank"
                                        rel="noreferrer"
                                    >
                                        Feltételek
                                    </a>
                                </div>
                            </> : <></>}

                            { error !== "" ? 
                                <h3 style={{color: "red", textAlign: "center"}}>{error}</h3>
                            : <></> }

                            <button className="reg_btn" id="reg_regisztracio-gomb" type="button" onClick={registerCheck}>
                                {showPage2 ? <>Fiók létrehozása</> : <>Következő oldal</>}
                            </button>
                        </form>
                    </div>
                    <br />
                </section>
            </main>
        </>
    );
}

export default Registration;