-- phpMyAdmin SQL Dump
-- version 4.6.5.2
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Jul 07, 2017 at 02:26 AM
-- Server version: 10.1.21-MariaDB
-- PHP Version: 5.6.30

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `daily_report`
--

-- --------------------------------------------------------

--
-- Table structure for table `car_daily`
--

CREATE TABLE `car_daily` (
  `car_daily_id` varchar(255) COLLATE utf8_bin NOT NULL,
  `car_id` varchar(255) COLLATE utf8_bin DEFAULT NULL,
  `imei` varchar(255) COLLATE utf8_bin DEFAULT NULL,
  `daily_date` varchar(255) COLLATE utf8_bin DEFAULT NULL,
  `distance` varchar(255) COLLATE utf8_bin DEFAULT NULL,
  `income` decimal(17,2) DEFAULT NULL,
  `trip_cnt` varchar(255) COLLATE utf8_bin DEFAULT NULL,
  `trip_distance` varchar(255) COLLATE utf8_bin DEFAULT NULL
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `car_daily`
--
ALTER TABLE `car_daily`
  ADD PRIMARY KEY (`car_daily_id`);

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
