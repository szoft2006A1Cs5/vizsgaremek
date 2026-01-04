-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3307
-- Generation Time: Jan 04, 2026 at 02:43 PM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `comove`
--
CREATE DATABASE IF NOT EXISTS `comove` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_hungarian_ci;
USE `comove`;

DELIMITER $$
--
-- Procedures
--
CREATE DEFINER=`root`@`localhost` PROCEDURE `regiErtesitesekTorlese` (IN `param` INT)   BEGIN
	DELETE FROM ertesites
    WHERE ertesites.olvasva = 1
    AND ertesites.kuldesIdeje < NOW() - INTERVAL 30 DAY;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `berles`
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
  `jarmuId` varchar(17) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `berles`
--

INSERT INTO `berles` (`id`, `teljesAr`, `letet`, `kezdet`, `veg`, `allapot`, `atveteliHelySzelesseg`, `atveteliHelyHosszusag`, `uzemanyagszint`, `berloErtekeles`, `berbeadoErtekeles`, `berloId`, `jarmuId`) VALUES
(1, 10500, 50000, '2026-01-10 08:00:00', '2026-01-10 11:00:00', 'BerloJavaslat', 0, 0, NULL, NULL, NULL, 3, 'WBA12345678901234'),
(2, 14000, 50000, '2026-01-12 10:00:00', '2026-01-12 14:00:00', 'BerbeadoJavaslat', 0, 0, NULL, NULL, NULL, 3, 'WBA12345678901234'),
(3, 7000, 50000, '2026-01-03 13:00:00', '2026-01-03 15:00:00', 'BerloAtvetelElfogadva', 0, 0, 90, NULL, NULL, 3, 'WBA12345678901234'),
(4, 21000, 50000, '2026-01-03 09:00:00', '2026-01-03 15:00:00', 'Aktiv', 0, 0, 75, NULL, NULL, 3, 'WBA12345678901234');

-- --------------------------------------------------------

--
-- Table structure for table `ertesites`
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
-- Table structure for table `felhasznalo`
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

--
-- Dumping data for table `felhasznalo`
--

INSERT INTO `felhasznalo` (`id`, `szemelyiSzam`, `nev`, `telefonszam`, `szuletesiDatum`, `profilKepEleresiUt`, `email`, `jelszo`, `jogosultsag`, `jogositvanySzam`, `jogositvanyKiallitasDatum`, `cimIranyitoszam`, `cimTelepules`, `cimUtcaHazszam`, `egyenleg`) VALUES
(1, '112233AA', 'Adminisztrátor', '06301111111', '1980-01-01', NULL, 'admin@comove.hu', 'hashed_pwd', '', 'AA000000', '2000-01-01', '1011', 'Budapest', 'Vár utca 1.', 0),
(2, '223344BB', 'Tulajdonos Tamás', '06202222222', '1990-05-15', NULL, 'tamas@tulaj.hu', 'hashed_pwd', 'Felhasznalo', 'BB111111', '2010-05-15', '4032', 'Debrecen', 'Egyetem tér 1.', 120000),
(3, '334455CC', 'Bérlő Béla', '06703333333', '1995-08-20', NULL, 'bela@berlo.hu', 'hashed_pwd', 'Felhasznalo', 'CC222222', '2015-08-20', '6720', 'Szeged', 'Dóm tér 2.', 5000);

-- --------------------------------------------------------

--
-- Table structure for table `jarmu`
--

