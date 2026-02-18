import { useState, useEffect } from 'react'
import CarCard from '../CarCard/CarCard'
import { Flex } from '@mantine/core';

function Search() {
    const [cars, setCars] = useState(null);
    useEffect(() => {
        let token = localStorage.getItem("token");
        
        fetch("https://localhost:7245/api/Vehicle", {
            method: "GET",
            headers: {
                "Authorization": (token ? `Bearer ${token}` : ""),
            }
        })
        .then(resp => {
            if (resp.status !== 200) {
                alert("Hiba!")
                return null;
            }

            return resp.json();
        })
        .then(data => {
            if (!data) return;

            setCars(data);
        })
        .catch(err => {});
    }, [])

    return <>
        <Flex direction="row" wrap="wrap" gap={50} justify="space-evenly">
            { cars ? cars.map((x, i) => <CarCard key={i} carInput={x} />) : <></> }
        </Flex>
    </>
}

export default Search;