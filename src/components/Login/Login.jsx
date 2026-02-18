import { useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import "./Login.css";
import logo from "../../assets/kepek/logo/comove_logo1.png";

function Login() {
    const navigate = useNavigate();
    const location = useLocation();
    const isLogin = location.pathname === "/login";

    const [showPassword, setShowPassword] = useState(false);

    return (
        <>
            <div className="log_page">
                <main className="log_frame">
                    <section className="log_card">
                        <header className="log_hero">
                            <div className="log_topbar">
                                <button
                                    className="log_back"
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

                            <div className="log_hero-logo" aria-hidden="true">
                                <img src={logo} alt="CoMove" />
                            </div>
                        </header>

                        <div className="log_content">
                            <div className="log_title">
                                <h1 id="headline">Üdvözöljük!</h1>
                                <div className="log_subtitle" id="subline">
                                    Jelentkezzen be fiókjába
                                </div>
                            </div>
                        </div>

                        <div className="log_tabs" role="tablist" aria-label="Belépés / Regisztráció">
                            <button
                                className={`log_tab ${isLogin ? "log_active" : ""}`}
                                id="tab-login"
                                type="button"
                                role="tab"
                                onClick={() => navigate("/login")}
                            >
                                Bejelentkezés
                            </button>
                            <button
                                className={`log_tab ${!isLogin ? "log_active" : ""}`}
                                id="tab-register"
                                type="button"
                                role="tab"
                                onClick={() => navigate("/register")}
                            >
                                Regisztráció
                            </button>
                        </div>

                        <div className="log_panel log_active" id="panel-login" role="tabpanel" aria-labelledby="tab-login">
                            <form className="log_form" action="#" method="post">
                                <label className="log_field">
                                    <svg className="log_icon" viewBox="0 0 24 24">
                                        <path d="M12 12a4.5 4.5 0 1 0-4.5-4.5A4.5 4.5 0 0 0 12 12zm0 2c-4.4 0-8 2.2-8 5v1h16v-1c0-2.8-3.6-5-8-5z" />
                                    </svg>
                                    <input type="text" name="login_name" placeholder="Teljes név" autoComplete="name" />
                                </label>

                                {/*<label className="log_field">
                                <svg className="log_icon" viewBox="0 0 24 24">
                                    <path d="M17 10h-1V8a4 4 0 0 0-8 0v2H7a2 2 0 0 0-2 2v7a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2v-7a2 2 0 0 0-2-2zM10 8a2 2 0 0 1 4 0v2h-4V8z" />
                                </svg>
                                <input
                                    type={showPassword ? "text" : "password"}
                                    name="login_password"
                                    placeholder="Jelszó"
                                    autoComplete="current-password"
                                />

                                <svg
                                    className={`log_szem ${showPassword ? "log_active" : ""}`}
                                    viewBox="0 0 24 24"
                                    aria-hidden="true"
                                    onClick={() => setShowPassword((v) => !v)}
                                >
                                    <path d="M12 5c-7 0-10 7-10 7s3 7 10 7 10-7 10-7-3-7-10-7zm0 11a4 4 0 1 1 4-4 4 4 0 0 1-4 4z" />
                                </svg>
                            </label>*/}

                                <label className="log_field">
                                    {/* LAKAT: zárt/nyitott */}
                                    <svg className="log_icon" viewBox="0 0 24 24">
                                        {showPassword ? (
                                            <path d="M17 10H8V8a4 4 0 1 1 8 0h2a6 6 0 1 0-12 0v2H5a2 2 0 0 0-2 2v7a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2v-7a2 2 0 0 0-2-2z" />
                                        ) : (
                                            <path d="M17 10h-1V8a4 4 0 0 0-8 0v2H7a2 2 0 0 0-2 2v7a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2v-7a2 2 0 0 0-2-2zM10 8a2 2 0 0 1 4 0v2h-4V8z" />
                                        )}
                                    </svg>

                                    <input
                                        type={showPassword ? "text" : "password"}
                                        name="login_password"
                                        placeholder="Jelszó"
                                        autoComplete="current-password"
                                    />

                                    <svg
                                        className={`log_szem ${showPassword ? "log_active" : ""}`}
                                        viewBox="0 0 24 24"
                                        aria-hidden="true"
                                        onClick={() => setShowPassword((v) => !v)}
                                    >
                                        <path d="M12 5c-7 0-10 7-10 7s3 7 10 7 10-7 10-7-3-7-10-7zm0 11a4 4 0 1 1 4-4 4 4 0 0 1-4 4z" />
                                    </svg>
                                </label>

                                <div className="log_row">
                                    <label className="log_remember">
                                        <input type="checkbox" id="log_feltetelek" />
                                        <span>Emlékezz rám</span>
                                    </label>
                                    <a href="#">Elfelejtetted a jelszót?</a>
                                </div>

                                <br />
                                <button
                                    className="log_btn"
                                    type="button"
                                    onClick={() => navigate("/")}
                                >
                                    Belépés
                                </button>
                            </form>

                            <br />
                            <div className="log_bottom">
                                Nincs fiókod?&nbsp;&nbsp;&nbsp;
                                <a
                                    href="/register"
                                    id="goto-register"
                                    onClick={(e) => {
                                        e.preventDefault();
                                        navigate("/register");
                                    }}
                                >
                                    Regisztrálok
                                </a>
                            </div>
                        </div>
                    </section>
                </main>
            </div>
        </>
    );
}

export default Login;