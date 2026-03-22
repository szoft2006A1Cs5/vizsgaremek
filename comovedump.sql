-- MySQL dump 10.13  Distrib 8.0.33, for macos13 (arm64)
--
-- Host: 127.0.0.1    Database: comove
-- ------------------------------------------------------
-- Server version	8.0.33

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

DROP DATABASE IF EXISTS `comove`;
CREATE DATABASE `comove` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_hungarian_ci;
USE `comove`;

--
-- Table structure for table `messages`
--

DROP TABLE IF EXISTS `messages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `messages` (
  `id` int NOT NULL AUTO_INCREMENT,
  `content` varchar(512) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `isImage` tinyint(1) NOT NULL DEFAULT '0',
  `timeSent` datetime NOT NULL,
  `isComplaint` tinyint(1) NOT NULL,
  `senderId` int NOT NULL,
  `rentalId` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `sender` (`senderId`),
  KEY `rental` (`rentalId`),
  CONSTRAINT `messages_ibfk_1` FOREIGN KEY (`senderId`) REFERENCES `users` (`id`),
  CONSTRAINT `messages_ibfk_2` FOREIGN KEY (`rentalId`) REFERENCES `rentals` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `messages`
--

LOCK TABLES `messages` WRITE;
/*!40000 ALTER TABLE `messages` DISABLE KEYS */;
INSERT INTO `messages` VALUES (1,'Szia! Meg√©rkeztem az aut√≥hoz, minden rendben t≈±nik.',0,'2026-01-11 08:55:00',0,4,2),(2,'Szuper, a kulcs a kijel√∂lt helyen volt?',0,'2026-01-11 08:57:00',0,2,2),(3,'Hey! How are you?',0,'2026-03-16 22:40:17',1,1,4);
/*!40000 ALTER TABLE `messages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `notifications`
--

