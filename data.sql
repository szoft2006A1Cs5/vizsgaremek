USE comove;

-- Teszt Elek - tesztelek@teszt.hu - NagyTesztElek32
-- Gipsz Jakab - gipszjakab@teszt.hu - KisGipszJakab64
-- Vincs Eszter - vincseszter@teszt.hu - NagyVincsEszter128
-- Teszt Teréz - tesztterez@teszt.hu - KisTesztTerez256 (Nem ez a jelszava :/)
INSERT INTO `users` (`id`, `idCardNumber`, `name`, `phone`, `dateOfBirth`, `profilePicPath`, `email`, `password`, `salt`, `role`, `driversLicenseNumber`, `driversLicenseDate`, `addressZipcode`, `addressSettlement`, `addressStreetHouse`, `balance`) VALUES
(1, '123456AA', 'Teszt Elek', '36201234567', '2004-04-18', NULL, 'tesztelek@teszt.hu', 0x5cd79118803c295ee4566a87a59423e7b4b020194520f52e78bddcdbfb36daef43b032e7a122323e5849344ff1fd625a7885c3ff62688a0241b7e4018ed3d9e0, 0x945200f84cef838d8d44e0121415fa53, 'User', 'AA123456', '2024-02-04', '9700', 'Szombathely', 'Zrínyi Ilona utca 12.', 0),
(2, '123456BB', 'Gipsz Jakab', '36701234567', '1995-07-21', NULL, 'gipszjakab@teszt.hu', 0xd08e0fc7c893f8e40935a39f87be3b90c43d63f563ff851e64469d0cdd468dd0ee73881850bb01a97a084fdaae5b634816dabd10c2b41dcbabef3705adbf16d9, 0x881c6de362d428ab7db9241bbe10d4ac, 'User', 'BB123456', '2017-03-12', '1117', 'Budapest', 'Budafoki út 12.', 0),
(3, '123456CC', 'Vincs Eszter', '36301234567', '2000-11-02', NULL, 'vincseszter@teszt.hu', 0xe9699389486299dda48bda984e52ca3b8d1d13a16ce95ebea830ca087980920867bfd72fc2846cb600cf8c4157c7af4059d6ca032292c947614ebe716c969493, 0xb1c0e441552bef260ef5b1d075e2c871, 'User', 'CC123456', '2019-10-09', '9700', 'Szombathely', 'Kéthly Anna utca 7.', 0),
(4, '123456DD', 'Teszt Teréz', '36707654321', '1989-12-12', NULL, 'tesztterez@teszt.hu', 0xb1fa19e5947dd5b0395e12c4a41fd93c723587e753db40e10003c27e9635f1dd5951748bc18effae1e42fc1df5fafa7b30d37390f01dfca893613ad58042516b, 0x04c98372f19591c29d3416242bdd64e6, 'User', 'DD123456', '2014-08-19', '1095', 'Budapest', 'Tinódi utca 1.', 0);

INSERT INTO `vehicles` (`id`, `ownerId`, `vin`, `licensePlate`, `manufacturer`, `model`, `year`, `description`, `odometerReading`, `avgFuelConsumption`, `insuranceNumber`) VALUES
(1, 1, 'VF312345678901234', 'ABC-123', 'Toyota', 'Corolla', 2018, 'Megbízható hibrid városi cirkáló.', 85000, 4.5, 'KGFB-998877'),
(2, 2, 'WBA41234567890123', 'SKY-789', 'BMW', '320d', 2015, 'Kényelmes utazóautó hosszabb távra.', 210000, 6.2, 'KGFB-112233'),
(3, 3, 'TMB51234567890123', 'RNL-456', 'Skoda', 'Octavia', 2020, 'Hatalmas csomagtartó, tiszta belső.', 45000, 5.5, 'KGFB-445566');

INSERT INTO `vehicleavailabilities` (`vehicleId`, `start`, `end`, `recurrence`, `hourlyRate`) VALUES
(1, '2026-01-01 08:00:00', '2026-12-31 20:00:00', 'None', 1800),
(2, '2026-01-10 00:00:00', '2026-02-10 00:00:00', 'None', 2500),
(3, '2026-01-12 08:00:00', '2026-01-20 20:00:00', 'Weekly', 2000);

INSERT INTO `vehicleimages` (`vehicleId`, `imagePath`) VALUES
(1, 'uploads/vehicles/toyota_corolla_front.jpg'),
(2, 'uploads/vehicles/bmw_320d_side.png'),
(3, 'uploads/vehicles/skoda_octavia.jpg');

INSERT INTO `rentals` (`id`, `fullPrice`, `downpayment`, `start`, `end`, `status`, `pickupLatitude`, `pickupLongtitude`, `fuelLevel`, `renterRating`, `ownerRating`, `renterId`, `vehicleId`) VALUES
(1, 15000, 3000, '2026-01-05 10:00:00', '2026-01-05 18:00:00', 'Finished', 47.1234, 18.4567, 100, 5.0, 4.5, 3, 1),
(2, 25000, 5000, '2026-01-11 09:00:00', '2026-01-13 17:00:00', 'Active', 47.4979, 19.0402, 75.5, NULL, NULL, 4, 2),
(3, 8000, 1500, '2026-01-15 08:00:00', '2026-01-15 12:00:00', 'RenterOffer', 47.2345, 16.6321, NULL, NULL, NULL, 1, 3);

INSERT INTO `messages` (`id`, `content`, `timeSent`, `isComplaint`, `senderId`, `rentalId`) VALUES
(1, 'Szia! Megérkeztem az autóhoz, minden rendben tűnik.', '2026-01-11 08:55:00', 0, 4, 2),
(2, 'Szuper, a kulcs a kijelölt helyen volt?', '2026-01-11 08:57:00', 0, 2, 2);

INSERT INTO `notifications` (`userId`, `content`, `timeSent`, `read`) VALUES
(2, 'Új üzeneted érkezett Teszt Teréztől a BMW bérléssel kapcsolatban.', '2026-01-11 08:55:05', 1),
(3, 'Új bérlési ajánlatod érkezett a Skodára Teszt Elektől!', '2026-01-12 00:05:00', 0);
