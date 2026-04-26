import { Badge, Flex, Rating } from "@mantine/core";
import defaultImage from '../../../assets/kepek/egyeb/default.png'
import './VehicleCard.css'
import { FaMapMarkerAlt } from 'react-icons/fa';
import { BACKEND_URL } from '../../../assets/scripts/Config';
import { formatPic } from '../../../assets/scripts/Utilities';

function VehicleCard({ vehicle, onClick = () => {} }) {
    const vehicleImg = formatPic(vehicle.images[0]?.path) ?? defaultImage;

    return (
        <div className="card" onClick={onClick}>
            <div className="img">
                <img alt={`${vehicle.manufacturer} ${vehicle.model}`} src={vehicleImg} />
            </div>

            <div className="content">
                <h3 className="title">{vehicle.manufacturer} {vehicle.model}</h3>
                <p className="meta">
                    <FaMapMarkerAlt /> {vehicle.owner.addressSettlement}, <span className="owner">{vehicle.owner.name}</span>
                </p>

                <div className="stars">
                    <Rating
                        value={Number(vehicle.rating) || 0}
                        fractions={10}
                        readOnly
                        size="sm"
                    />
                    <span className="rating-value">
                        {(Number(vehicle.rating) || 0).toFixed(1)}
                    </span>
                </div>

                <Flex wrap='wrap' gap={5} mt={15}> 
                    <span className="pillmain">{vehicle.owner.addressSettlement}</span>
                    <span className="pill">{vehicle.year}</span>
                    <span className="pill">{vehicle.fuelType}</span>
                    <span className="pill">{vehicle.horsepower}LE</span>
                    <span className="pill">{Number(vehicle.odometerReading).toLocaleString("hu-HU")} km</span>
                    <span className="pill">{vehicle.transmission}</span>
                </Flex>

                { vehicle.quote ?
                    <div className="row">
                        <div className="price">
                            <strong>
                                {Number(vehicle.quote.fullPrice).toLocaleString("hu-HU")} Ft
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

export default VehicleCard;