DROP TABLE IF EXISTS `notifications`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `notifications` (
  `id` int NOT NULL AUTO_INCREMENT,
  `userId` int NOT NULL,
  `notificationId` int NOT NULL,
  `content` varchar(512) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `timeSent` datetime NOT NULL,
  `read` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `user` (`userId`),
  CONSTRAINT `notifications_ibfk_1` FOREIGN KEY (`userId`) REFERENCES `users` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `notifications`
--

LOCK TABLES `notifications` WRITE;
/*!40000 ALTER TABLE `notifications` DISABLE KEYS */;
/*!40000 ALTER TABLE `notifications` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `rentals`
--

DROP TABLE IF EXISTS `rentals`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rentals` (
  `id` int NOT NULL AUTO_INCREMENT,
  `rentalPrice` int NOT NULL,
  `start` datetime NOT NULL,
  `end` datetime NOT NULL,
  `status` int NOT NULL DEFAULT '0',
  `pickupLocation` varchar(512) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `fuelLevel` float DEFAULT NULL,
  `renterRating` double DEFAULT NULL,
  `ownerRating` double DEFAULT NULL,
  `renterId` int NOT NULL,
  `vehicleId` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `renter` (`renterId`),
  KEY `vehicle` (`vehicleId`),
  CONSTRAINT `rentals_ibfk_1` FOREIGN KEY (`vehicleId`) REFERENCES `vehicles` (`id`),
  CONSTRAINT `rentals_ibfk_2` FOREIGN KEY (`renterId`) REFERENCES `users` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rentals`
--

LOCK TABLES `rentals` WRITE;
/*!40000 ALTER TABLE `rentals` DISABLE KEYS */;
INSERT INTO `rentals` VALUES (1,15000,'2026-01-05 10:00:00','2026-01-05 18:00:00',8,'9700 Szombathely, Zr√≠nyi Ilona utca 12.',100,5,4.5,3,1),(2,25000,'2026-01-11 09:00:00','2026-01-13 17:00:00',5,'9700 Szombathely, K√©thly Anna utca 12.',75.5,NULL,NULL,4,2),(3,8000,'2026-01-15 08:00:00','2026-01-15 12:00:00',1,'9700 Szombathely, Ur√°nia udvar 1.',NULL,NULL,NULL,1,3),(4,49100,'2026-03-13 10:00:00','2026-03-15 17:00:00',0,'string',0,NULL,NULL,2,1);
/*!40000 ALTER TABLE `rentals` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `id` int NOT NULL AUTO_INCREMENT,
  `idCardNumber` varchar(8) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `name` varchar(64) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `phone` varchar(11) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `dateOfBirth` date NOT NULL,
  `profilePicPath` varchar(256) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `email` varchar(64) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `password` blob NOT NULL,
  `salt` blob NOT NULL,
  `role` enum('User','Administrator') COLLATE utf8mb4_hungarian_ci NOT NULL DEFAULT 'User',
  `driversLicenseNumber` varchar(10) COLLATE utf8mb4_hungarian_ci DEFAULT NULL,
  `addressZipcode` varchar(4) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `addressSettlement` varchar(64) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `addressStreetHouse` varchar(64) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `balance` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'123456AA','Teszt Elek','36201234567','2004-04-18',NULL,'tesztelek@teszt.hu',_binary '\\◊ëÄ<)^\‰Vjá•î#Á¥∞ E \ı.xΩ\‹\€˚6\⁄\ÔC∞2\Á°\"2>XI4O\Ò˝bZxÖ\√ˇbhäA∑\‰é\”\Ÿ\‡',_binary 'îR\0¯LÔÉççD\‡˙S','User','AA123456','9700','Szombathely','Zr√≠nyi Ilona utca 12.',0),(2,'123456BB','Gipsz Jakab','36701234567','1995-07-21',NULL,'gipszjakab@teszt.hu',_binary '–é\«»ì¯\‰	5£üáæ;ê\ƒ=c\ıcˇÖdFù\›Fç\–\ÓsàPª©zO⁄Æ[cH⁄Ω¬¥À´\Ô7≠ø\Ÿ',_binary 'àm\„b\‘(´}π$æ‘¨','User','BB123456','1117','Budapest','Budafoki √∫t 12.',48445),(3,'123456CC','Vincs Eszter','36301234567','2000-11-02',NULL,'vincseszter@teszt.hu',_binary '\ÈiìâHbô›§ã⁄òNR\ ;ç°l\È^æ®0\ yÄígø\◊/¬Ñl∂\0œåAW«Ø@Y\÷\ \"í\…GaNæqlñîì',_binary '±¿\‰AU+\Ô&\ı±\–u\‚\»q','User','CC123456','9700','Szombathely','K√©thly Anna utca 7.',0),(4,'123456DD','Teszt Ter√©z','36707654321','1989-12-12',NULL,'tesztterez@teszt.hu',_binary '±˙\Âî}’∞9^ƒ§\Ÿ<r5á\ÁS\€@\·\0\¬~ñ5\Ò\›YQtã¡éˇÆB¸\ı˙˙{0\”sê\¸®ìa:’ÄBQk',_binary '…Ér\Òïë¬ù4$+\›d\Ê','User','DD123456','1095','Budapest','Tin√≥di utca 1.',0),(5,'623412AD','Admin Tam√°s','36704124536','1978-09-07',NULL,'admin@teszt.hu',_binary 'µo+Ä\Û;\Õ<ˇ*Ju®¥(\r≤HKs¢R \ı~q,\"±\Õ\√UCx\Ì\Êà\Ÿ˙«∫\˜ u¶1s\ˆß\Ùá~\Zj0',_binary '_Eé^çNVë9\\\ŒVÆà','Administrator',NULL,'9700','Szombathely','Zr√≠nyi Ilona utca 12.',0);
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `vehicleavailabilities`
--

DROP TABLE IF EXISTS `vehicleavailabilities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `vehicleavailabilities` (
  `id` int NOT NULL AUTO_INCREMENT,
  `vehicleId` int NOT NULL,
  `availabilityId` int NOT NULL,
  `start` datetime NOT NULL,
  `end` datetime NOT NULL,
  `hourlyRate` int NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `vehicleavailability` (`vehicleId`,`availabilityId`),
  CONSTRAINT `vehicleavailabilities_ibfk_1` FOREIGN KEY (`vehicleId`) REFERENCES `vehicles` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vehicleavailabilities`
--

LOCK TABLES `vehicleavailabilities` WRITE;
/*!40000 ALTER TABLE `vehicleavailabilities` DISABLE KEYS */;
INSERT INTO `vehicleavailabilities` VALUES (4,2,1,'2026-03-11 11:00:00','2026-03-14 17:00:00',700),(5,1,1,'2026-03-11 11:00:00','2026-03-13 12:00:00',700),(6,1,2,'2026-03-13 12:00:00','2026-03-15 18:00:00',900),(7,1,3,'2026-03-23 10:00:00','2026-03-27 11:00:00',800),(8,1,4,'2026-03-27 11:00:00','2026-03-29 15:00:00',900),(9,4,1,'2026-03-23 11:00:00','2026-03-24 17:00:00',600);
/*!40000 ALTER TABLE `vehicleavailabilities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `vehicleimages`
--

DROP TABLE IF EXISTS `vehicleimages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `vehicleimages` (
  `id` int NOT NULL AUTO_INCREMENT,
  `vehicleId` int NOT NULL,
  `imageId` int NOT NULL,
  `sortIndex` int NOT NULL,
  `path` varchar(2048) COLLATE utf8mb4_hungarian_ci NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `vehicleimage` (`vehicleId`,`imageId`),
  CONSTRAINT `vehicleimages_ibfk_1` FOREIGN KEY (`vehicleId`) REFERENCES `vehicles` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vehicleimages`
--

LOCK TABLES `vehicleimages` WRITE;
/*!40000 ALTER TABLE `vehicleimages` DISABLE KEYS */;
INSERT INTO `vehicleimages` VALUES (1,1,1,1,'res/58274674-91a9-4afb-ab80-52a0a605c520.jpg'),(2,2,1,1,'res/64725ad2-8502-4eef-b4f3-07d3c5af9423.jpg'),(3,3,1,1,'res/43b1c8fd-ce7c-4c34-8dfc-f46d2e12b6ba.png'),(4,1,2,2,'res/3bd9f544-f633-4a6f-8a29-0463842e19d2.jpg'),(6,4,1,1,'res/c27ef141-b934-4520-94a3-7a6fd3bd7301.jpg');
/*!40000 ALTER TABLE `vehicleimages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `vehicles`
--

DROP TABLE IF EXISTS `vehicles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `vehicles` (
  `id` int NOT NULL AUTO_INCREMENT,
  `ownerId` int NOT NULL,
  `vin` varchar(17) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `licensePlate` varchar(7) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `manufacturer` varchar(16) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `model` varchar(32) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `year` int NOT NULL,
  `description` varchar(512) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `odometerReading` int NOT NULL,
  `horsepower` int NOT NULL,
  `avgFuelConsumption` double NOT NULL,
  `fuelType` varchar(20) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `insuranceNumber` varchar(64) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `transmission` varchar(16) COLLATE utf8mb4_hungarian_ci NOT NULL,
  PRIMARY KEY (`id`),
  KEY `owner` (`ownerId`),
  CONSTRAINT `vehicles_ibfk_1` FOREIGN KEY (`ownerId`) REFERENCES `users` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vehicles`
--

LOCK TABLES `vehicles` WRITE;
/*!40000 ALTER TABLE `vehicles` DISABLE KEYS */;
INSERT INTO `vehicles` VALUES (1,1,'VF312345678901234','ABC123','Toyota','Corolla',2018,'Megb√≠zhat√≥ hibrid v√°rosi cirk√°l√≥.',85000,132,4.5,'benzin-elektromos','KGFB-998877','automatikus'),(2,2,'WBA41234567890123','SKY789','BMW','320d',2015,'K√©nyelmes utaz√≥aut√≥ hosszabb t√°vra.',210000,180,6.2,'d√≠zel','KGFB-112233','manu√°lis'),(3,3,'TMB51234567890123','RNL456','Skoda','Octavia',2020,'Hatalmas csomagtart√≥, tiszta bels≈ë.',45000,150,5.5,'benzin','KGFB-445566','manu√°lis'),(4,1,'DEFGHIJK12341241A','ABCD123','Suzuki','Swift',2018,'',5,120,6.2,'Benzin','KGFB-123412515','Manu√°lis');
/*!40000 ALTER TABLE `vehicles` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-03-22 21:55:35
