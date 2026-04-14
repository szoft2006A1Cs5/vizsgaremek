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
CREATE DATABASE `comove`
DEFAULT CHARACTER SET utf8
COLLATE utf8_hungarian_ci;

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
  CONSTRAINT `messages_ibfk_2` FOREIGN KEY (`rentalId`) REFERENCES `rentals` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `messages`
--

LOCK TABLES `messages` WRITE;
/*!40000 ALTER TABLE `messages` DISABLE KEYS */;
INSERT INTO `messages` VALUES (1,'Szia! Megérkeztem az autóhoz, minden rendben tűnik.',0,'2026-01-11 08:55:00',0,4,2),(2,'Szuper, a kulcs a kijelölt helyen volt?',0,'2026-01-11 08:57:00',0,2,2),(5,'Szia mizujs',0,'2026-03-22 22:55:30',0,2,5),(6,'Cs, nem sok',0,'2026-03-23 00:13:31',0,1,5),(7,'Sup bro',0,'2026-03-23 07:40:44',0,1,5),(8,'Szia! Fogadd mar ael',0,'2026-03-23 07:49:47',0,2,7),(9,'Elfogadtam, kosz a penzt',0,'2026-03-23 07:51:54',0,1,7),(10,'Szivesen',0,'2026-03-23 07:52:07',0,2,7),(11,'Eleg maganyos hogy igy magamba beszelgetek nem?',0,'2026-03-23 07:52:20',0,1,7),(12,'De, de, az, magyanyosak vagyunk',0,'2026-03-23 07:52:32',0,2,7),(13,'Csmo',0,'2026-03-23 09:27:49',0,2,8),(14,'szla',0,'2026-03-23 09:28:33',0,2,8),(15,'szla',0,'2026-03-23 09:28:34',0,2,8),(16,'szla',0,'2026-03-23 09:28:35',0,2,8),(17,'szla',0,'2026-03-23 09:28:35',0,2,8),(18,'szia osszetortem a kocsit',0,'2026-03-23 09:28:49',0,2,8),(19,'szia!',0,'2026-03-27 09:55:18',0,1,12),(20,'utv',0,'2026-03-27 09:56:08',0,6,12),(21,'ars',0,'2026-03-27 10:33:16',0,6,13);
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
) ENGINE=InnoDB AUTO_INCREMENT=57 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `notifications`
--

