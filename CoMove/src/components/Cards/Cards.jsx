import "./Cards.css";
import { useEffect, useState } from "react";
import CarDetails from "../CarDetails/CarDetails";
import { Rating } from "@mantine/core";

function Cards({ cars = [] }) {
    const [favorites, setFavorites] = useState(() => new Set());

    // animáció vezérlés
    const [visible, setVisible] = useState(false);
    const [animKey, setAnimKey] = useState(0);
    const [selectedCar, setSelectedCar] = useState(null);

    useEffect(() => {
        setVisible(false);
        setAnimKey((k) => k + 1);

        const t = setTimeout(() => setVisible(true), 50);
        return () => clearTimeout(t);
    }, [cars]);

    const toggleFav = (id) => {
        setFavorites((prev) => {
            const next = new Set(prev);
            if (next.has(id)) next.delete(id);
            else next.add(id);
            return next;
        });
    };

    return (
        <>
            <div className="wrap" key={animKey}>
                <section className={`grid ${visible ? "grid-show" : ""}`} id="grid">
                    {cars.map((car, index) => (
                        <article
                            className={`card ${visible ? "card-show" : ""}`}
                            key={car.id}
                            style={{ "--delay": `${index * 90}ms` }}
                            onClick={() => setSelectedCar(car)}
                        >
                            {/* EZ AZ ÚJ WRAPPER */}
                            <div className="card-inner">
                                <div className="img">
                                    <img alt={car.title} src={car.img} />
                                </div>

                                <div className="content">
                                    <h3 className="title">{car.title}</h3>
                                    <p className="meta">
                                        <i className="fas fa-map-marker-alt"></i>&nbsp;&nbsp;{car.county},&nbsp;
                                        <span className="owner">{car.owner}</span>
                                    </p>

                                    <div className="stars">
                                        <Rating
                                            value={Number(car.rating) || 0}
                                            fractions={10}
                                            readOnly
                                            size="sm"
                                            color="#ffc219"
                                        />
                                        <span className="rating-value">
                                            {(Number(car.rating) || 0).toFixed(1)}
                                        </span>
                                    </div>

                                    <div className="pillek">
                                        <span className="pillmain">{car.pickup}</span>
                                        <span className="pill">{car.year}</span>
                                        <span className="pill">{car.fuel}</span>
                                    </div>

                                    <div className="nextrow">
                                        <span className="pill">{car.hp}LE</span>
                                        <span className="pill">{Number(car.km).toLocaleString("hu-HU")} km</span>
                                        <span className="pill">{car.gearbox}</span>
                                    </div>

                                    <div className="row">
                                        <div className="price">
                                            <strong>
                                                {Number(car.pricePerHour).toLocaleString("hu-HU")} Ft <span>/ óra</span>
                                            </strong>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </article>
                    ))}
                </section>
            </div>
            <CarDetails
                car={selectedCar}
                open={!!selectedCar}
                onClose={() => setSelectedCar(null)}
            />
        </>
    );
}

export default Cards;