-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2026. Feb 23. 09:04
-- Kiszolgáló verziója: 10.4.32-MariaDB
-- PHP verzió: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Adatbázis: `comove`
--
CREATE DATABASE IF NOT EXISTS `comove` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_hungarian_ci;
USE `comove`;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `messages`
--

CREATE TABLE `messages` (
  `id` int(11) NOT NULL,
  `content` varchar(512) NOT NULL,
  `isImage` tinyint(1) NOT NULL DEFAULT 0,
  `timeSent` datetime NOT NULL,
  `isComplaint` tinyint(1) NOT NULL,
  `senderId` int(11) NOT NULL,
  `rentalId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `messages`
--

INSERT INTO `messages` (`id`, `content`, `isImage`, `timeSent`, `isComplaint`, `senderId`, `rentalId`) VALUES
(1, 'Szia! Megérkeztem az autóhoz, minden rendben tűnik.', 0, '2026-01-11 08:55:00', 0, 4, 2),
(2, 'Szuper, a kulcs a kijelölt helyen volt?', 0, '2026-01-11 08:57:00', 0, 2, 2);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `notifications`
--

CREATE TABLE `notifications` (
  `id` int(11) NOT NULL,
  `userId` int(11) NOT NULL,
  `content` varchar(512) NOT NULL,
  `timeSent` datetime NOT NULL,
  `read` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `notifications`
--

INSERT INTO `notifications` (`id`, `userId`, `content`, `timeSent`, `read`) VALUES
(1, 2, 'Új üzeneted érkezett Teszt Teréztől a BMW bérléssel kapcsolatban.', '2026-01-11 08:55:05', 1),
(2, 3, 'Új bérlési ajánlatod érkezett a Skodára Teszt Elektől!', '2026-01-12 00:05:00', 0);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `rentals`
--

CREATE TABLE `rentals` (
  `id` int(11) NOT NULL,
  `fullPrice` int(11) NOT NULL,
  `downpayment` int(11) NOT NULL,
  `start` datetime NOT NULL,
  `end` datetime NOT NULL,
  `status` int(11) NOT NULL DEFAULT 0,
  `pickupLatitude` double NOT NULL,
  `pickupLongtitude` double NOT NULL,
  `fuelLevel` float DEFAULT NULL,
  `renterRating` double DEFAULT NULL,
  `ownerRating` double DEFAULT NULL,
  `renterId` int(11) NOT NULL,
  `vehicleId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `rentals`
--

INSERT INTO `rentals` (`id`, `fullPrice`, `downpayment`, `start`, `end`, `status`, `pickupLatitude`, `pickupLongtitude`, `fuelLevel`, `renterRating`, `ownerRating`, `renterId`, `vehicleId`) VALUES
(1, 15000, 3000, '2026-01-05 10:00:00', '2026-01-05 18:00:00', 8, 47.1234, 18.4567, 100, 5, 4.5, 3, 1),
(2, 25000, 5000, '2026-01-11 09:00:00', '2026-01-13 17:00:00', 5, 47.4979, 19.0402, 75.5, NULL, NULL, 4, 2),
(3, 8000, 1500, '2026-01-15 08:00:00', '2026-01-15 12:00:00', 1, 47.2345, 16.6321, NULL, NULL, NULL, 1, 3);

--
-- Tábla szerkezet ehhez a táblához `users`
--

CREATE TABLE `users` (
  `id` int(11) NOT NULL,
  `idCardNumber` varchar(8) NOT NULL,
  `name` varchar(64) NOT NULL,
  `phone` varchar(11) NOT NULL,
  `dateOfBirth` date NOT NULL,
  `profilePicPath` varchar(256) DEFAULT NULL,
  `email` varchar(64) NOT NULL,
  `password` blob NOT NULL,
  `salt` blob NOT NULL,
  `role` enum('User','Administrator') NOT NULL DEFAULT 'User',
  `driversLicenseNumber` varchar(10) NOT NULL,
  `driversLicenseDate` date NOT NULL,
  `addressZipcode` varchar(4) NOT NULL,
  `addressSettlement` varchar(64) NOT NULL,
  `addressStreetHouse` varchar(64) NOT NULL,
  `balance` int(11) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `users`
--

INSERT INTO `users` (`id`, `idCardNumber`, `name`, `phone`, `dateOfBirth`, `profilePicPath`, `email`, `password`, `salt`, `role`, `driversLicenseNumber`, `driversLicenseDate`, `addressZipcode`, `addressSettlement`, `addressStreetHouse`, `balance`) VALUES
(1, '123456AA', 'Teszt Elek', '36201234567', '2004-04-18', NULL, 'tesztelek@teszt.hu', 0x5cd79118803c295ee4566a87a59423e7b4b020194520f52e78bddcdbfb36daef43b032e7a122323e5849344ff1fd625a7885c3ff62688a0241b7e4018ed3d9e0, 0x945200f84cef838d8d44e0121415fa53, 'User', 'AA123456', '2024-02-04', '9700', 'Szombathely', 'Zrínyi Ilona utca 12.', 0),
(2, '123456BB', 'Gipsz Jakab', '36701234567', '1995-07-21', NULL, 'gipszjakab@teszt.hu', 0xd08e0fc7c893f8e40935a39f87be3b90c43d63f563ff851e64469d0cdd468dd0ee73881850bb01a97a084fdaae5b634816dabd10c2b41dcbabef3705adbf16d9, 0x881c6de362d428ab7db9241bbe10d4ac, 'User', 'BB123456', '2017-03-12', '1117', 'Budapest', 'Budafoki út 12.', 0),
(3, '123456CC', 'Vincs Eszter', '36301234567', '2000-11-02', NULL, 'vincseszter@teszt.hu', 0xe9699389486299dda48bda984e52ca3b8d1d13a16ce95ebea830ca087980920867bfd72fc2846cb600cf8c4157c7af4059d6ca032292c947614ebe716c969493, 0xb1c0e441552bef260ef5b1d075e2c871, 'User', 'CC123456', '2019-10-09', '9700', 'Szombathely', 'Kéthly Anna utca 7.', 0),
(4, '123456DD', 'Teszt Teréz', '36707654321', '1989-12-12', NULL, 'tesztterez@teszt.hu', 0xb1fa19e5947dd5b0395e12c4a41fd93c723587e753db40e10003c27e9635f1dd5951748bc18effae1e42fc1df5fafa7b30d37390f01dfca893613ad58042516b, 0x04c98372f19591c29d3416242bdd64e6, 'User', 'DD123456', '2014-08-19', '1095', 'Budapest', 'Tinódi utca 1.', 0);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `vehicleavailabilities`
--

CREATE TABLE `vehicleavailabilities` (
  `id` int(11) NOT NULL,
  `vehicleId` int(11) NOT NULL,
  `start` datetime NOT NULL,
  `end` datetime NOT NULL,
  `recurrence` enum('None','Weekly','Biweekly','Monthly') NOT NULL DEFAULT 'None',
  `hourlyRate` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `vehicleavailabilities`
--

INSERT INTO `vehicleavailabilities` (`id`, `vehicleId`, `start`, `end`, `recurrence`, `hourlyRate`) VALUES
(1, 1, '2026-01-01 08:00:00', '2026-12-31 20:00:00', 'None', 1800),
(1, 2, '2026-01-10 00:00:00', '2026-02-10 00:00:00', 'None', 2500),
(1, 3, '2026-01-12 08:00:00', '2026-01-20 20:00:00', 'Weekly', 2000);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `vehicleimages`
--

CREATE TABLE `vehicleimages` (
  `id` int(11) NOT NULL,
  `vehicleId` int(11) NOT NULL,
  `imageId` int(11) NOT NULL,
  `sortIndex` int(11) NOT NULL,
  `path` varchar(2048) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `vehicleimages`
--

INSERT INTO `vehicleimages` (`id`, `vehicleId`, `imageId`, `sortIndex`, `path`) VALUES
(1, 1, 1, 0, 'uploads/vehicles/toyota_corolla_front.jpg'),
(2, 1, 2, 1, 'uploads/vehicles/bmw_320d_side.png'),
(3, 1, 3, 2, 'uploads/vehicles/skoda_octavia.jpg');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `vehicles`
--

CREATE TABLE `vehicles` (
  `id` int(11) NOT NULL,
  `ownerId` int(11) NOT NULL,
  `vin` varchar(17) NOT NULL,
  `licensePlate` varchar(7) NOT NULL,
  `manufacturer` varchar(16) NOT NULL,
  `model` varchar(32) NOT NULL,
  `year` int(11) NOT NULL,
  `description` varchar(512) NOT NULL,
  `odometerReading` int(11) NOT NULL,
  `avgFuelConsumption` double NOT NULL,
  `insuranceNumber` varchar(64) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `vehicles`
--

INSERT INTO `vehicles` (`id`, `ownerId`, `vin`, `licensePlate`, `manufacturer`, `model`, `year`, `description`, `odometerReading`, `avgFuelConsumption`, `insuranceNumber`) VALUES
(1, 1, 'VF312345678901234', 'ABC-123', 'Toyota', 'Corolla', 2018, 'Megbízható hibrid városi cirkáló.', 85000, 4.5, 'KGFB-998877'),
(2, 2, 'WBA41234567890123', 'SKY-789', 'BMW', '320d', 2015, 'Kényelmes utazóautó hosszabb távra.', 210000, 6.2, 'KGFB-112233'),
(3, 3, 'TMB51234567890123', 'RNL-456', 'Skoda', 'Octavia', 2020, 'Hatalmas csomagtartó, tiszta belső.', 45000, 5.5, 'KGFB-445566');

--
-- Indexek a kiírt táblákhoz
--

--
-- A tábla indexei `messages`
--
ALTER TABLE `messages`
  ADD PRIMARY KEY (`id`),
  ADD KEY `sender` (`senderId`),
  ADD KEY `rental` (`rentalId`);

--
-- A tábla indexei `notifications`
--
ALTER TABLE `notifications`
  ADD PRIMARY KEY (`id`),
  ADD KEY `user` (`userId`);

--
-- A tábla indexei `rentals`
--
ALTER TABLE `rentals`
  ADD PRIMARY KEY (`id`),
  ADD KEY `renter` (`renterId`),
  ADD KEY `vehicle` (`vehicleId`);

--
-- A tábla indexei `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`id`);

--
-- A tábla indexei `vehicleavailabilities`
--
ALTER TABLE `vehicleavailabilities`
  ADD PRIMARY KEY (`vehicleId`,`id`);

--
-- A tábla indexei `vehicleimages`
--
ALTER TABLE `vehicleimages`
  ADD PRIMARY KEY (`id`)
  ADD UNIQUE INDEX `vehicleimage` (`vehicleId`, `imageId`);

--
-- A tábla indexei `vehicles`
--
ALTER TABLE `vehicles`
  ADD PRIMARY KEY (`id`),
  ADD KEY `owner` (`ownerId`);

--
-- A kiírt táblák AUTO_INCREMENT értéke
--

--
-- AUTO_INCREMENT a táblához `messages`
--
ALTER TABLE `messages`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT a táblához `notifications`
--
ALTER TABLE `notifications`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT a táblához `rentals`
--
ALTER TABLE `rentals`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT a táblához `users`
--
ALTER TABLE `users`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT a táblához `vehicleimages`
--
ALTER TABLE `vehicleimages`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT a táblához `vehicles`
--
ALTER TABLE `vehicles`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- Megkötések a kiírt táblákhoz
--

--
-- Megkötések a táblához `messages`
--
ALTER TABLE `messages`
  ADD CONSTRAINT `messages_ibfk_1` FOREIGN KEY (`senderId`) REFERENCES `users` (`id`),
  ADD CONSTRAINT `messages_ibfk_2` FOREIGN KEY (`rentalId`) REFERENCES `rentals` (`id`);

--
-- Megkötések a táblához `notifications`
--
ALTER TABLE `notifications`
  ADD CONSTRAINT `notifications_ibfk_1` FOREIGN KEY (`userId`) REFERENCES `users` (`id`);

--
-- Megkötések a táblához `rentals`
--
ALTER TABLE `rentals`
  ADD CONSTRAINT `rentals_ibfk_1` FOREIGN KEY (`vehicleId`) REFERENCES `vehicles` (`id`),
  ADD CONSTRAINT `rentals_ibfk_2` FOREIGN KEY (`renterId`) REFERENCES `users` (`id`);

--
-- Megkötések a táblához `vehicleavailabilities`
--
ALTER TABLE `vehicleavailabilities`
  ADD CONSTRAINT `vehicleavailabilities_ibfk_1` FOREIGN KEY (`vehicleId`) REFERENCES `vehicles` (`id`);

--
-- Megkötések a táblához `vehicleimages`
--
ALTER TABLE `vehicleimages`
  ADD CONSTRAINT `vehicleimages_ibfk_1` FOREIGN KEY (`vehicleId`) REFERENCES `vehicles` (`id`);

--
-- Megkötések a táblához `vehicles`
--
ALTER TABLE `vehicles`
  ADD CONSTRAINT `vehicles_ibfk_1` FOREIGN KEY (`ownerId`) REFERENCES `users` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
