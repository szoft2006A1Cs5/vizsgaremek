import "./Searching.css";
import { useLayoutEffect, useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { FaFacebookF, FaInstagram, FaTiktok } from "react-icons/fa";
import Cards from "../Cards/Cards";
import auto1 from "../../assets/kepek/autok/toyotacorolla1.png";
import auto2 from "../../assets/kepek/autok/toyotacorolla3.png";
import auto3 from "../../assets/kepek/autok/toyotacorolla5.png";
import auto4 from "../../assets/kepek/autok/toyotacorolla4.png";

const CARS = [
    {
        id: 1,
        title: "Toyota Corolla 1.8",
        brand: "Toyota",
        type: "Sedan",
        year: 2019,
        pricePerHour: 3000,
        pickup: "Szombathely",
        county: "Vas vármegye",
        owner: "Németh Csaba",
        rating: 4.0,
        img: auto1,
        fuel: "Hybrid",
        hp: 120,
        km: 215000,
        gearbox: "Automata",
        description: `Saját, rendszeresen karbantartott Toyota 
                    Corolla 1.8 Hybrid autómat adom bérbe. Nagyon 
                    megbízható, csendes és rendkívül takarékos, főleg
                    városban, de hosszabb utakra is kényelmes választás.
                    Az autó tiszta, megkímélt állapotú, mindig 
                    odafigyelek rá, hogy műszakilag és esztétikailag 
                    is rendben legyen. Ha egy biztonságos és gazdaságos 
                    autót keresel, jó választás lehet számodra.`
    },
    {
        id: 2,
        title: "Toyota Corolla 1.8",
        brand: "Toyota",
        type: "Sedan",
        year: 2008,
        pricePerHour: 1250,
        pickup: "Zalaegerszeg",
        county: "Zala vármegye",
        owner: "Konrádi Ferenc",
        rating: 2.5,
        img: auto2,
        fuel: "Benzin",
        hp: 90,
        km: 304522,
        gearbox: "Manuális",
        description: `Toyota Corolla benzines autómat kínálom bérlésre. 
                    Megbízható, egyszerű fenntartású és könnyen vezethető,
                    ideális városi közlekedéshez vagy rövidebb utakra. 
                    A korának megfelelő, de műszakilag rendben van, 
                    rendszeresen szervizelt. Ha egy kedvező árú, 
                    praktikus autót keresel a mindennapokra, 
                    jó választás lehet.`
    },
    {
        id: 3,
        title: "Toyota Corolla Sedan 1.8",
        brand: "Toyota",
        type: "Sedan",
        year: 2016,
        pricePerHour: 3100,
        pickup: "Nagykanizsa",
        county: "Zala vármegye",
        owner: "Kovács András",
        rating: 4.0,
        img: auto3,
        fuel: "Benzin",
        hp: 110,
        km: 200330,
        gearbox: "Manuális",
        description: `2016-os autómat adom bérbe, amely kényelmes és
                    jó választás a mindennapokra. Benzines, manuális 
                    váltós, könnyen vezethető és stabil úttartású. 
                    Rendszeresen karbantartott, megkímélt állapotban 
                    van. Városi használatra és hosszabb utakra is 
                    ideális, tágas csomagtartóval és kényelmes utastérrel.`
    },
    {
        id: 4,
        title: "Toyota Corolla 2.0 Dynamic Force",
        brand: "Toyota",
        type: "Sedan",
        year: 2021,
        pricePerHour: 4500,
        pickup: "Sopron",
        county: "Győr-Moson-Sopron vármegye",
        owner: "Mulga Krisztián",
        rating: 5.0,
        img: auto4,
        fuel: "Hybrid",
        hp: 130,
        km: 181000,
        gearbox: "Automata",
        description: `2016-os autómat adom bérbe, amely kényelmes és
                    jó választás a mindennapokra. Benzines, manuális 
                    váltós, könnyen vezethető és stabil úttartású. 
                    Rendszeresen karbantartott, megkímélt állapotban 
                    van. Városi használatra és hosszabb utakra is 
                    ideális, tágas csomagtartóval és kényelmes utastérrel.`
    },
    {
        id: 1,
        title: "Toyota Corolla 1.8",
        brand: "Toyota",
        type: "Sedan",
        year: 2019,
        pricePerHour: 3000,
        pickup: "Szombathely",
        county: "Vas vármegye",
        owner: "Németh Csaba",
        rating: 4.0,
        img: auto1,
        fuel: "Hybrid",
        hp: 120,
        km: 215000,
        gearbox: "Automata",
        description: `Saját, rendszeresen karbantartott Toyota 
                    Corolla 1.8 Hybrid autómat adom bérbe. Nagyon 
                    megbízható, csendes és rendkívül takarékos, főleg
                    városban, de hosszabb utakra is kényelmes választás.
                    Az autó tiszta, megkímélt állapotú, mindig 
                    odafigyelek rá, hogy műszakilag és esztétikailag 
                    is rendben legyen. Ha egy biztonságos és gazdaságos 
                    autót keresel, jó választás lehet számodra.`
    },
    {
        id: 2,
        title: "Toyota Corolla 1.8",
        brand: "Toyota",
        type: "Sedan",
        year: 2008,
        pricePerHour: 1250,
        pickup: "Zalaegerszeg",
        county: "Zala vármegye",
        owner: "Konrádi Ferenc",
        rating: 2.5,
        img: auto2,
        fuel: "Benzin",
        hp: 90,
        km: 304522,
        gearbox: "Manuális",
        description: `Toyota Corolla benzines autómat kínálom bérlésre. 
                    Megbízható, egyszerű fenntartású és könnyen vezethető,
                    ideális városi közlekedéshez vagy rövidebb utakra. 
                    A korának megfelelő, de műszakilag rendben van, 
                    rendszeresen szervizelt. Ha egy kedvező árú, 
                    praktikus autót keresel a mindennapokra, 
                    jó választás lehet.`
    },
    {
        id: 3,
        title: "Toyota Corolla Sedan 1.8",
        brand: "Toyota",
        type: "Sedan",
        year: 2016,
        pricePerHour: 3100,
        pickup: "Nagykanizsa",
        county: "Zala vármegye",
        owner: "Kovács András",
        rating: 4.0,
        img: auto3,
        fuel: "Benzin",
        hp: 110,
        km: 200330,
        gearbox: "Manuális",
        description: `2016-os autómat adom bérbe, amely kényelmes és
                    jó választás a mindennapokra. Benzines, manuális 
                    váltós, könnyen vezethető és stabil úttartású. 
                    Rendszeresen karbantartott, megkímélt állapotban 
                    van. Városi használatra és hosszabb utakra is 
                    ideális, tágas csomagtartóval és kényelmes utastérrel.`
    },
    {
        id: 4,
        title: "Toyota Corolla 2.0 Dynamic Force",
        brand: "Toyota",
        type: "Sedan",
        year: 2021,
        pricePerHour: 4500,
        pickup: "Sopron",
        county: "Győr-Moson-Sopron vármegye",
        owner: "Mulga Krisztián",
        rating: 5.0,
        img: auto4,
        fuel: "Hybrid",
        hp: 130,
        km: 181000,
        gearbox: "Automata",
        description: `2016-os autómat adom bérbe, amely kényelmes és
                    jó választás a mindennapokra. Benzines, manuális 
                    váltós, könnyen vezethető és stabil úttartású. 
                    Rendszeresen karbantartott, megkímélt állapotban 
                    van. Városi használatra és hosszabb utakra is 
                    ideális, tágas csomagtartóval és kényelmes utastérrel.`
    },
]

function normalize(s) {
    return (s || "").toString().trim().toLowerCase();
}

function Searching() {
    const navigate = useNavigate();
    const [loaded, setLoaded] = useState(false);
    const [start, setStart] = useState("");
    const [end, setEnd] = useState("");
    const [brand, setBrand] = useState("");
    const [type, setType] = useState("");
    const [minPrice, setMinPrice] = useState("");
    const [maxPrice, setMaxPrice] = useState("");
    const [pickup, setPickup] = useState("");
    const [submittedFilters, setSubmittedFilters] = useState(null);
    const [showFloatingTop, setShowFloatingTop] = useState(false);

    useLayoutEffect(() => window.scrollTo(0, 0), []);

    useEffect(() => {
        const id = requestAnimationFrame(() => setLoaded(true));
        const handleScroll = () => {
            if (window.scrollY > 400) {
                setShowFloatingTop(true);
            } else {
                setShowFloatingTop(false);
            }
        };

        window.addEventListener("scroll", handleScroll);
        return () => {
            cancelAnimationFrame(id);
            window.removeEventListener("scroll", handleScroll);
        };
    }, []);

    const filters = useMemo(
        () => ({
            start,
            end,
            brand,
            type,
            minPrice: minPrice ? Number(minPrice) : null,
            maxPrice: maxPrice ? Number(maxPrice) : null,
            pickup: pickup.trim(),
        }),
        [start, end, brand, type, minPrice, maxPrice, pickup]
    );

    const handleSearch = () => {
        if (filters.start && filters.end && new Date(filters.start) >= new Date(filters.end)) {
            alert("A bérlés vége nem lehet korábban vagy ugyanakkor, mint a kezdete.");
            return;
        }
        setSubmittedFilters(filters);
        requestAnimationFrame(() => {
            document.getElementById("search-results")?.scrollIntoView({ behavior: "smooth", block: "start" });
        });
    };

    const filteredCars = useMemo(() => {
        if (!submittedFilters) return [];
        const f = submittedFilters;
        const fBrand = normalize(f.brand);
        const fType = normalize(f.type);
        const fPickup = normalize(f.pickup);

        return CARS.filter((c) => {
            if (fBrand && normalize(c.brand) !== fBrand) return false;
            if (fType && normalize(c.type) !== fType) return false;
            if (f.minPrice != null && c.pricePerHour < f.minPrice) return false;
            if (f.maxPrice != null && c.pricePerHour > f.maxPrice) return false;
            if (fPickup && !normalize(`${c.pickup} ${c.county}`).includes(fPickup)) return false;
            return true;
        });
    }, [submittedFilters]);

    return (
        <div className="searching_root">
            {/* Hero Section */}
            <section className={`searching_hero searching_pageAnim ${loaded ? "isLoaded" : ""}`}>
                <div className="searching_dots" aria-hidden="true">
                    <span></span><span></span><span></span><span></span>
                    <span></span><span></span><span></span><span></span><span></span>
                </div>
                <div className="searching_container">
                    <div className="searching_content">
                        <div className="searching_left">
                            <h1>Bérelj olcsón,<br />biztonságosan!</h1>
                            <p>Találd meg az igényeidnek megfelelő autót, mellyel élvezet lesz a vezetés minden perce.</p>
                            <button className="searching_btn" type="button" onClick={() => navigate("/")}>Főoldal</button>
                        </div>
                        <div className="searching_right">
                            <div className="searching_carShadow" aria-hidden="true"></div>
                            <img className="searching_carImg" src="https://www.pngall.com/wp-content/uploads/8/White-SUV-PNG.png" alt="Autó" loading="lazy" />
                        </div>
                    </div>
                </div>
            </section>


            {/* Search Bar Section */}
            <section className={`searching_page searching_pageAnim ${loaded ? "isLoaded" : ""}`}>
                <div className="searching_searchWrap">
                    <h2 className="searching_searchTitle">Keressen autót most!</h2>
                    <div className="searching_searchBarOuter">
                        <div className="searching_searchBar">
                            <div className="searching_field">
                                <div className="searching_label">Bérlés kezdete</div>
                                <input className="searching_input" type="datetime-local" value={start} onChange={(e) => setStart(e.target.value)} />
                            </div>
                            <div className="searching_divider" />
                            <div className="searching_field">
                                <div className="searching_label">Bérlés vége</div>
                                <input className="searching_input" type="datetime-local" value={end} onChange={(e) => setEnd(e.target.value)} />
                            </div>
                            <div className="searching_divider" />
                            <div className="searching_field">
                                <div className="searching_label">Autó márka</div>
                                <select className="searching_input" value={brand} onChange={(e) => setBrand(e.target.value)}>
                                    <option value="">Bármely</option>
                                    <option value="Toyota">Toyota</option>
                                    <option value="BMW">BMW</option>
                                    <option value="Audi">Audi</option>
                                    <option value="Mercedes">Mercedes</option>
                                    <option value="Volkswagen">Volkswagen</option>
                                </select>
                            </div>
                            <div className="searching_divider" />
                            <div className="searching_field">
                                <div className="searching_label">Autó típusa</div>
                                <select className="searching_input" value={type} onChange={(e) => setType(e.target.value)}>
                                    <option value="">Bármely</option>
                                    <option value="Sedan">Sedan</option>
                                    <option value="Kombi">Kombi</option>
                                    <option value="SUV">SUV</option>
                                    <option value="Hatchback">Hatchback</option>
                                    <option value="Kisbusz">Kisbusz</option>
                                </select>
                            </div>
                            <div className="searching_divider" />
                            <div className="searching_field searching_fieldSmall">
                                <div className="searching_label">Min. ár</div>
                                <input className="searching_input" type="number" placeholder="Ft/óra" value={minPrice} onChange={(e) => setMinPrice(e.target.value)} />
                            </div>
                            <div className="searching_divider" />
                            <div className="searching_field searching_fieldSmall">
                                <div className="searching_label">Max. ár</div>
                                <input className="searching_input" type="number" placeholder="Ft/óra" value={maxPrice} onChange={(e) => setMaxPrice(e.target.value)} />
                            </div>
                            <div className="searching_divider" />
                            <div className="searching_field">
                                <div className="searching_label">Átvételi hely</div>
                                <input className="searching_input" type="text" placeholder="Város, megye" value={pickup} onChange={(e) => setPickup(e.target.value)} />
                            </div>
                            <button className="searching_searchBtn" type="button" onClick={handleSearch}>Keresés</button>
                        </div>
                    </div>

                    <div id="search-results" className="searching_results">
                        {submittedFilters && (
                            <>
                                <h2 className="searching_resultsTitle">Találatok ({filteredCars.length + " db"} )</h2>
                                <Cards cars={filteredCars} />
                                {filteredCars.length === 0 && (
                                    <p className="searching_noResults">Nincs találat a megadott feltételekre.</p>
                                )}
                            </>
                        )}
                    </div>
                </div>
            </section>


            {/* Footer Section */}
            <footer className="footer">
                <div className="footerInner">
                    <div className="footerContainer">
                        <div className="footerCol">
                            <h4>Kapcsolat</h4>
                            <div className="footerContact">
                                <p className="footerItem footerItem--loc">9700 Szombathely, Magyarország</p>
                                <p className="footerItem footerItem--phone">+36 20 123 4567</p>
                                <p className="footerItem footerItem--mail">comove@projekt.hu</p>
                            </div>
                            <div className="footerSocials">
                                <a href="https://www.facebook.com/profile.php?id=61586242866516" target="_blank" rel="noreferrer" aria-label="Facebook"><FaFacebookF /></a>
                                <a href="https://instagram.com" target="_blank" rel="noreferrer" aria-label="Instagram"><FaInstagram /></a>
                                <a href="https://tiktok.com" target="_blank" rel="noreferrer" aria-label="TikTok"><FaTiktok /></a>
                            </div>
                        </div>
                        <div className="footerCol">
                            <h4>Oldalak</h4>
                            <a href="/">Főoldal</a>
                            <a href="/searching">Autóbérlés</a>
                            <a href="/login">Fiókom</a>
                        </div>
                        <div className="footerCol">
                            <h4>Kövess minket</h4>
                            <a href="https://www.facebook.com/profile.php?id=61586242866516" target="_blank" rel="noreferrer">Facebook</a>
                            <a href="https://instagram.com" target="_blank" rel="noreferrer">Instagram</a>
                            <a href="https://tiktok.com" target="_blank" rel="noreferrer">TikTok</a>
                        </div>
                    </div>
                </div>
                <div className="footerBottom">
                    <div className="footerBottomInner">
                        <div className="footerCopy">
                            ©&nbsp;&nbsp;<span className="footerBrandName">CoMove</span>&nbsp;&nbsp;– Minden jog fenntartva.
                        </div>
                        <button
                            className={`footerToTop ${showFloatingTop ? "floating" : ""}`}
                            onClick={() => window.scrollTo({ top: 0, behavior: "smooth" })}
                        >
                            ↑
                        </button>
                    </div>
                </div>
            </footer>
        </div>
    );
}

export default Searching;



