-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3307
-- Generation Time: Jan 12, 2026 at 12:41 AM
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

-- --------------------------------------------------------

--
-- Table structure for table `messageattachments`
--

CREATE TABLE `messageattachments` (
  `messageId` int(11) NOT NULL,
  `attachmentPath` varchar(256) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `messages`
--

CREATE TABLE `messages` (
  `id` int(11) NOT NULL,
  `content` varchar(512) NOT NULL,
  `timeSent` datetime NOT NULL,
  `isComplaint` tinyint(1) NOT NULL,
  `senderId` int(11) NOT NULL,
  `rentalId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `notifications`
--

CREATE TABLE `notifications` (
  `id` int(11) NOT NULL,
  `userId` int(11) NOT NULL,
  `content` varchar(512) NOT NULL,
  `timeSent` datetime NOT NULL,
  `read` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `rentals`
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

-- --------------------------------------------------------

--
-- Table structure for table `users`
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

-- --------------------------------------------------------

--
-- Table structure for table `vehicleavailabilities`
--

CREATE TABLE `vehicleavailabilities` (
  `id` int(11) NOT NULL,
  `vehicleId` int(11) NOT NULL,
  `start` datetime NOT NULL,
  `end` datetime NOT NULL,
  `recurrence` enum('None','Weekly','Biweekly','Monthly') NOT NULL DEFAULT 'None',
  `hourlyRate` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `vehicleimages`
--

CREATE TABLE `vehicleimages` (
  `id` int(11) NOT NULL,
  `vehicleId` int(11) NOT NULL,
  `imagePath` varchar(256) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;

-- --------------------------------------------------------

--
-- Table structure for table `vehicles`
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
-- Indexes for dumped tables
--

--
-- Indexes for table `messageattachments`
--
ALTER TABLE `messageattachments`
  ADD PRIMARY KEY (`messageId`,`attachmentPath`);

--
-- Indexes for table `messages`
--
ALTER TABLE `messages`
  ADD PRIMARY KEY (`id`),
  ADD KEY `sender` (`senderId`),
  ADD KEY `rental` (`rentalId`);

--
-- Indexes for table `notifications`
--
ALTER TABLE `notifications`
  ADD PRIMARY KEY (`id`),
  ADD KEY `user` (`userId`);

--
-- Indexes for table `rentals`
--
ALTER TABLE `rentals`
  ADD PRIMARY KEY (`id`),
  ADD KEY `renter` (`renterId`),
  ADD KEY `vehicle` (`vehicleId`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `vehicleavailabilities`
--
ALTER TABLE `vehicleavailabilities`
  ADD PRIMARY KEY (`vehicleId`,`id`);

--
-- Indexes for table `vehicleimages`
--
ALTER TABLE `vehicleimages`
  ADD PRIMARY KEY (`vehicleId`,`id`,`imagePath`);

--
-- Indexes for table `vehicles`
--
ALTER TABLE `vehicles`
  ADD PRIMARY KEY (`id`),
  ADD KEY `owner` (`ownerId`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `messages`
--
ALTER TABLE `messages`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `notifications`
--
ALTER TABLE `notifications`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `rentals`
--
ALTER TABLE `rentals`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `vehicles`
--
ALTER TABLE `vehicles`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `messageattachments`
--
ALTER TABLE `messageattachments`
  ADD CONSTRAINT `messageattachments_ibfk_1` FOREIGN KEY (`messageId`) REFERENCES `messages` (`id`);

--
-- Constraints for table `messages`
--
ALTER TABLE `messages`
  ADD CONSTRAINT `messages_ibfk_1` FOREIGN KEY (`senderId`) REFERENCES `users` (`id`),
  ADD CONSTRAINT `messages_ibfk_2` FOREIGN KEY (`rentalId`) REFERENCES `rentals` (`id`);

--
-- Constraints for table `notifications`
--
ALTER TABLE `notifications`
  ADD CONSTRAINT `notifications_ibfk_1` FOREIGN KEY (`userId`) REFERENCES `users` (`id`);

--
-- Constraints for table `rentals`
--
ALTER TABLE `rentals`
  ADD CONSTRAINT `rentals_ibfk_1` FOREIGN KEY (`vehicleId`) REFERENCES `vehicles` (`id`),
  ADD CONSTRAINT `rentals_ibfk_2` FOREIGN KEY (`renterId`) REFERENCES `users` (`id`);

--
-- Constraints for table `vehicleavailabilities`
--
ALTER TABLE `vehicleavailabilities`
  ADD CONSTRAINT `vehicleavailabilities_ibfk_1` FOREIGN KEY (`vehicleId`) REFERENCES `vehicles` (`id`);

--
-- Constraints for table `vehicleimages`
--
ALTER TABLE `vehicleimages`
  ADD CONSTRAINT `vehicleimages_ibfk_1` FOREIGN KEY (`vehicleId`) REFERENCES `vehicles` (`id`);

--
-- Constraints for table `vehicles`
--
ALTER TABLE `vehicles`
  ADD CONSTRAINT `vehicles_ibfk_1` FOREIGN KEY (`ownerId`) REFERENCES `users` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