LOCK TABLES `notifications` WRITE;
/*!40000 ALTER TABLE `notifications` DISABLE KEYS */;
INSERT INTO `notifications` VALUES (27,3,1,'A Skoda Octavia-ra/re vonatkozó bérlés törölve lett!','2026-03-23 00:11:18',0),(32,2,1,'A bérlési ajánlat Teszt Elek-val/vel, a(z) Toyota Corolla-ra/re elfogadásra került.','2026-03-23 09:30:09',0);
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
  `renterRating` double DEFAULT NULL,
  `ownerRating` double DEFAULT NULL,
  `renterId` int NOT NULL,
  `vehicleId` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `renter` (`renterId`),
  KEY `vehicle` (`vehicleId`),
  CONSTRAINT `rentals_ibfk_1` FOREIGN KEY (`vehicleId`) REFERENCES `vehicles` (`id`),
  CONSTRAINT `rentals_ibfk_2` FOREIGN KEY (`renterId`) REFERENCES `users` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rentals`
--

LOCK TABLES `rentals` WRITE;
/*!40000 ALTER TABLE `rentals` DISABLE KEYS */;
INSERT INTO `rentals` VALUES (1,15000,'2026-01-05 10:00:00','2026-01-05 18:00:00',8,'9700 Szombathely, Zrínyi Ilona utca 12.',5,4.5,3,1),(2,25000,'2026-01-11 09:00:00','2026-01-13 17:00:00',7,'9700 Szombathely, Kéthly Anna utca 12.',NULL,NULL,4,2),(5,39200,'2026-03-24 11:00:00','2026-03-26 12:00:00',2,'Szombathely, Fő tér 1.',NULL,NULL,2,1),(6,16200,'2026-03-22 23:55:00','2026-03-24 12:00:00',8,'Szombathely, Fő tér 1.',NULL,4.5,2,4),(7,43200,'2026-03-27 10:00:00','2026-03-29 09:00:00',2,'Szombathely, Kéthly Anna utca 12.',NULL,NULL,2,1),(8,42000,'2026-03-30 10:00:00','2026-04-01 22:00:00',4,'Otthon',NULL,NULL,2,1),(9,14828,'2026-03-23 10:49:00','2026-03-24 08:00:00',8,'Szombathely, Zrínyi Ilona utca 12.',NULL,5,1,6),(10,19200,'2026-03-26 23:00:00','2026-03-27 23:00:00',8,'Szombathely, Fő tér 12.',5,4.5,1,6),(12,23000,'2026-03-28 11:00:00','2026-03-29 10:00:00',2,'Szombathely, Fo ter 12.',NULL,NULL,1,5),(13,20835,'2026-03-28 11:00:00','2026-03-29 10:09:00',3,'Szombathely, Zrin',NULL,NULL,1,6);
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
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'123456AA','Teszt Elek','36201234567','2004-04-18',NULL,'tesztelek@teszt.hu',_binary '��5�\�p\�㶆\�6\�P\�\�N-�G\�-5}��9Ü\����\�&M\�\�*9h0\�\�c��\���\0s�\�',_binary '\��\�㠄�Z\nY�','User','AA123456','9700','Szombathely','Zrínyi Ilona utca 12.',58845),(2,'123456BB','Gipsz Jakab','36701234567','1995-07-21',NULL,'gipszjakab@teszt.hu',_binary '6%\����\�ڷ�2\��\�\�!=\Z!��LQm\\:\�wA0\0)U\�5Q���Ua\����m\�t�\�/b\�\�݌�\�y',_binary '�N٠\r\�#�\�PF3x�ح','User','BB123456','1117','Budapest','Budafoki út 12.',440815),(3,'123456CC','Vincs Eszter','36301234567','2000-11-02',NULL,'vincseszter@teszt.hu',_binary 'L^t��*p���{Ɨ�\�\�4��Ӏt*�<)�l��a\�\�	�f�\�$B�0<8<}j�<�\�\�T�h',_binary '}|��ߚ�XL=�\�','User','CC123456','9700','Szombathely','Kéthly Anna utca 7.',0),(4,'123456DD','Teszt Teréz','36707654321','1989-12-12',NULL,'tesztterez@teszt.hu',_binary 'j^�gWQ$Y��\�\�3ٿ_N\�~\�\�M��Cm5[W��KaHjK�@h/��+��\�5�S+��!\ZЃ<�.',_binary 'tM����H�r6�b��\�','User','DD123456','1095','Budapest','Tinódi utca 1.',0),(5,'623412AD','Admin Tamás','36704124536','1978-09-07',NULL,'admin@teszt.hu',_binary '\�\0W]�\�Cèl�vҬx�VQy\�\�Ր��Zc\��k\\\�\�\�n\�\Z�-YR\�\�3Lگ\��\�h��\�\�\�\�U',_binary '\"\�7\\�C\�LS��	\�T\'G','Administrator',NULL,'9700','Szombathely','Zrínyi Ilona utca 12.',0),(6,'124124AA','Csaba Bence','36208192471','2005-12-01',NULL,'csaba.bence@hbsz.edu.hu',_binary '�-}z\\�\�^\��S�:S\�^�꡹\�\�A��\��\�`d��\�?�z�\�e\�H\��?kK�d�\�\�nK��{\�',_binary '���\�\�_A�|6\�','User',NULL,'9700','Szombathely','ABC utca 1.',77863),(7,'737373GA','Geo Áron','36705152882','2006-06-22',NULL,'me@arongeo.com',_binary '-՟ ,p\�P\�@/\�\�%9���I�H\�s{M9\'�]D��I��� �7�uo;\�1�\�D)Z0\�\�k;��ݻ\"\�o',_binary '\��çI\�J��\�ދ\"[S','Administrator',NULL,'9700','Szombathely','Zrínyi Ilona utca 12.',0);
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usertokens`
--

DROP TABLE IF EXISTS `usertokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usertokens` (
  `id` int NOT NULL AUTO_INCREMENT,
  `userId` int NOT NULL,
  `token` varchar(8) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `type` varchar(15) COLLATE utf8mb4_hungarian_ci NOT NULL,
  `timeCreated` datetime NOT NULL DEFAULT (utc_timestamp()),
  PRIMARY KEY (`id`),
  KEY `usertokens_users_id_fk` (`userId`),
  CONSTRAINT `usertokens_users_id_fk` FOREIGN KEY (`userId`) REFERENCES `users` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usertokens`
--

LOCK TABLES `usertokens` WRITE;
/*!40000 ALTER TABLE `usertokens` DISABLE KEYS */;
/*!40000 ALTER TABLE `usertokens` ENABLE KEYS */;
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
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vehicleavailabilities`
--

LOCK TABLES `vehicleavailabilities` WRITE;
/*!40000 ALTER TABLE `vehicleavailabilities` DISABLE KEYS */;
INSERT INTO `vehicleavailabilities` VALUES (11,1,6,'2026-04-03 10:00:00','2026-05-03 15:00:00',800),(17,5,2,'2026-04-14 08:39:06','2026-04-17 10:00:00',900),(18,5,3,'2026-04-17 10:00:00','2026-04-19 17:00:00',1000),(19,6,4,'2026-04-14 08:56:12','2026-04-17 10:00:00',500),(20,6,5,'2026-04-17 10:00:00','2026-04-19 16:00:00',700);
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
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vehicleimages`
--

LOCK TABLES `vehicleimages` WRITE;
/*!40000 ALTER TABLE `vehicleimages` DISABLE KEYS */;
INSERT INTO `vehicleimages` VALUES (1,1,1,1,'res/58274674-91a9-4afb-ab80-52a0a605c520.jpg'),(2,2,1,1,'res/64725ad2-8502-4eef-b4f3-07d3c5af9423.jpg'),(3,3,1,1,'res/43b1c8fd-ce7c-4c34-8dfc-f46d2e12b6ba.png'),(4,1,2,2,'res/3bd9f544-f633-4a6f-8a29-0463842e19d2.jpg'),(6,4,1,1,'res/c27ef141-b934-4520-94a3-7a6fd3bd7301.jpg'),(9,5,1,1,'res/8d86abed-d261-4f78-a917-ceb558726993.jpeg'),(10,6,1,1,'res/72b8619a-af94-4b8c-b4eb-25df5d7514a2.jpg');
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
  UNIQUE KEY `vehicles_pk` (`vin`),
  UNIQUE KEY `vehicles_pk_2` (`licensePlate`),
  KEY `owner` (`ownerId`),
  CONSTRAINT `vehicles_ibfk_1` FOREIGN KEY (`ownerId`) REFERENCES `users` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_hungarian_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vehicles`
--

LOCK TABLES `vehicles` WRITE;
/*!40000 ALTER TABLE `vehicles` DISABLE KEYS */;
INSERT INTO `vehicles` VALUES (1,1,'VF312345678901234','ABC123','Toyota','Corolla',2018,'Megbízható hibrid városi cirkáló.',85000,132,4.5,'benzin-elektromos','KGFB-998877','automatikus'),(2,2,'WBA41234567890123','SKY789','BMW','320d',2015,'Kényelmes utazóautó hosszabb távra.',210000,180,6.2,'dízel','KGFB-112233','manuális'),(3,3,'TMB51234567890123','RNL456','Skoda','Octavia',2020,'Hatalmas csomagtartó, tiszta belső.',45000,150,5.5,'benzin','KGFB-445566','manuális'),(4,1,'DEFGHIJK12341241A','ABCD123','Suzuki','Swift',2018,'',5,120,6.2,'Benzin','KGFB-123412515','Manuális'),(5,6,'S12124NEINHSR1231','BENC369','BMW','E36 M3',1999,'Nagyon meno auto',150000,321,12,'Benzin','KFB-ul12j491247961','Manuális'),(6,6,'RNTDHIENSTHDNE124','KKL724','Ford','Focus',2002,'',446800,116,6,'Dízel','neirosdeiarsnd12947','Manuális');
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

-- Dump completed on 2026-04-15  0:30:36
