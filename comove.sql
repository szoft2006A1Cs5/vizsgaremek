-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3307
-- Generation Time: Jan 03, 2026 at 01:16 PM
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
  `berlesId` int(11) NOT NULL,
  `teljesAr` int(11) NOT NULL,
  `letet` int(11) NOT NULL,
  `kezdet` datetime NOT NULL,
  `veg` datetime NOT NULL,
  `allapot` enum('javaslat','elfogadva','folyamatban','lezarva') NOT NULL,
  `atveteliHely` point NOT NULL,
  `uzemanyagszint` float DEFAULT NULL,
  `berloErtekeles` double DEFAULT NULL,
  `berbeadoErtekeles` double DEFAULT NULL,
  `berlo` int(11) NOT NULL,
  `jarmu` varchar(17) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `ertesites`
--

CREATE TABLE `ertesites` (
  `ertesitesId` int(11) NOT NULL,
  `felhasznalo` int(11) NOT NULL,
  `szoveg` varchar(512) NOT NULL,
  `kuldesIdeje` datetime NOT NULL,
  `olvasva` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `felhasznalo`
--

CREATE TABLE `felhasznalo` (
  `felhasznaloId` int(11) NOT NULL,
  `szemelyiSzam` varchar(8) NOT NULL,
  `nev` varchar(64) NOT NULL,
  `telefonszam` varchar(11) NOT NULL,
  `szuletesiDatum` date NOT NULL,
  `profilKepEleresiUt` varchar(256) DEFAULT NULL,
  `email` varchar(64) NOT NULL,
  `jelszo` varchar(128) NOT NULL,
  `jogosultsag` enum('felhasznalo','admin') NOT NULL DEFAULT 'felhasznalo',
  `jogositvanySzam` varchar(10) NOT NULL,
  `jogositvanyKiallitasDatum` date NOT NULL,
  `cimIranyitoszam` varchar(4) NOT NULL,
  `cimTelepules` varchar(64) NOT NULL,
  `cimUtcaHazszam` varchar(64) NOT NULL,
  `egyenleg` int(11) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `jarmu`
--

CREATE TABLE `jarmu` (
  `alvazszam` varchar(17) NOT NULL,
  `tulajdonos` int(11) NOT NULL,
  `rendszam` varchar(7) NOT NULL,
  `marka` varchar(16) NOT NULL,
  `tipus` varchar(32) NOT NULL,
  `evjarat` int(11) NOT NULL,
  `leiras` varchar(512) NOT NULL,
  `kmAllas` int(11) NOT NULL,
  `atlagFogyasztas` double NOT NULL,
  `biztositasiSzam` varchar(64) NOT NULL,
  `oradij` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `jarmuberelhetoseg`
--

CREATE TABLE `jarmuberelhetoseg` (
  `jarmu` varchar(17) NOT NULL,
  `kezdet` datetime NOT NULL,
  `veg` datetime NOT NULL,
  `ismetlodes` enum('nincs','hetente','kethetente','havonta') NOT NULL DEFAULT 'nincs'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `jarmukep`
--

CREATE TABLE `jarmukep` (
  `jarmu` varchar(17) NOT NULL,
  `eleresiUt` varchar(256) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `uzenet`
--

CREATE TABLE `uzenet` (
  `uzenetId` int(11) NOT NULL,
  `tartalom` varchar(512) NOT NULL,
  `kuldesiIdo` datetime NOT NULL,
  `panasz` tinyint(1) NOT NULL,
  `kuldo` int(11) NOT NULL,
  `berles` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `uzenetcsatolmany`
--

CREATE TABLE `uzenetcsatolmany` (
  `uzenet` int(11) NOT NULL,
  `eleresiUt` varchar(256) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `berles`
--
ALTER TABLE `berles`
  ADD PRIMARY KEY (`berlesId`),
  ADD KEY `berlo` (`berlo`),
  ADD KEY `jarmu` (`jarmu`);

--
-- Indexes for table `ertesites`
--
ALTER TABLE `ertesites`
  ADD PRIMARY KEY (`ertesitesId`),
  ADD KEY `felhasznalo` (`felhasznalo`);

--
-- Indexes for table `felhasznalo`
--
ALTER TABLE `felhasznalo`
  ADD PRIMARY KEY (`felhasznaloId`);

--
-- Indexes for table `jarmu`
--
ALTER TABLE `jarmu`
  ADD PRIMARY KEY (`alvazszam`),
  ADD KEY `tulajdonos` (`tulajdonos`);

--
-- Indexes for table `jarmuberelhetoseg`
--
ALTER TABLE `jarmuberelhetoseg`
  ADD PRIMARY KEY (`jarmu`,`kezdet`,`veg`);

--
-- Indexes for table `jarmukep`
--
ALTER TABLE `jarmukep`
  ADD PRIMARY KEY (`jarmu`,`eleresiUt`);

--
-- Indexes for table `uzenet`
--
ALTER TABLE `uzenet`
  ADD PRIMARY KEY (`uzenetId`),
  ADD KEY `kuldo` (`kuldo`),
  ADD KEY `berles` (`berles`);

--
-- Indexes for table `uzenetcsatolmany`
--
ALTER TABLE `uzenetcsatolmany`
  ADD PRIMARY KEY (`uzenet`,`eleresiUt`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `berles`
--
ALTER TABLE `berles`
  MODIFY `berlesId` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `ertesites`
--
ALTER TABLE `ertesites`
  MODIFY `ertesitesId` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `felhasznalo`
--
ALTER TABLE `felhasznalo`
  MODIFY `felhasznaloId` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `uzenet`
--
ALTER TABLE `uzenet`
  MODIFY `uzenetId` int(11) NOT NULL AUTO_INCREMENT;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `berles`
--
ALTER TABLE `berles`
  ADD CONSTRAINT `berles_ibfk_1` FOREIGN KEY (`jarmu`) REFERENCES `jarmu` (`alvazszam`),
  ADD CONSTRAINT `berles_ibfk_2` FOREIGN KEY (`berlo`) REFERENCES `felhasznalo` (`felhasznaloId`);

--
-- Constraints for table `ertesites`
--
ALTER TABLE `ertesites`
  ADD CONSTRAINT `ertesites_ibfk_1` FOREIGN KEY (`felhasznalo`) REFERENCES `felhasznalo` (`felhasznaloId`);

--
-- Constraints for table `jarmu`
--
ALTER TABLE `jarmu`
  ADD CONSTRAINT `jarmu_ibfk_1` FOREIGN KEY (`tulajdonos`) REFERENCES `felhasznalo` (`felhasznaloId`);

--
-- Constraints for table `jarmuberelhetoseg`
--
ALTER TABLE `jarmuberelhetoseg`
  ADD CONSTRAINT `jarmuberelhetoseg_ibfk_1` FOREIGN KEY (`jarmu`) REFERENCES `jarmu` (`alvazszam`);

--
-- Constraints for table `jarmukep`
--
ALTER TABLE `jarmukep`
  ADD CONSTRAINT `jarmukep_ibfk_1` FOREIGN KEY (`jarmu`) REFERENCES `jarmu` (`alvazszam`);

--
-- Constraints for table `uzenet`
--
ALTER TABLE `uzenet`
  ADD CONSTRAINT `uzenet_ibfk_1` FOREIGN KEY (`kuldo`) REFERENCES `felhasznalo` (`felhasznaloId`),
  ADD CONSTRAINT `uzenet_ibfk_2` FOREIGN KEY (`berles`) REFERENCES `berles` (`berlesId`);

--
-- Constraints for table `uzenetcsatolmany`
--
ALTER TABLE `uzenetcsatolmany`
  ADD CONSTRAINT `uzenetcsatolmany_ibfk_1` FOREIGN KEY (`uzenet`) REFERENCES `uzenet` (`uzenetId`);

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
