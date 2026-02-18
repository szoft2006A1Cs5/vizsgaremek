import { useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import "./Registration.css";
import logo from "../../assets/kepek/logo/comove_logo1.png";

function Registration() {
    const navigate = useNavigate();
    const location = useLocation();
    const isRegister = location.pathname === "/register";

    const [showPass1, setShowPass1] = useState(false);
    const [showPass2, setShowPass2] = useState(false);

    return (
        <>
            <div className="reg_page">
                <main className="reg_frame">
                    <section className="reg_card">
                        <header className="reg_hero">
                            <div className="reg_topbar">
                                <button
                                    className="reg_back"
                                    type="button"
                                    aria-label="Vissza"
                                    onClick={() => navigate("/")}
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
                                <label className="reg_field">
                                    <svg className="reg_icon" viewBox="0 0 24 24">
                                        <path d="M12 12a4.5 4.5 0 1 0-4.5-4.5A4.5 4.5 0 0 0 12 12zm0 2c-4.4 0-8 2.2-8 5v1h16v-1c0-2.8-3.6-5-8-5z" />
                                    </svg>
                                    <input type="text" name="teljes_name" placeholder="Teljes név" />
                                </label>

                                <label className="reg_field">
                                    <svg className="reg_icon" viewBox="0 0 24 24" aria-hidden="true">
                                        <path d="M7 2a1 1 0 0 1 1 1v1h8V3a1 1 0 1 1 2 0v1h1a3 3 0 0 1 3 3v12a3 3 0 0 1-3 3H5a3 3 0 0 1-3-3V7a3 3 0 0 1 3-3h1V3a1 1 0 0 1 1-1zm12 8H5v9a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1v-9zM6 8h14V7a1 1 0 0 0-1-1h-1v1a1 1 0 1 1-2 0V6H8v1a1 1 0 1 1-2 0V6H5a1 1 0 0 0-1 1v1z" />
                                    </svg>
                                    <input type="date" name="szuletesi_datum" placeholder="Születési dátum" />
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
                                    />
                                </label>

                                <label className="reg_field">
                                    <svg className="reg_icon" viewBox="0 0 24 24">
                                        <path d="M20 4H4a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2V6a2 2 0 0 0-2-2zm0 4-8 5L4 8V6l8 5 8-5z" />
                                    </svg>
                                    <input type="email" name="email" placeholder="user@gmail.com" />
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

                                    <input type={showPass2 ? "text" : "password"} name="jelszo2" placeholder="Jelszó újra" />

                                    <svg
                                        className={`reg_szem ${showPass2 ? "reg_active" : ""}`}
                                        viewBox="0 0 24 24"
                                        aria-hidden="true"
                                        onClick={() => setShowPass2((v) => !v)}
                                    >
                                        <path d="M12 5c-7 0-10 7-10 7s3 7 10 7 10-7 10-7-3-7-10-7zm0 11a4 4 0 1 1 4-4 4 4 0 0 1-4 4z" />
                                    </svg>
                                </label>

                                <div className="reg_row reg_terms-row" style={{ marginTop: "6px" }}>
                                    <label className="reg_remember">
                                        <input type="checkbox" id="reg_feltetelek" />
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

                                <button
                                    className="reg_btn"
                                    type="button"
                                    onClick={() => navigate("/")}
                                >
                                    Fiók létrehozása
                                </button>
                            </form>
                            <br />
                            <div className="reg_bottom">
                                Van már fiókod?&nbsp;&nbsp;&nbsp;
                                <a
                                    href="/login"
                                    id="reg_goto-login"
                                    onClick={(e) => {
                                        e.preventDefault();
                                        navigate("/login");
                                    }}
                                >
                                    Bejelentkezem
                                </a>
                            </div>
                        </div>
                    </section>
                </main>
            </div>
        </>
    );
}

export default Registration;