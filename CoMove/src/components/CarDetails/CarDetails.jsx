import "./CarDetails.css";
import { useEffect } from "react";

function CarDetails({ car, open, onClose }) {
    useEffect(() => {
        if (!open) return;

        const onKeyDown = (e) => {
            if (e.key === "Escape") onClose?.();
        };
        window.addEventListener("keydown", onKeyDown);

        return () => window.removeEventListener("keydown", onKeyDown);
    }, [open, onClose]);

    if (!open || !car) return null;

    return (
        <div
            className="cdm_overlay"
            role="dialog"
            aria-modal="true"
            onMouseDown={(e) => {
                if (e.target === e.currentTarget) onClose?.();
            }}
        >
            <div className="cdm_modal">
                <button className="cdm_close" type="button" onClick={onClose} aria-label="Bezárás">
                    ×
                </button>

                <div className="cdm_body">
                    {/* BAL: KÉP */}
                    <div className="cdm_left">
                        <img className="cdm_img" src={car.img} alt={car.title} />
                    </div>

                    {/* JOBB: ADATOK */}
                    <div className="cdm_right">
                        <h2 className="cdm_title">{car.title}</h2>
                        <p className="cdm_ownerline">
                            Tulaj: <strong>{car.owner}</strong>
                        </p>

                        <ul className="cdm_infoList">
                            <li className="cdm_infoItem">
                                <span className="cdm_infoIcon"><i className="fas fa-map-marker-alt"></i></span>
                                <span>{car.pickup}</span>
                            </li>

                            <li className="cdm_infoItem">
                                <span className="cdm_infoIcon"><i className="fas fa-car"></i></span>
                                <span>{car.year}</span>
                            </li>

                            <li className="cdm_infoItem">
                                <span className="cdm_infoIcon"><i className="fas fa-gas-pump"></i></span>
                                <span>{car.fuel}</span>
                            </li>

                            <li className="cdm_infoItem">
                                <span className="cdm_infoIcon"><i className="fas fa-tachometer-alt"></i></span>
                                <span>{car.hp} LE</span>
                            </li>

                            <li className="cdm_infoItem">
                                <span className="cdm_infoIcon"><i className="fas fa-road"></i></span>
                                <span>{Number(car.km).toLocaleString("hu-HU")} km</span>
                            </li>

                            <li className="cdm_infoItem">
                                <span className="cdm_infoIcon"><i className="fas fa-cog"></i></span>
                                <span>{car.gearbox}</span>
                            </li>
                        </ul>

                        <div className="cdm_price">
                            {Number(car.pricePerHour).toLocaleString("hu-HU")} Ft <span>/ óra</span>
                        </div>

                        <div className="cdm_descWrap">
                            <h3 className="cdm_descTitle">Leírás</h3>
                            <p className="cdm_desc">
                                {car.description?.trim()
                                    ? car.description
                                    : "A tulajdonos nem adott meg leírást ehhez a járműhöz."}
                            </p>
                        </div>
                        <br />
                        <button className="cdm_rentBtn">
                            Bérlés megkezdése
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default CarDetails