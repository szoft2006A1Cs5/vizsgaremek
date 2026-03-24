import { useState, useEffect } from 'react'
import { Badge, Flex, Rating } from "@mantine/core";
import defaultImage from '../../assets/kepek/egyeb/default.png'
import './CarCard.css'
import { FaMapMarkerAlt } from 'react-icons/fa';

function CarCard({ car, onClick = () => {} }) {
    const carImg = car.images[0]?.path ? `https://localhost:7245/${car.images[0]?.path}` : defaultImage;

    return (
        <div className="card" onClick={onClick}>
            <div className="img">
                <img alt={`${car.manufacturer} ${car.model}`} src={carImg} />
            </div>

            <div className="content">
                <h3 className="title">{car.manufacturer} {car.model}</h3>
                <p className="meta">
                    <FaMapMarkerAlt /> {car.owner.addressSettlement}, <span className="owner">{car.owner.name}</span>
                </p>

                <div className="stars">
                    <Rating
                        value={Number(car.rating) || 0}
                        fractions={10}
                        readOnly
                        size="sm"
                    />
                    <span className="rating-value">
                        {(Number(car.rating) || 0).toFixed(1)}
                    </span>
                </div>

                <Flex wrap='wrap' gap={5} mt={15}> 
                    <span className="pillmain">{car.owner.addressSettlement}</span>
                    <span className="pill">{car.year}</span>
                    <span className="pill">{car.fuelType}</span>
                    <span className="pill">{car.horsepower}LE</span>
                    <span className="pill">{Number(car.odometerReading).toLocaleString("hu-HU")} km</span>
                    <span className="pill">{car.transmission}</span>
                </Flex>

                { car.quote ?
                    <div className="row">
                        <div className="price">
                            <strong>
                                {Number(car.quote.fullPrice).toLocaleString("hu-HU")} Ft
                            </strong>
                        </div>
                    </div>
                :
                    <></>
                }
            </div>
        </div>
    )
}

export default CarCard;