import "./Searching.css";
import { useLayoutEffect, useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { FaFacebookF, FaInstagram, FaTiktok } from "react-icons/fa";
import { LoadingOverlay, SimpleGrid } from "@mantine/core";
import { DateTimePicker } from "@mantine/dates";
import CarCard from "../../components/CarCard/CarCard";
import { API_URL } from "../../assets/scripts/Config";
import "@mantine/dates/styles.css";

function normalize(s) {
    return (s || "").toString().trim().toLowerCase();
}

function Searching() {
    const navigate = useNavigate();
    const [loaded, setLoaded] = useState(false);
    const [start, setStart] = useState(null);
    const [end, setEnd] = useState(null);
    const [brand, setBrand] = useState("");
    const [type, setType] = useState("");
    const [minPrice, setMinPrice] = useState("");
    const [maxPrice, setMaxPrice] = useState("");
    const [pickup, setPickup] = useState("");
    const [showFloatingTop, setShowFloatingTop] = useState(false);
    const [cars, setCars] = useState(null);
    const [isLoading, setIsLoading] = useState(false);

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
            start: start ?? null,
            end: end ?? null,
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

        const fBrand = normalize(filters.brand);
        const fType = normalize(filters.type);
        const fPickup = normalize(filters.pickup);
        const startDate = new Date(filters.start);
        const endDate = new Date(filters.end);

        setIsLoading(true);

        const auth = JSON.parse(localStorage.getItem("auth"));
        const endpoint = new URL(`${API_URL}/Vehicle`);

        if (startDate && !isNaN(startDate)) endpoint.searchParams.set("rentalStart", startDate.toISOString());
        if (startDate && !isNaN(endDate)) endpoint.searchParams.set("rentalEnd", endDate.toISOString());
        if (fBrand) endpoint.searchParams.set("manufacturer", fBrand);
        if (fType) endpoint.searchParams.set("model", fType);
        if (fPickup) endpoint.searchParams.set("settlement", fPickup);
        if (filters.minPrice && !isNaN(filters.minPrice)) endpoint.searchParams.set("minRate", filters.minPrice);
        if (filters.maxPrice && !isNaN(filters.maxPrice)) endpoint.searchParams.set("maxRate", filters.maxPrice);

        fetch(endpoint.toString(), {
            headers: {
                Authorization: `Bearer ${auth?.token}`
            }
        })
        .then(resp => {
            setIsLoading(false);
            if (resp.status !== 200) {
                setCars(null);
                return null;
            }

            return resp.json();
        })
        .then(data => {
            if (!data) return;

            setCars(data);
            requestAnimationFrame(() => {
                document.getElementById("search-results")?.scrollIntoView({ behavior: "smooth", block: "start" });
            });
        })
        .catch(err => console.err(err));
    };

    return (
        <div className="searching_root">
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


            <section className={`searching_page searching_pageAnim ${loaded ? "isLoaded" : ""}`}>
                <div className="searching_searchWrap">
                    <h2 className="searching_searchTitle">Keressen autót most!</h2>
                    <div className="searching_searchBarOuter">
                        <div className="searching_searchBar">
                            <div className="searching_field searching_fieldDate">
                                <div className="searching_label">Bérlés kezdete</div>
                                <DateTimePicker
                                    variant="unstyled"
                                    placeholder="Dátum és idő"
                                    value={start}
                                    onChange={setStart}
                                    minDate={new Date()}
                                    valueFormat="YYYY. MM. DD. HH:mm"
                                    styles={{
                                        input: {
                                            fontSize: 13,
                                            fontWeight: 800,
                                            color: 'rgba(13,20,27,.88)',
                                            paddingLeft: 6,
                                            paddingRight: 0,
                                            minHeight: 'unset',
                                            height: 'auto',
                                            cursor: 'pointer',
                                        },
                                        section: { color: 'rgba(13,20,27,.35)' },
                                    }}
                                />
                            </div>
                            <div className="searching_divider" />
                            <div className="searching_field searching_fieldDate">
                                <div className="searching_label">Bérlés vége</div>
                                <DateTimePicker
                                    variant="unstyled"
                                    placeholder="Dátum és idő"
                                    value={end}
                                    onChange={setEnd}
                                    minDate={start ?? new Date()}
                                    valueFormat="YYYY. MM. DD. HH:mm"
                                    styles={{
                                        input: {
                                            fontSize: 13,
                                            fontWeight: 'bold',
                                            color: 'black',
                                            paddingLeft: 6,
                                            paddingRight: 0,
                                            minHeight: 'unset',
                                            height: 'auto',
                                            cursor: 'pointer',
                                        },
                                        section: { color: 'rgba(13,20,27,.35)' },
                                    }}
                                />
                            </div>
                            <div className="searching_divider" />
                            <div className="searching_field">
                                <div className="searching_label">Autó márka</div>
                                <input className="searching_input" placeholder="pl. Volkswagen" value={brand} onInput={(e) => setBrand(e.target.value)} />
                            </div>
                            <div className="searching_divider" />
                            <div className="searching_field">
                                <div className="searching_label">Autó típusa</div>
                                <input className="searching_input" placeholder="pl. Golf" value={type} onInput={(e) => setType(e.target.value)} />
                            </div>
                            <div className="searching_divider" />
                            <div className="searching_field searching_fieldSmall">
                                <div className="searching_label">Min. ár</div>
                                <input className="searching_input" type="number" placeholder="Ft/óra" value={minPrice} onInput={(e) => setMinPrice(e.target.value)} />
                            </div>
                            <div className="searching_divider" />
                            <div className="searching_field searching_fieldSmall">
                                <div className="searching_label">Max. ár</div>
                                <input className="searching_input" type="number" placeholder="Ft/óra" value={maxPrice} onInput={(e) => setMaxPrice(e.target.value)} />
                            </div>
                            <div className="searching_divider" />
                            <div className="searching_field">
                                <div className="searching_label">Átvételi hely</div>
                                <input className="searching_input" type="text" placeholder="Város" value={pickup} onInput={(e) => setPickup(e.target.value)} />
                            </div>
                            <button className="searching_searchBtn" type="button" onClick={handleSearch}>Keresés</button>
                        </div>
                    </div>

                    <div id="search-results" className="searching_results">
                        <LoadingOverlay visible={isLoading} />
                        {cars && (
                            <>
                                <h2 className="searching_resultsTitle">Találatok ({cars.length + " db"})</h2>
                                <SimpleGrid cols={{
                                    base: 1,
                                    sm: 2,
                                    lg: 4,
                                }}>
                                    {cars.map(car => {
                                        return (
                                            <CarCard key={car.id} car={car} onClick={() => {
                                                navigate(`/vehicle/${car.id}?rentalStart=${filters.start ? new Date(filters.start).toISOString() : ""}&rentalEnd=${filters.end ? new Date(filters.end).toISOString() : ""}`)
                                            }} />
                                    )
                                    })}
                                </SimpleGrid>
                                {cars.length === 0 && (
                                    <p className="searching_noResults">Nincs találat a megadott feltételekre.</p>
                                )}
                            </>
                        )}
                    </div>
                </div>
            </section>


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



