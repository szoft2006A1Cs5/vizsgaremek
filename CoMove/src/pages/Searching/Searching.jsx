import "./Searching.css";
import { useLayoutEffect, useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { LoadingOverlay, SimpleGrid } from "@mantine/core";
import { DateTimePicker } from "@mantine/dates";
import VehicleCard from "../../components/common/VehicleCard/VehicleCard";
import "@mantine/dates/styles.css";
import { useMutation } from "@tanstack/react-query";
import { notifications } from "@mantine/notifications";
import 'dayjs/locale/hu'
import { fetchAPI } from "../../assets/scripts/Utilities";
import { useDateInputProps } from "../../assets/scripts/hooks/Hooks";

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
    const [fuelType, setFuelType] = useState("");
    const [transmission, setTransmission] = useState("");
    const [maxPrice, setMaxPrice] = useState("");
    const [pickup, setPickup] = useState("");
    const dateTimeInputProps = useDateInputProps('dateTime');

    useLayoutEffect(() => window.scrollTo(0, 0), []);

    useEffect(() => {
        const id = requestAnimationFrame(() => setLoaded(true));
        return () => cancelAnimationFrame(id);
    }, []);

    const filters = useMemo(
        () => ({
            start: start ?? null,
            end: end ?? null,
            brand,
            type,
            fuelType,
            transmission,
            maxPrice: maxPrice ? Number(maxPrice) : null,
            pickup: pickup.trim(),
        }),
        [start, end, brand, type, fuelType, transmission, maxPrice, pickup]
    );

    const searchMutation = useMutation({
        mutationFn: async (params) => {
            const resp = await fetchAPI(`/Vehicle?${params}`);

            if (!resp.ok) throw new Error("Nem sikerült lekérni a járműveket!");

            return resp.json();
        },
        onSuccess: () => {
            requestAnimationFrame(() => {
                document.getElementById("search-results")?.scrollIntoView({ behavior: "smooth", block: "start" });
            });
        },
        onError: (error) => notifications.show({ title: "Hiba!", message: error.message, color: "red" })
    });

    const handleSearch = () => {
        if (filters.start && filters.end && new Date(filters.start) >= new Date(filters.end)) {
            notifications.show({ 
                title: "Hiba!",
                message: "A bérlés vége nem lehet korábban vagy ugyanakkor, mint a kezdete.",
                color: "red"
            });
            return;
        }

        const fBrand = normalize(filters.brand);
        const fType = normalize(filters.type);
        const fPickup = normalize(filters.pickup);
        const fFuel = normalize(filters.fuelType);
        const fTransmission = normalize(filters.transmission);
        const startDate = new Date(filters.start);
        const endDate = new Date(filters.end);

        const params = new URLSearchParams();

        if (startDate && !isNaN(startDate)) params.set("rentalStart", startDate.toISOString());
        if (startDate && !isNaN(endDate)) params.set("rentalEnd", endDate.toISOString());
        if (fBrand) params.set("manufacturer", fBrand);
        if (fType) params.set("model", fType);
        if (fPickup) params.set("settlement", fPickup);
        if (fFuel) params.set("fuelType", fFuel);
        if (fTransmission) params.set("transmission", fTransmission);
        if (filters.maxPrice && !isNaN(filters.maxPrice)) params.set("maxPrice", filters.maxPrice);

        searchMutation.mutate(params);
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
                                    {...dateTimeInputProps}
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
                                    {...dateTimeInputProps}
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
                                <input className="searching_input" placeholder="pl. Toyota" value={brand} onInput={(e) => setBrand(e.target.value)} />
                            </div>
                            <div className="searching_divider" />
                            <div className="searching_field">
                                <div className="searching_label">Autó típusa</div>
                                <input className="searching_input" placeholder="pl. Corolla" value={type} onInput={(e) => setType(e.target.value)} />
                            </div>
                            <div className="searching_divider" />
                            <div className="searching_field searching_fieldSmall">
                                <div className="searching_label">Üzemanyag</div>
                                <input className="searching_input" placeholder="pl. Benzin" value={fuelType} onInput={(e) => setFuelType(e.target.value)} />
                            </div>
                            <div className="searching_divider" />
                            <div className="searching_field searching_fieldSmall">
                                <div className="searching_label">Váltó</div>
                                <input className="searching_input" placeholder="pl. Manuális" value={transmission} onInput={(e) => setTransmission(e.target.value)} />
                            </div>
                            <div className="searching_divider" />
                            <div className="searching_field searching_fieldSmall">
                                <div className="searching_label">Max. ár</div>
                                <input className="searching_input" type="number" placeholder="Ft" value={maxPrice} onInput={(e) => setMaxPrice(e.target.value)} />
                            </div>
                            <div className="searching_divider" />
                            <div className="searching_field">
                                <div className="searching_label">Átvételi hely</div>
                                <input className="searching_input" type="text" placeholder="Település" value={pickup} onInput={(e) => setPickup(e.target.value)} />
                            </div>
                            <button className="searching_searchBtn" type="button" onClick={handleSearch}>Keresés</button>
                        </div>
                    </div>

                    <div id="search-results" className="searching_results">
                        <LoadingOverlay visible={searchMutation.isPending} />
                        {searchMutation.data && (
                            <>
                                <h2 className="searching_resultsTitle">Találatok ({searchMutation.data.length + " db"})</h2>
                                <SimpleGrid cols={{
                                    base: 1,
                                    sm: 2,
                                    lg: 4,
                                }}>
                                    {searchMutation.data.map(car => {
                                        return (
                                            <VehicleCard key={car.id} vehicle={car} onClick={() => {
                                                navigate(`/vehicle/${car.id}?rentalStart=${filters.start ? new Date(filters.start).toISOString() : ""}&rentalEnd=${filters.end ? new Date(filters.end).toISOString() : ""}`)
                                            }} />
                                    )
                                    })}
                                </SimpleGrid>
                                {searchMutation.data.length === 0 && (
                                    <p className="searching_noResults">Nincs találat a megadott feltételekre.</p>
                                )}
                            </>
                        )}
                    </div>
                </div>
            </section>
        </div>
    );
}

export default Searching;



