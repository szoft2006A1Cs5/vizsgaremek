-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2026. Jan 06. 07:52
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

DELIMITER $$
--
-- Eljárások
--
CREATE DEFINER=`root`@`localhost` PROCEDURE `regiErtesitesekTorlese` (IN `param` INT)   BEGIN
	DELETE FROM ertesites
    WHERE ertesites.olvasva = 1
    AND ertesites.kuldesIdeje < NOW() - INTERVAL 30 DAY;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `berles`
--

CREATE TABLE `berles` (
  `id` int(11) NOT NULL,
  `teljesAr` int(11) NOT NULL,
  `letet` int(11) NOT NULL,
  `kezdet` datetime NOT NULL,
  `veg` datetime NOT NULL,
  `allapot` enum('BerloJavaslat','BerbeadoJavaslat','Elfogadva','BerloAtvetelElfogadva','BerbeadoAtvetelElfogadva','Aktiv','BerloLezarasElfogadva','BerbeadoLezarasElfogadva','Lezarva','Visszamondva') NOT NULL DEFAULT 'BerloJavaslat',
  `atveteliHelySzelesseg` double NOT NULL,
  `atveteliHelyHosszusag` double NOT NULL,
  `uzemanyagszint` float DEFAULT NULL,
  `berloErtekeles` double DEFAULT NULL,
  `berbeadoErtekeles` double DEFAULT NULL,
  `berloId` int(11) NOT NULL,
  `jarmuId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `ertesites`
--

CREATE TABLE `ertesites` (
  `id` int(11) NOT NULL,
  `felhasznaloId` int(11) NOT NULL,
  `szoveg` varchar(512) NOT NULL,
  `kuldesIdeje` datetime NOT NULL,
  `olvasva` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `felhasznalo`
--

CREATE TABLE `felhasznalo` (
  `id` int(11) NOT NULL,
  `szemelyiSzam` varchar(8) NOT NULL,
  `nev` varchar(64) NOT NULL,
  `telefonszam` varchar(11) NOT NULL,
  `szuletesiDatum` date NOT NULL,
  `profilKepEleresiUt` varchar(256) DEFAULT NULL,
  `email` varchar(64) NOT NULL,
  `jelszo` varchar(128) NOT NULL,
  `jogosultsag` enum('Felhasznalo','Adminisztrator') NOT NULL DEFAULT 'Felhasznalo',
  `jogositvanySzam` varchar(10) NOT NULL,
  `jogositvanyKiallitasDatum` date NOT NULL,
  `cimIranyitoszam` varchar(4) NOT NULL,
  `cimTelepules` varchar(64) NOT NULL,
  `cimUtcaHazszam` varchar(64) NOT NULL,
  `egyenleg` int(11) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `jarmu`
--

CREATE TABLE `jarmu` (
  `id` int(11) NOT NULL,
  `tulajdonosId` int(11) NOT NULL,
  `alvazszam` varchar(17) NOT NULL,
  `rendszam` varchar(7) NOT NULL,
  `marka` varchar(16) NOT NULL,
  `tipus` varchar(32) NOT NULL,
  `evjarat` int(11) NOT NULL,
  `leiras` varchar(512) NOT NULL,
  `kmAllas` int(11) NOT NULL,
  `atlagFogyasztas` double NOT NULL,
  `biztositasiSzam` varchar(64) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `jarmuberelhetoseg`
--

CREATE TABLE `jarmuberelhetoseg` (
  `jarmuId` int(11) NOT NULL,
  `kezdet` datetime NOT NULL,
  `veg` datetime NOT NULL,
  `ismetlodes` enum('Nincs','Hetente','Kethetente','Havonta') NOT NULL DEFAULT 'Nincs',
  `oradij` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `jarmukep`
--

CREATE TABLE `jarmukep` (
  `jarmuId` int(11) NOT NULL,
  `eleresiUt` varchar(256) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `uzenet`
--

CREATE TABLE `uzenet` (
  `id` int(11) NOT NULL,
  `tartalom` varchar(512) NOT NULL,
  `kuldesiIdo` datetime NOT NULL,
  `panasz` tinyint(1) NOT NULL,
  `kuldoId` int(11) NOT NULL,
  `berlesId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `uzenetcsatolmany`
--

CREATE TABLE `uzenetcsatolmany` (
  `uzenetId` int(11) NOT NULL,
  `eleresiUt` varchar(256) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Indexek a kiírt táblákhoz
--

--
-- A tábla indexei `berles`
--
ALTER TABLE `berles`
  ADD PRIMARY KEY (`id`),
  ADD KEY `berlo` (`berloId`),
  ADD KEY `jarmu` (`jarmuId`);

--
-- A tábla indexei `ertesites`
--
ALTER TABLE `ertesites`
  ADD PRIMARY KEY (`id`),
  ADD KEY `felhasznalo` (`felhasznaloId`);

--
-- A tábla indexei `felhasznalo`
--
ALTER TABLE `felhasznalo`
  ADD PRIMARY KEY (`id`);

--
-- A tábla indexei `jarmu`
--
ALTER TABLE `jarmu`
  ADD PRIMARY KEY (`id`),
  ADD KEY `tulajdonos` (`tulajdonosId`);

--
-- A tábla indexei `jarmuberelhetoseg`
--
ALTER TABLE `jarmuberelhetoseg`
  ADD PRIMARY KEY (`jarmuId`,`kezdet`,`veg`);

--
-- A tábla indexei `jarmukep`
--
ALTER TABLE `jarmukep`
  ADD PRIMARY KEY (`jarmuId`,`eleresiUt`);

--
-- A tábla indexei `uzenet`
--
ALTER TABLE `uzenet`
  ADD PRIMARY KEY (`id`),
  ADD KEY `kuldo` (`kuldoId`),
  ADD KEY `berles` (`berlesId`);

--
-- A tábla indexei `uzenetcsatolmany`
--
ALTER TABLE `uzenetcsatolmany`
  ADD PRIMARY KEY (`uzenetId`,`eleresiUt`);

--
-- A kiírt táblák AUTO_INCREMENT értéke
--

--
-- AUTO_INCREMENT a táblához `berles`
--
ALTER TABLE `berles`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT a táblához `ertesites`
--
ALTER TABLE `ertesites`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `felhasznalo`
--
ALTER TABLE `felhasznalo`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT a táblához `jarmu`
--
ALTER TABLE `jarmu`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `uzenet`
--
ALTER TABLE `uzenet`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- Megkötések a kiírt táblákhoz
--

--
-- Megkötések a táblához `berles`
--
ALTER TABLE `berles`
  ADD CONSTRAINT `berles_ibfk_1` FOREIGN KEY (`jarmuId`) REFERENCES `jarmu` (`id`),
  ADD CONSTRAINT `berles_ibfk_2` FOREIGN KEY (`berloId`) REFERENCES `felhasznalo` (`id`);

--
-- Megkötések a táblához `ertesites`
--
ALTER TABLE `ertesites`
  ADD CONSTRAINT `ertesites_ibfk_1` FOREIGN KEY (`felhasznaloId`) REFERENCES `felhasznalo` (`id`);

--
-- Megkötések a táblához `jarmu`
--
ALTER TABLE `jarmu`
  ADD CONSTRAINT `jarmu_ibfk_1` FOREIGN KEY (`tulajdonosId`) REFERENCES `felhasznalo` (`id`);

--
-- Megkötések a táblához `jarmuberelhetoseg`
--
ALTER TABLE `jarmuberelhetoseg`
  ADD CONSTRAINT `jarmuberelhetoseg_ibfk_1` FOREIGN KEY (`jarmuId`) REFERENCES `jarmu` (`id`);

--
-- Megkötések a táblához `jarmukep`
--
ALTER TABLE `jarmukep`
  ADD CONSTRAINT `jarmukep_ibfk_1` FOREIGN KEY (`jarmuId`) REFERENCES `jarmu` (`id`);

--
-- Megkötések a táblához `uzenet`
--
ALTER TABLE `uzenet`
  ADD CONSTRAINT `uzenet_ibfk_1` FOREIGN KEY (`kuldoId`) REFERENCES `felhasznalo` (`id`),
  ADD CONSTRAINT `uzenet_ibfk_2` FOREIGN KEY (`berlesId`) REFERENCES `berles` (`id`);

--
-- Megkötések a táblához `uzenetcsatolmany`
--
ALTER TABLE `uzenetcsatolmany`
  ADD CONSTRAINT `uzenetcsatolmany_ibfk_1` FOREIGN KEY (`uzenetId`) REFERENCES `uzenet` (`id`);

DELIMITER $$
--
-- Események
--
CREATE DEFINER=`root`@`localhost` EVENT `regiErtesitesekTorlese` ON SCHEDULE EVERY 1 DAY STARTS '2026-01-03 12:57:20' ON COMPLETION NOT PRESERVE ENABLE DO CALL regiErtesitesekTorlese()$$

DELIMITER ;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
