-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1:3307
-- Létrehozás ideje: 2026. Ápr 21. 22:33
-- Kiszolgáló verziója: 10.4.32-MariaDB
-- PHP verzió: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

DROP DATABASE IF EXISTS `comove`;

--
-- Adatbázis: `comove`
--
CREATE DATABASE IF NOT EXISTS `comove` DEFAULT CHARACTER SET utf8 COLLATE utf8_hungarian_ci;
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
(23, 'Szia! Köszi az ajánlatot, esetleg kezdődhetne kicsit később?', 0, '2026-04-21 20:24:32', 0, 1, 15),
(24, 'Szia! Persze, jó az 1 is.', 0, '2026-04-21 20:29:04', 0, 2, 15);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `notifications`
--

CREATE TABLE `notifications` (
  `id` int(11) NOT NULL,
  `userId` int(11) NOT NULL,
  `notificationId` int(11) NOT NULL,
  `content` varchar(512) NOT NULL,
  `timeSent` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `notifications`
--

INSERT INTO `notifications` (`id`, `userId`, `notificationId`, `content`, `timeSent`) VALUES
(57, 2, 1, 'A Toyota Corolla-ra/re vonatkozó bérlés vissza lett mondva!', '2026-04-21 22:19:56'),
(58, 1, 1, 'A Toyota Corolla-ra/re vonatkozó bérlés vissza lett mondva!', '2026-04-21 22:19:56'),
(62, 1, 4, 'Új bérlési kérelem érkezett Toyota Corolla járművedre.', '2026-04-21 22:23:51'),
(63, 2, 3, 'A bérlési ajánlat Teszt Elek-val/vel, a(z) Toyota Corolla-ra/re elfogadásra került.', '2026-04-21 22:29:07'),
(64, 1, 5, 'A bérlési ajánlat Gipsz Jakab-val/vel, a(z) Toyota Corolla-ra/re elfogadásra került.', '2026-04-21 22:29:07');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `rentals`
--

CREATE TABLE `rentals` (
  `id` int(11) NOT NULL,
  `rentalPrice` int(11) NOT NULL,
  `start` datetime NOT NULL,
  `end` datetime NOT NULL,
  `status` int(11) NOT NULL DEFAULT 0,
  `pickupLocation` varchar(512) NOT NULL,
  `renterRating` double DEFAULT NULL,
  `ownerRating` double DEFAULT NULL,
  `renterId` int(11) NOT NULL,
  `vehicleId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `rentals`
--

INSERT INTO `rentals` (`id`, `rentalPrice`, `start`, `end`, `status`, `pickupLocation`, `renterRating`, `ownerRating`, `renterId`, `vehicleId`) VALUES
(1, 15000, '2026-01-05 10:00:00', '2026-01-05 18:00:00', 8, '9700 Szombathely, Zrínyi Ilona utca 12.', 5, 4.5, 3, 1),
(5, 39200, '2026-03-24 11:00:00', '2026-03-26 12:00:00', 9, 'Szombathely, Fő tér 1.', NULL, NULL, 2, 1),
(15, 42400, '2026-04-29 11:00:00', '2026-05-01 16:00:00', 2, 'Szombathely, Fő tér 1.', NULL, NULL, 2, 1);

-- --------------------------------------------------------

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
  `driversLicenseNumber` varchar(10) DEFAULT NULL,
  `addressZipcode` varchar(4) NOT NULL,
  `addressSettlement` varchar(64) NOT NULL,
  `addressStreetHouse` varchar(64) NOT NULL,
  `balance` int(11) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `users`
--

INSERT INTO `users` (`id`, `idCardNumber`, `name`, `phone`, `dateOfBirth`, `profilePicPath`, `email`, `password`, `salt`, `role`, `driversLicenseNumber`, `addressZipcode`, `addressSettlement`, `addressStreetHouse`, `balance`) VALUES
(1, '123456AA', 'Teszt Elek', '36201234567', '2004-04-18', NULL, 'tesztelek@teszt.hu', 0x50a660f1a621afd4f176d317d4c773c3faa999457800896cfb569922eca979a9ff56e1520d06931917410cd2785084bddb0f7547aa9969857f832fec8f73c3b2, 0xe82c03cf87e46fa6f3418800ef361716, 'User', 'AA123456', '9700', 'Szombathely', 'Zrínyi Ilona utca 12.', 62045),
(2, '123456BB', 'Gipsz Jakab', '36701234567', '1995-07-21', NULL, 'gipszjakab@teszt.hu', 0xe4bef92d9367862aeda1cc5e6ec281794c0dc2efb3abdfeb10b09f021f97effcc0b7a60b9ff8673034d57ee59b8b1c614819213c5583fe498b7b3731d2c3c096, 0xcc819104175115d6f3a6c4fd2a02ca0a, 'User', 'BB123456', '1117', 'Budapest', 'Budafoki út 12.', 437455),
(3, '123456CC', 'Vincs Eszter', '36301234567', '2000-11-02', NULL, 'vincseszter@teszt.hu', 0xe7290419cd7a36ebc1af108550c260af3db4b55ee742c1839eafaca073127b43a652be5ad9632fd43035cc7f657c8b7afcc42cd2392f70e8f7b615704e2db118, 0x6439c25d98305dcf33f75ae705304611, 'User', 'CC123456', '9700', 'Szombathely', 'Kéthly Anna utca 7.', 0);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `usertokens`
--

CREATE TABLE `usertokens` (
  `id` int(11) NOT NULL,
  `userId` int(11) NOT NULL,
  `token` varchar(8) NOT NULL,
  `type` varchar(15) NOT NULL,
  `timeCreated` datetime NOT NULL DEFAULT utc_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `vehicleavailabilities`
--

CREATE TABLE `vehicleavailabilities` (
  `id` int(11) NOT NULL,
  `vehicleId` int(11) NOT NULL,
  `availabilityId` int(11) NOT NULL,
  `start` datetime NOT NULL,
  `end` datetime NOT NULL,
  `hourlyRate` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `vehicleavailabilities`
--

INSERT INTO `vehicleavailabilities` (`id`, `vehicleId`, `availabilityId`, `start`, `end`, `hourlyRate`) VALUES
(11, 1, 6, '2026-04-03 10:00:00', '2026-05-03 15:00:00', 800),
(21, 1, 7, '2026-05-03 15:00:00', '2026-05-08 10:00:00', 800),
(22, 1, 8, '2026-05-08 10:00:00', '2026-05-17 17:00:00', 900),
(23, 1, 9, '2026-05-17 17:00:00', '2026-05-22 10:00:00', 800),
(25, 1, 10, '2026-05-22 10:00:00', '2026-05-24 17:00:00', 900),
(26, 1, 11, '2026-05-24 17:00:00', '2026-05-29 10:00:00', 800),
(27, 1, 12, '2026-05-29 10:00:00', '2026-05-31 10:00:00', 1000),
(28, 2, 1, '2026-04-22 08:00:00', '2026-04-25 10:00:00', 800),
(29, 2, 2, '2026-04-27 16:00:00', '2026-04-30 10:00:00', 800),
(30, 2, 3, '2026-05-04 14:00:00', '2026-05-08 09:00:00', 800),
(31, 2, 4, '2026-05-11 09:00:00', '2026-05-15 09:00:00', 800),
(32, 2, 5, '2026-05-18 09:00:00', '2026-05-22 09:00:00', 800),
(33, 2, 6, '2026-05-25 09:00:00', '2026-05-29 09:00:00', 800),
(34, 3, 1, '2026-05-01 10:00:00', '2026-05-03 16:00:00', 1100),
(35, 3, 2, '2026-05-08 10:00:00', '2026-05-10 16:00:00', 800),
(36, 3, 3, '2026-05-15 10:00:00', '2026-05-17 16:00:00', 700),
(37, 3, 4, '2026-05-29 10:00:00', '2026-05-31 16:00:00', 800),
(38, 4, 1, '2026-05-07 10:00:00', '2026-05-09 16:00:00', 800),
(39, 4, 2, '2026-05-11 07:00:00', '2026-05-18 17:00:00', 900),
(40, 4, 3, '2026-05-25 06:00:00', '2026-05-29 15:00:00', 700),
(41, 4, 4, '2026-05-29 15:00:00', '2026-05-31 17:00:00', 900);

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
(1, 1, 1, 1, '58274674-91a9-4afb-ab80-52a0a605c520.jpg'),
(2, 2, 1, 1, '64725ad2-8502-4eef-b4f3-07d3c5af9423.jpg'),
(3, 3, 1, 1, '43b1c8fd-ce7c-4c34-8dfc-f46d2e12b6ba.png'),
(6, 4, 1, 1, 'c27ef141-b934-4520-94a3-7a6fd3bd7301.jpg'),
(11, 1, 2, 2, '135bf3d7-2166-4354-883a-43f51d323d5d.jpg');

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
  `horsepower` int(11) NOT NULL,
  `avgFuelConsumption` double NOT NULL,
  `fuelType` varchar(20) NOT NULL,
  `insuranceNumber` varchar(64) NOT NULL,
  `transmission` varchar(16) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- A tábla adatainak kiíratása `vehicles`
--

INSERT INTO `vehicles` (`id`, `ownerId`, `vin`, `licensePlate`, `manufacturer`, `model`, `year`, `description`, `odometerReading`, `horsepower`, `avgFuelConsumption`, `fuelType`, `insuranceNumber`, `transmission`) VALUES
(1, 1, 'VF312345678901234', 'ABC123', 'Toyota', 'Corolla', 2018, 'Megbízható hibrid városi cirkáló.', 85000, 132, 4.5, 'benzin-elektromos', 'KGFB-998877', 'automatikus'),
(2, 2, 'WBA41234567890123', 'SKY789', 'BMW', '320d', 2015, 'Kényelmes utazóautó hosszabb távra.', 210000, 180, 6.2, 'dízel', 'KGFB-112233', 'manuális'),
(3, 3, 'TMB51234567890123', 'RNL456', 'Skoda', 'Octavia', 2020, 'Hatalmas csomagtartó, tiszta belső.', 45000, 150, 5.5, 'benzin', 'KGFB-445566', 'manuális'),
(4, 1, 'DEFGHIJK12341241A', 'ABCD123', 'Suzuki', 'Swift', 2018, '', 5, 120, 6.2, 'Benzin', 'KGFB-123412515', 'Manuális');

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
-- A tábla indexei `usertokens`
--
ALTER TABLE `usertokens`
  ADD PRIMARY KEY (`id`),
  ADD KEY `usertokens_users_id_fk` (`userId`);

--
-- A tábla indexei `vehicleavailabilities`
--
ALTER TABLE `vehicleavailabilities`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `vehicleavailability` (`vehicleId`,`availabilityId`);

--
-- A tábla indexei `vehicleimages`
--
ALTER TABLE `vehicleimages`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `vehicleimage` (`vehicleId`,`imageId`);

--
-- A tábla indexei `vehicles`
--
ALTER TABLE `vehicles`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `vehicles_pk` (`vin`),
  ADD UNIQUE KEY `vehicles_pk_2` (`licensePlate`),
  ADD KEY `owner` (`ownerId`);

--
-- A kiírt táblák AUTO_INCREMENT értéke
--

--
-- AUTO_INCREMENT a táblához `messages`
--
ALTER TABLE `messages`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=25;

--
-- AUTO_INCREMENT a táblához `notifications`
--
ALTER TABLE `notifications`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=65;

--
-- AUTO_INCREMENT a táblához `rentals`
--
ALTER TABLE `rentals`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- AUTO_INCREMENT a táblához `users`
--
ALTER TABLE `users`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT a táblához `usertokens`
--
ALTER TABLE `usertokens`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT a táblához `vehicleavailabilities`
--
ALTER TABLE `vehicleavailabilities`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=42;

--
-- AUTO_INCREMENT a táblához `vehicleimages`
--
ALTER TABLE `vehicleimages`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- AUTO_INCREMENT a táblához `vehicles`
--
ALTER TABLE `vehicles`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- Megkötések a kiírt táblákhoz
--

--
-- Megkötések a táblához `messages`
--
ALTER TABLE `messages`
  ADD CONSTRAINT `messages_ibfk_1` FOREIGN KEY (`senderId`) REFERENCES `users` (`id`),
  ADD CONSTRAINT `messages_ibfk_2` FOREIGN KEY (`rentalId`) REFERENCES `rentals` (`id`) ON DELETE CASCADE;

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
-- Megkötések a táblához `usertokens`
--
ALTER TABLE `usertokens`
  ADD CONSTRAINT `usertokens_users_id_fk` FOREIGN KEY (`userId`) REFERENCES `users` (`id`);

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
