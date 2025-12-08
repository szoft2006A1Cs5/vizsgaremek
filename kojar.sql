-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3307
-- Generation Time: Nov 02, 2025 at 10:08 PM
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
-- Database: `kojar`
--
CREATE DATABASE IF NOT EXISTS `kojar` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_hungarian_ci;
USE `kojar`;

-- --------------------------------------------------------

--
-- Table structure for table `bérlés`
--

CREATE TABLE `bérlés` (
  `bérlésId` int(11) NOT NULL,
  `teljesÁr` int(11) NOT NULL,
  `letét` int(11) NOT NULL,
  `kezdet` date NOT NULL,
  `vég` date NOT NULL,
  `állapot` enum('javaslat','elfogadva','folyamatban','lezárva') NOT NULL,
  `átvételiHely` point NOT NULL,
  `üzemanyagszint` float DEFAULT NULL,
  `bérlőÉrtékelés` double DEFAULT NULL,
  `bérbeadóÉrtékelés` double DEFAULT NULL,
  `bérlő` varchar(8) NOT NULL,
  `jármű` varchar(17) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `felhasználó`
--

CREATE TABLE `felhasználó` (
  `személyiSzám` varchar(8) NOT NULL,
  `név` varchar(64) NOT NULL,
  `telefonszám` varchar(11) NOT NULL,
  `születésiDátum` date NOT NULL,
  `profilKépElérésiÚt` varchar(256) NOT NULL,
  `email` varchar(64) NOT NULL,
  `jelszó` varchar(128) NOT NULL,
  `jogosítványSzám` varchar(10) NOT NULL,
  `jogosítványKiállításiDátum` date NOT NULL,
  `címIrányítószám` varchar(4) NOT NULL,
  `címTelepülés` varchar(64) NOT NULL,
  `címUtcaHázszám` varchar(64) NOT NULL,
  `bankkártyaSzám` varchar(16) NOT NULL,
  `bankkártyaLejáratiDátum` date NOT NULL,
  `egyenleg` int(11) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `jármű`
--

CREATE TABLE `jármű` (
  `alvázszám` varchar(17) NOT NULL,
  `tulajdonos` varchar(8) NOT NULL,
  `rendszám` varchar(7) NOT NULL,
  `márka` varchar(16) NOT NULL,
  `típus` varchar(32) NOT NULL,
  `évjárat` int(11) NOT NULL,
  `leírás` varchar(512) NOT NULL,
  `kmÁllás` int(11) NOT NULL,
  `átlagFogyasztás` double NOT NULL,
  `biztosításiSzám` varchar(64) NOT NULL,
  `óradíj` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `járműbérelhetőség`
--

CREATE TABLE `járműbérelhetőség` (
  `jármű` varchar(17) NOT NULL,
  `kezdet` int(11) NOT NULL,
  `vég` int(11) NOT NULL,
  `ismétlődés` enum('nincs','hetente','kéthetente','havonta') NOT NULL DEFAULT 'nincs'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `járműkép`
--

CREATE TABLE `járműkép` (
  `jármű` varchar(17) NOT NULL,
  `elérésiÚt` varchar(256) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `üzenet`
--

CREATE TABLE `üzenet` (
  `üzenetId` int(11) NOT NULL,
  `tartalom` varchar(512) NOT NULL,
  `küldésiIdő` datetime NOT NULL,
  `panasz` tinyint(1) NOT NULL,
  `küldő` varchar(8) NOT NULL,
  `bérlés` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `üzenetcsatolmány`
--

CREATE TABLE `üzenetcsatolmány` (
  `üzenet` int(11) NOT NULL,
  `elérésiÚt` varchar(256) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `bérlés`
--
ALTER TABLE `bérlés`
  ADD PRIMARY KEY (`bérlésId`),
  ADD KEY `bérlő` (`bérlő`),
  ADD KEY `jármű` (`jármű`);

--
-- Indexes for table `felhasználó`
--
ALTER TABLE `felhasználó`
  ADD PRIMARY KEY (`személyiSzám`);

--
-- Indexes for table `jármű`
--
ALTER TABLE `jármű`
  ADD PRIMARY KEY (`alvázszám`),
  ADD KEY `tulajdonos` (`tulajdonos`);

--
-- Indexes for table `járműbérelhetőség`
--
ALTER TABLE `járműbérelhetőség`
  ADD PRIMARY KEY (`jármű`,`kezdet`,`vég`),
  ADD KEY `jármű` (`jármű`);

--
-- Indexes for table `járműkép`
--
ALTER TABLE `járműkép`
  ADD PRIMARY KEY (`jármű`,`elérésiÚt`),
  ADD KEY `jármű` (`jármű`);

--
-- Indexes for table `üzenet`
--
ALTER TABLE `üzenet`
  ADD PRIMARY KEY (`üzenetId`),
  ADD KEY `küldő` (`küldő`),
  ADD KEY `bérlés` (`bérlés`);

--
-- Indexes for table `üzenetcsatolmány`
--
ALTER TABLE `üzenetcsatolmány`
  ADD PRIMARY KEY (`üzenet`,`elérésiÚt`),
  ADD KEY `üzenet` (`üzenet`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `bérlés`
--
ALTER TABLE `bérlés`
  MODIFY `bérlésId` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `üzenet`
--
ALTER TABLE `üzenet`
  MODIFY `üzenetId` int(11) NOT NULL AUTO_INCREMENT;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `bérlés`
--
ALTER TABLE `bérlés`
  ADD CONSTRAINT `bérlés_ibfk_1` FOREIGN KEY (`jármű`) REFERENCES `jármű` (`alvázszám`),
  ADD CONSTRAINT `bérlés_ibfk_2` FOREIGN KEY (`bérlő`) REFERENCES `felhasználó` (`személyiSzám`);

--
-- Constraints for table `jármű`
--
ALTER TABLE `jármű`
  ADD CONSTRAINT `jármű_ibfk_1` FOREIGN KEY (`tulajdonos`) REFERENCES `felhasználó` (`személyiSzám`);

--
-- Constraints for table `járműbérelhetőség`
--
ALTER TABLE `járműbérelhetőség`
  ADD CONSTRAINT `járműbérelhetőség_ibfk_1` FOREIGN KEY (`jármű`) REFERENCES `jármű` (`alvázszám`);

--
-- Constraints for table `járműkép`
--
ALTER TABLE `járműkép`
  ADD CONSTRAINT `járműkép_ibfk_1` FOREIGN KEY (`jármű`) REFERENCES `jármű` (`alvázszám`);

--
-- Constraints for table `üzenet`
--
ALTER TABLE `üzenet`
  ADD CONSTRAINT `üzenet_ibfk_1` FOREIGN KEY (`küldő`) REFERENCES `felhasználó` (`személyiSzám`),
  ADD CONSTRAINT `üzenet_ibfk_2` FOREIGN KEY (`bérlés`) REFERENCES `bérlés` (`bérlésId`);

--
-- Constraints for table `üzenetcsatolmány`
--
ALTER TABLE `üzenetcsatolmány`
  ADD CONSTRAINT `üzenetcsatolmány_ibfk_1` FOREIGN KEY (`üzenet`) REFERENCES `üzenet` (`üzenetId`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