CREATE TABLE `jarmu` (
  `alvazszam` varchar(17) NOT NULL,
  `tulajdonosId` int(11) NOT NULL,
  `rendszam` varchar(7) NOT NULL,
  `marka` varchar(16) NOT NULL,
  `tipus` varchar(32) NOT NULL,
  `evjarat` int(11) NOT NULL,
  `leiras` varchar(512) NOT NULL,
  `kmAllas` int(11) NOT NULL,
  `atlagFogyasztas` double NOT NULL,
  `biztositasiSzam` varchar(64) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `jarmu`
--

INSERT INTO `jarmu` (`alvazszam`, `tulajdonosId`, `rendszam`, `marka`, `tipus`, `evjarat`, `leiras`, `kmAllas`, `atlagFogyasztas`, `biztositasiSzam`) VALUES
('WBA12345678901234', 2, 'AA-BB-1', 'BMW', 'i3', 2019, 'Elektromos városi kisautó, hatótáv 250km.', 85000, 0, 'KGFB-998877');

-- --------------------------------------------------------

--
-- Table structure for table `jarmuberelhetoseg`
--

CREATE TABLE `jarmuberelhetoseg` (
  `jarmuId` varchar(17) NOT NULL,
  `kezdet` datetime NOT NULL,
  `veg` datetime NOT NULL,
  `ismetlodes` enum('Nincs','Hetente','Kethetente','Havonta') NOT NULL DEFAULT 'Nincs',
  `oradij` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `jarmukep`
--

CREATE TABLE `jarmukep` (
  `jarmuId` varchar(17) NOT NULL,
  `eleresiUt` varchar(256) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `jarmukep`
--

INSERT INTO `jarmukep` (`jarmuId`, `eleresiUt`) VALUES
('WBA12345678901234', 'uploads/cars/bmw_i3_front.jpg'),
('WBA12345678901234', 'uploads/cars/bmw_i3_interior.jpg');

-- --------------------------------------------------------

--
-- Table structure for table `uzenet`
--

CREATE TABLE `uzenet` (
  `id` int(11) NOT NULL,
  `tartalom` varchar(512) NOT NULL,
  `kuldesiIdo` datetime NOT NULL,
  `panasz` tinyint(1) NOT NULL,
  `kuldoId` int(11) NOT NULL,
  `berlesId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Dumping data for table `uzenet`
--

INSERT INTO `uzenet` (`id`, `tartalom`, `kuldesiIdo`, `panasz`, `kuldoId`, `berlesId`) VALUES
(1, 'Szia! Az időpont jó, de a helyszín legyen inkább a Duna Pláza parkolója.', '2026-01-03 13:10:00', 0, 2, 2);

-- --------------------------------------------------------

--
-- Table structure for table `uzenetcsatolmany`
--

CREATE TABLE `uzenetcsatolmany` (
  `uzenetId` int(11) NOT NULL,
  `eleresiUt` varchar(256) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `berles`
--
ALTER TABLE `berles`
  ADD PRIMARY KEY (`id`),
  ADD KEY `berlo` (`berloId`),
  ADD KEY `jarmu` (`jarmuId`);

--
-- Indexes for table `ertesites`
--
ALTER TABLE `ertesites`
  ADD PRIMARY KEY (`id`),
  ADD KEY `felhasznalo` (`felhasznaloId`);

--
-- Indexes for table `felhasznalo`
--
ALTER TABLE `felhasznalo`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `jarmu`
--
ALTER TABLE `jarmu`
  ADD PRIMARY KEY (`alvazszam`),
  ADD KEY `tulajdonos` (`tulajdonosId`);

--
-- Indexes for table `jarmuberelhetoseg`
--
ALTER TABLE `jarmuberelhetoseg`
  ADD PRIMARY KEY (`jarmuId`,`kezdet`,`veg`);

--
-- Indexes for table `jarmukep`
--
ALTER TABLE `jarmukep`
  ADD PRIMARY KEY (`jarmuId`,`eleresiUt`),
  ADD UNIQUE KEY `jarmuId` (`jarmuId`,`eleresiUt`);

--
-- Indexes for table `uzenet`
--
ALTER TABLE `uzenet`
  ADD PRIMARY KEY (`id`),
  ADD KEY `kuldo` (`kuldoId`),
  ADD KEY `berles` (`berlesId`);

--
-- Indexes for table `uzenetcsatolmany`
--
ALTER TABLE `uzenetcsatolmany`
  ADD PRIMARY KEY (`uzenetId`,`eleresiUt`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `berles`
--
ALTER TABLE `berles`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `ertesites`
--
ALTER TABLE `ertesites`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `felhasznalo`
--
ALTER TABLE `felhasznalo`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `uzenet`
--
ALTER TABLE `uzenet`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `berles`
--
ALTER TABLE `berles`
  ADD CONSTRAINT `berles_ibfk_1` FOREIGN KEY (`jarmuId`) REFERENCES `jarmu` (`alvazszam`),
  ADD CONSTRAINT `berles_ibfk_2` FOREIGN KEY (`berloId`) REFERENCES `felhasznalo` (`id`);

--
-- Constraints for table `ertesites`
--
ALTER TABLE `ertesites`
  ADD CONSTRAINT `ertesites_ibfk_1` FOREIGN KEY (`felhasznaloId`) REFERENCES `felhasznalo` (`id`);

--
-- Constraints for table `jarmu`
--
ALTER TABLE `jarmu`
  ADD CONSTRAINT `jarmu_ibfk_1` FOREIGN KEY (`tulajdonosId`) REFERENCES `felhasznalo` (`id`);

--
-- Constraints for table `jarmuberelhetoseg`
--
ALTER TABLE `jarmuberelhetoseg`
  ADD CONSTRAINT `jarmuberelhetoseg_ibfk_1` FOREIGN KEY (`jarmuId`) REFERENCES `jarmu` (`alvazszam`);

--
-- Constraints for table `jarmukep`
--
ALTER TABLE `jarmukep`
  ADD CONSTRAINT `jarmukep_ibfk_1` FOREIGN KEY (`jarmuId`) REFERENCES `jarmu` (`alvazszam`);

--
-- Constraints for table `uzenet`
--
ALTER TABLE `uzenet`
  ADD CONSTRAINT `uzenet_ibfk_1` FOREIGN KEY (`kuldoId`) REFERENCES `felhasznalo` (`id`),
  ADD CONSTRAINT `uzenet_ibfk_2` FOREIGN KEY (`berlesId`) REFERENCES `berles` (`id`);

--
-- Constraints for table `uzenetcsatolmany`
--
ALTER TABLE `uzenetcsatolmany`
  ADD CONSTRAINT `uzenetcsatolmany_ibfk_1` FOREIGN KEY (`uzenetId`) REFERENCES `uzenet` (`id`);

DELIMITER $$
--
-- Events
--
CREATE DEFINER=`root`@`localhost` EVENT `regiErtesitesekTorlese` ON SCHEDULE EVERY 1 DAY STARTS '2026-01-03 12:57:20' ON COMPLETION NOT PRESERVE ENABLE DO CALL regiErtesitesekTorlese()$$

DELIMITER ;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
