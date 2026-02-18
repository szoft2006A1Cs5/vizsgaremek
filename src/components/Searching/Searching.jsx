import "./Searching.css";
import logo from "../../assets/kepek/logo/comove_logo4.png";
import { useNavigate } from "react-router-dom";
import { useLayoutEffect, useEffect, useState } from "react";

function Searching() {
    const navigate = useNavigate();
    const [loaded, setLoaded] = useState(false);


    useLayoutEffect(() => {
        window.scrollTo(0, 0);
    }, []);

    useEffect(() => {
        const id = requestAnimationFrame(() => setLoaded(true));
        return () => cancelAnimationFrame(id);
    }, []);

    return (
        <>
            {/* Fejléc */}
            <div className="searching_root">
                <section className={`searching_hero searching_pageAnim ${loaded ? "isLoaded" : ""}`}>
                    <div className="searching_dots" aria-hidden="true">
                        <span></span><span></span><span></span><span></span>
                        <span></span><span></span><span></span><span></span><span></span>
                    </div>

                    <div className="searching_container">
                        <header className="searching_nav">
                            <div className="searching_brand" onClick={() => navigate("/")} role="button" tabIndex={0}>
                                <img className="searching_logo" src={logo} alt="CoMove" />
                            </div>

                            <div className="searching_auth">
                                <button className="searching_authBtnReg" type="button" onClick={() => navigate("/register")}>
                                    Regisztráció
                                </button>

                                <button className="searching_authBtn" type="button" onClick={() => navigate("/login")}>
                                    Bejelentkezés
                                </button>
                            </div>
                        </header>

                        <div className="searching_content">
                            <div className="searching_left">
                                <h1>
                                    Bérelj olcsón,
                                    <br />
                                    biztonságosan!
                                </h1>

                                <p>
                                    Találd meg az igényeidnek megfelelő autót, mellyel élvezet lesz a vezetés minden perce.
                                </p>

                                <button className="searching_btn" type="button" onClick={() => navigate("/")}>
                                    Főoldal
                                </button>
                            </div>

                            <div className="searching_right">
                                <div className="searching_carShadow" aria-hidden="true"></div>
                                <img
                                    className="searching_carImg"
                                    src="https://www.pngall.com/wp-content/uploads/8/White-SUV-PNG.png"
                                    alt="Autó"
                                    loading="lazy"
                                />
                            </div>
                        </div>
                    </div>
                </section>

                {/* Keresősáv */}
                <section className={`searching_page searching_pageAnim ${loaded ? "isLoaded" : ""}`}>
                    <div className="searching_searchWrap">
                        <h2 className="searching_searchTitle">Keressen autót most!</h2>

                        <div className="searching_searchBarOuter">
                            <div className="searching_searchBar">
                                <div className="searching_field">
                                    <div className="searching_label">Bérlés kezdete</div>
                                    <input className="searching_input" type="datetime-local" />
                                </div>

                                <div className="searching_divider" />

                                <div className="searching_field">
                                    <div className="searching_label">Bérlés vége</div>
                                    <input className="searching_input" type="datetime-local" />
                                </div>

                                <div className="searching_divider" />

                                <div className="searching_field">
                                    <div className="searching_label">Autó márka</div>
                                    <select className="searching_input" defaultValue="">
                                        <option value="">Bármely</option>
                                        <option>Toyota</option>
                                        <option>BMW</option>
                                        <option>Audi</option>
                                        <option>Mercedes</option>
                                        <option>Volkswagen</option>
                                    </select>
                                </div>

                                <div className="searching_divider" />

                                <div className="searching_field">
                                    <div className="searching_label">Autó típusa</div>
                                    <select className="searching_input" defaultValue="">
                                        <option value="">Bármely</option>
                                        <option>Sedan</option>
                                        <option>Kombi</option>
                                        <option>SUV</option>
                                        <option>Hatchback</option>
                                        <option>Kisbusz</option>
                                    </select>
                                </div>

                                <div className="searching_divider" />

                                <div className="searching_field">
                                    <div className="searching_label">Autó évjárata</div>
                                    <input className="searching_input" type="number" placeholder="pl. 2018" />
                                </div>

                                <div className="searching_divider" />

                                <div className="searching_field searching_fieldSmall">
                                    <div className="searching_label">Minimum óradíj</div>
                                    <input className="searching_input" type="number" placeholder="Ft/óra" />
                                </div>

                                <div className="searching_divider" />

                                <div className="searching_field searching_fieldSmall">
                                    <div className="searching_label">Maximum óradíj</div>
                                    <input className="searching_input" type="number" placeholder="Ft/óra" />
                                </div>

                                <div className="searching_divider" />

                                <div className="searching_field">
                                    <div className="searching_label">Átvételi hely</div>
                                    <input className="searching_input" type="text" placeholder="Város, megye" />
                                </div>

                                <button className="searching_searchBtn" type="button">
                                    Keresés
                                </button>
                            </div>
                        </div>
                    </div>
                </section>
            </div>
        </>
    );
}

export default Searching;