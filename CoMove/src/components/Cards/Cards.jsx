import { SimpleGrid } from "@mantine/core";
import CarCard from "../CarCard/CarCard";
import { useNavigate } from "react-router-dom";

function Cards({ cars = [] }) {
    const navigate = useNavigate();

    return (
        <>
            <SimpleGrid cols={{
                base: 1,
                sm: 2,
                lg: 4,
            }}>
                {cars.map(car => {
                    return (
                        <CarCard key={car.id} car={car} onClick={() => {
                            navigate(`/vehicle/${car.id}`)
                        }} />
                )
                })}
            </SimpleGrid>
        </>
    );
}

export default Cards;