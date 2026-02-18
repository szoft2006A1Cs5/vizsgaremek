import { useState, useEffect } from "react";
import { useNavigate, useLocation, Link, Route } from "react-router-dom";
import { Text, Anchor, Group, Menu, Card, Image, Badge, Flex, Rating, Title } from '@mantine/core';
import { Carousel } from '@mantine/carousel';
import defaultCarImage from '../../assets/kepek/defaultCarImage.png'
import style from './CarCard.module.css';
import '@mantine/carousel/styles.css';

function CarCard(carInput) {
    const [car, setCar] = useState(carInput.carInput);

    return <>
        <Card className={style.car} shadow="sm" padding="lg" radius="md" withBorder w={350}>
            { car.images ?
                <Card.Section>
                    <Carousel withControls withIndicators>
                        {car.images.map((x, i) => <Carousel.Slide key={i}><Image src={x.imagePath} /></Carousel.Slide>)}
                    </Carousel>
                </Card.Section>
                : 
                <Image src={defaultCarImage} />
            }

            <Flex direction="column" justify="space-between" gap={5} mt="md" mb="xs">
                <Flex direction="row" justify="space-between" align="baseline">
                    <Title>{car.manufacturer} {car.model}</Title>
                    <Text>{car.year}</Text>
                </Flex>
                { car.owner ? <Text size="md">{car.owner.name}</Text> : <></> }
                { car.rating ? <Rating defaultValue={car.rating} fractions={10} readOnly /> : <></> }
            </Flex>

            <Text>
                {car.description}
            </Text>

            <Flex justify="space-between" wrap="wrap" mt="md" mb="xs" gap={5}>
                { car.owner ? <Badge color="blue">{car.owner.addressSettlement}</Badge> : <></> }
                <Badge color="blue">{car.odometerReading} km</Badge>
                <Badge color="blue">{car.avgFuelConsumption} L/100km</Badge>
            </Flex>
        </Card>
    </>
}

export default CarCard;