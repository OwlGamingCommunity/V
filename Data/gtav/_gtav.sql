-- --------------------------------------------------------
-- Host:                         database
-- Server version:               10.4.12-MariaDB-1:10.4.12+maria~bionic - mariadb.org binary distribution
-- Server OS:                    debian-linux-gnu
-- HeidiSQL Version:             11.0.0.5919
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- Dumping structure for table gtav.achievements
CREATE TABLE IF NOT EXISTS `achievements` (
  `id` int(20) NOT NULL AUTO_INCREMENT,
  `account` int(11) DEFAULT NULL,
  `achievement_id` int(11) DEFAULT NULL,
  `unlocked` bigint(20) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8377 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.banks
CREATE TABLE IF NOT EXISTS `banks` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `x` float NOT NULL,
  `y` float NOT NULL,
  `z` float NOT NULL,
  `rz` float NOT NULL,
  `type` int(11) NOT NULL,
  `dimension` int(11) unsigned NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=217 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.carwash_points
CREATE TABLE IF NOT EXISTS `carwash_points` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `x` float NOT NULL,
  `y` float NOT NULL,
  `z` float NOT NULL,
  `dimension` int(11) unsigned NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.characters
CREATE TABLE IF NOT EXISTS `characters` (
  `id` bigint(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(32) NOT NULL,
  `account` int(11) NOT NULL,
  `x` float NOT NULL DEFAULT -130.559,
  `y` float NOT NULL DEFAULT 6325.08,
  `z` float NOT NULL DEFAULT 31.5339,
  `last_seen` timestamp NULL DEFAULT NULL,
  `gender` int(1) NOT NULL DEFAULT 0,
  `age` int(2) NOT NULL DEFAULT 18,
  `rz` float NOT NULL DEFAULT 133.91,
  `money` float(15,2) NOT NULL DEFAULT 0.00,
  `bank_money` float(15,2) NOT NULL DEFAULT 0.00,
  `health` int(11) NOT NULL DEFAULT 100,
  `armor` int(11) NOT NULL DEFAULT 0,
  `job` int(2) NOT NULL DEFAULT -1,
  `trucker_job_xp` int(11) NOT NULL DEFAULT 0,
  `deliverydriver_job_xp` int(11) NOT NULL DEFAULT 0,
  `busdriver_job_xp` int(11) NOT NULL DEFAULT 0,
  `mailman_job_xp` int(11) NOT NULL DEFAULT 0,
  `trashman_job_xp` int(11) NOT NULL DEFAULT 0,
  `fishing_xp` int(11) NOT NULL DEFAULT 0,
  `dimension` int(11) unsigned NOT NULL DEFAULT 0,
  `payday_progress` int(2) NOT NULL DEFAULT 0,
  `cuffed` tinyint(1) NOT NULL DEFAULT 0,
  `cuffer` int(10) NOT NULL DEFAULT 0,
  `unjail_time` bigint(20) NOT NULL DEFAULT 0,
  `cell_number` int(11) NOT NULL DEFAULT 0,
  `bail_amount` float NOT NULL DEFAULT 0,
  `jail_reason` text NOT NULL,
  `duty` int(11) NOT NULL DEFAULT 0,
  `cked` tinyint(1) NOT NULL DEFAULT 0,
  `type` int(1) NOT NULL DEFAULT 0,
  `pending_job_money` float(15,2) NOT NULL DEFAULT 0.00,
  `impairment` float(3,2) NOT NULL DEFAULT 0.00,
  `drug_fx1` tinyint(1) NOT NULL DEFAULT 0,
  `drug_fx2` tinyint(1) NOT NULL DEFAULT 0,
  `drug_fx3` tinyint(1) NOT NULL DEFAULT 0,
  `drug_fx4` tinyint(1) NOT NULL DEFAULT 0,
  `drug_fx5` tinyint(1) NOT NULL DEFAULT 0,
  `drug_fx1_duration` int(11) NOT NULL DEFAULT 0,
  `drug_fx2_duration` int(11) NOT NULL DEFAULT 0,
  `drug_fx3_duration` int(11) NOT NULL DEFAULT 0,
  `drug_fx4_duration` int(11) NOT NULL DEFAULT 0,
  `drug_fx5_duration` int(11) NOT NULL DEFAULT 0,
  `first_use` tinyint(1) NOT NULL DEFAULT 1,
  `minutes_played` int(10) unsigned DEFAULT 0,
  `pending_firearms_lic_state_tier1` int(1) DEFAULT 0,
  `pending_firearms_lic_state_tier2` int(1) DEFAULT 0,
  `tag` text DEFAULT NULL,
  `tag_wip` text DEFAULT NULL,
  `show_spawnselector` tinyint(1) DEFAULT 0,
  `creation_version` int(4) DEFAULT -1,
  `current_version` int(4) DEFAULT -1,
  `premade_masked` tinyint(1) NOT NULL DEFAULT 0,
  `marijuana_given` tinyint(1) NOT NULL DEFAULT 0,
  `source` int(3) DEFAULT 0,
  PRIMARY KEY (`id`),
  UNIQUE KEY `NAME` (`name`)
) ENGINE=InnoDB AUTO_INCREMENT=2556 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.characters_custom_data
CREATE TABLE IF NOT EXISTS `characters_custom_data` (
  `char_id` bigint(20) NOT NULL,
  `ageing` int(11) NOT NULL DEFAULT 0,
  `ageing_opacity` float NOT NULL DEFAULT 0,
  `makeup` int(11) NOT NULL DEFAULT 0,
  `makeup_opacity` float NOT NULL DEFAULT 0,
  `makeup_color` int(11) NOT NULL DEFAULT 0,
  `makeup_color_highlight` int(11) NOT NULL DEFAULT 0,
  `blush` int(11) NOT NULL DEFAULT 0,
  `blush_opacity` float NOT NULL DEFAULT 0,
  `blush_color` int(11) NOT NULL DEFAULT 0,
  `blush_color_highlight` int(11) NOT NULL DEFAULT 0,
  `complexion` int(11) NOT NULL DEFAULT 0,
  `complexion_opacity` float NOT NULL DEFAULT 0,
  `sundamage` int(11) NOT NULL DEFAULT 0,
  `sundamage_opacity` float NOT NULL DEFAULT 0,
  `lipstick` int(11) NOT NULL DEFAULT 0,
  `lipstick_opacity` float NOT NULL DEFAULT 0,
  `lipstick_color` int(11) NOT NULL DEFAULT 0,
  `lipstick_color_highlights` int(11) NOT NULL DEFAULT 0,
  `moles_and_freckles` int(11) NOT NULL DEFAULT 0,
  `moles_and_freckles_opacity` float NOT NULL DEFAULT 0,
  `nose_size_horizontal` float NOT NULL DEFAULT 0,
  `nose_size_vertical` float NOT NULL DEFAULT 0,
  `nose_size_outwards` float NOT NULL DEFAULT 0,
  `nose_size_outwards_upper` float NOT NULL DEFAULT 0,
  `nose_size_outwards_lower` float NOT NULL DEFAULT 0,
  `nose_angle` float NOT NULL DEFAULT 0,
  `eyebrow_height` float NOT NULL DEFAULT 0,
  `eyebrow_depth` float NOT NULL DEFAULT 0,
  `cheekbone_height` float NOT NULL DEFAULT 0,
  `cheek_width` float NOT NULL DEFAULT 0,
  `cheek_width_lower` float NOT NULL DEFAULT 0,
  `eye_size` float NOT NULL DEFAULT 0,
  `lip_size` float NOT NULL DEFAULT 0,
  `mouth_size` float NOT NULL DEFAULT 0,
  `mouth_size_lower` float NOT NULL DEFAULT 0,
  `chin_size` float NOT NULL DEFAULT 0,
  `chin_size_lower` float NOT NULL DEFAULT 0,
  `chin_width` float NOT NULL DEFAULT 0,
  `chin_effect` float NOT NULL DEFAULT 0,
  `neck_width` float NOT NULL DEFAULT 0,
  `neck_width_lower` float NOT NULL DEFAULT 0,
  `face_blend_1_mother` int(11) NOT NULL DEFAULT 0,
  `face_blend_1_father` int(11) NOT NULL DEFAULT 0,
  `face_blend_1_third` int(11) NOT NULL DEFAULT 0,
  `face_blend_2_third` int(11) NOT NULL DEFAULT 0,
  `face_blend_father_percent` float NOT NULL DEFAULT 0,
  `skin_blend_father_percent` float NOT NULL DEFAULT 0,
  `third_blend_percent` float NOT NULL DEFAULT 0,
  `hair_style` int(11) NOT NULL DEFAULT 0,
  `base_hair` int(11) NOT NULL DEFAULT -1,
  `hair_color` int(11) NOT NULL DEFAULT 0,
  `hair_color_highlight` int(11) NOT NULL DEFAULT 0,
  `eye_color` int(11) NOT NULL DEFAULT 0,
  `facial_hair_style` int(11) NOT NULL DEFAULT 0,
  `facial_hair_color` int(11) NOT NULL DEFAULT 0,
  `facial_hair_color_highlight` int(11) NOT NULL DEFAULT 0,
  `facial_hair_opacity` float NOT NULL DEFAULT 0,
  `blemishes` int(11) DEFAULT NULL,
  `blemishes_opacity` float NOT NULL DEFAULT 0,
  `eyebrows` int(11) NOT NULL DEFAULT 0,
  `eyebrows_opacity` float NOT NULL DEFAULT 0,
  `eyebrows_color` int(11) NOT NULL DEFAULT 0,
  `eyebrows_color_highlight` int(11) NOT NULL DEFAULT 0,
  `body_blemishes` int(11) NOT NULL DEFAULT 0,
  `body_blemishes_opacity` float NOT NULL DEFAULT 0,
  `chest_hair` int(11) NOT NULL DEFAULT 0,
  `chest_hair_color` int(11) NOT NULL DEFAULT 0,
  `chest_hair_color_highlights` int(11) NOT NULL DEFAULT 0,
  `chest_hair_opacity` float NOT NULL DEFAULT 0,
  `full_beard_style` int(3) NOT NULL DEFAULT 0,
  `full_beard_color` int(3) NOT NULL DEFAULT 0,
  PRIMARY KEY (`char_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.characters_tattoos
CREATE TABLE IF NOT EXISTS `characters_tattoos` (
  `char_id` bigint(20) NOT NULL,
  `tattoo_id` int(4) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.character_kills
CREATE TABLE IF NOT EXISTS `character_kills` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `character_id` int(11) NOT NULL,
  `reason` varchar(50) NOT NULL,
  `admin` int(11) NOT NULL,
  `ck_date` datetime NOT NULL DEFAULT current_timestamp(),
  `x` float NOT NULL,
  `y` float NOT NULL,
  `z` float NOT NULL,
  `rz` float NOT NULL,
  `dimension` int(11) NOT NULL,
  `buried` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=45 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.character_languages
CREATE TABLE IF NOT EXISTS `character_languages` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `language_id` int(11) NOT NULL,
  `progress` float NOT NULL,
  `parent` bigint(20) NOT NULL,
  `active` tinyint(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`),
  UNIQUE KEY `unique_index` (`language_id`,`parent`)
) ENGINE=InnoDB AUTO_INCREMENT=157 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.character_looks
CREATE TABLE IF NOT EXISTS `character_looks` (
  `character_id` bigint(11) NOT NULL,
  `height` int(3) NOT NULL,
  `weight` int(3) NOT NULL,
  `physical_appearance` varchar(255) NOT NULL,
  `scars` varchar(255) NOT NULL,
  `tattoos` text NOT NULL,
  `makeup` varchar(255) NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `updated_at` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  PRIMARY KEY (`character_id`),
  CONSTRAINT `character_looks_character_id_foreign` FOREIGN KEY (`character_id`) REFERENCES `characters` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.chat_settings
CREATE TABLE IF NOT EXISTS `chat_settings` (
  `account_id` int(11) NOT NULL,
  `num_messages` int(3) NOT NULL,
  `tab_0` text NOT NULL,
  `tab_1` text NOT NULL,
  `tab_2` text NOT NULL,
  `tab_3` text NOT NULL,
  `background` tinyint(1) DEFAULT 1,
  `background_alpha` float DEFAULT 0.8,
  PRIMARY KEY (`account_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.custom_anims
CREATE TABLE IF NOT EXISTS `custom_anims` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `account_id` bigint(20) unsigned NOT NULL,
  `command_name` varchar(100) NOT NULL,
  `anim_dictionary` varchar(100) NOT NULL,
  `anim_name` varchar(100) NOT NULL,
  `loop` tinyint(1) NOT NULL DEFAULT 1,
  `stop_on_last_frame` tinyint(1) NOT NULL DEFAULT 0,
  `only_animate_upper_body` tinyint(1) NOT NULL DEFAULT 0,
  `allow_player_movement` tinyint(1) NOT NULL DEFAULT 0,
  `duration` int(10) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4;

-- Dumping structure for table gtav.corpses
CREATE TABLE IF NOT EXISTS `corpses` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `x` float NOT NULL,
  `y` float NOT NULL,
  `z` float NOT NULL,
  `rz` int(11) NOT NULL,
  `dimension` float NOT NULL,
  `character_id` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.custom_interior_maps
CREATE TABLE IF NOT EXISTS `custom_interior_maps` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `property_id` int(10) NOT NULL,
  `marker_x` float NOT NULL,
  `marker_y` float NOT NULL,
  `marker_z` float NOT NULL,
  `uploaded_at` datetime DEFAULT current_timestamp(),
  `updated_at` datetime DEFAULT current_timestamp(),
  PRIMARY KEY (`id`),
  KEY `custom_interior_maps_property_id_created_at_index` (`property_id`,`uploaded_at`),
  CONSTRAINT `custom_interior_maps_property_id_foreign` FOREIGN KEY (`property_id`) REFERENCES `properties` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.custom_interior_objects
DROP TABLE IF EXISTS `custom_interior_objects`;
CREATE TABLE IF NOT EXISTS `custom_interior_objects` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `model` varchar(255) NOT NULL,
  `position_x` float NOT NULL,
  `position_y` float NOT NULL,
  `position_z` float NOT NULL,
  `rotation_x` float NOT NULL,
  `rotation_y` float NOT NULL,
  `rotation_z` float NOT NULL,
  `map_id` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7237 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.dancers
CREATE TABLE IF NOT EXISTS `dancers` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `x` float NOT NULL,
  `y` float NOT NULL,
  `z` float NOT NULL,
  `rz` float NOT NULL,
  `dimension` int(10) unsigned NOT NULL,
  `skin` int(20) unsigned NOT NULL,
  `parent_property` int(11) NOT NULL,
  `tip_money` float NOT NULL,
  `allow_tip` tinyint(1) NOT NULL,
  `anim_dict` varchar(50) NOT NULL,
  `anim_name` varchar(50) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.disallowed_character_names
CREATE TABLE IF NOT EXISTS `disallowed_character_names` (
  `name` varchar(32) NOT NULL,
  PRIMARY KEY (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.donation_inventory
CREATE TABLE IF NOT EXISTS `donation_inventory` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `account` int(11) NOT NULL,
  `character_id` bigint(11) NOT NULL,
  `time_activated` bigint(20) NOT NULL DEFAULT 0,
  `time_expire` bigint(20) NOT NULL DEFAULT 0,
  `donation_id` int(11) unsigned NOT NULL,
  `vehicle_id` bigint(20) NOT NULL DEFAULT -1,
  `property_id` bigint(20) NOT NULL DEFAULT -1,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6102 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.donation_store
CREATE TABLE IF NOT EXISTS `donation_store` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `title` varchar(32) NOT NULL,
  `desc` varchar(64) NOT NULL,
  `cost` int(11) NOT NULL DEFAULT 5,
  `type` int(11) NOT NULL,
  `unique` tinyint(1) NOT NULL,
  `duration` int(11) NOT NULL DEFAULT -1,
  `effect` int(1) NOT NULL DEFAULT 0,
  `active` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=57 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.duty_points
CREATE TABLE IF NOT EXISTS `duty_points` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `type` int(11) NOT NULL,
  `x` float NOT NULL,
  `y` float NOT NULL,
  `z` float NOT NULL,
  `dimension` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.elevators
CREATE TABLE IF NOT EXISTS `elevators` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `entrance_x` float DEFAULT NULL,
  `entrance_y` float DEFAULT NULL,
  `entrance_z` float DEFAULT NULL,
  `exit_x` float DEFAULT NULL,
  `exit_y` float DEFAULT NULL,
  `exit_z` float DEFAULT NULL,
  `exit_dimension` int(6) unsigned DEFAULT 0,
  `entrance_dimension` int(6) unsigned DEFAULT 0,
  `car` tinyint(1) NOT NULL DEFAULT 0,
  `entrance_rot` float DEFAULT NULL,
  `exit_rot` float DEFAULT NULL,
  `name` varchar(255) NOT NULL DEFAULT 'Elevator',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=423 DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC;

-- Data exporting was unselected.

-- Dumping structure for table gtav.emails
CREATE TABLE IF NOT EXISTS `emails` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `sender_id` int(10) unsigned NOT NULL,
  `reply_to_id` bigint(20) unsigned DEFAULT NULL,
  `subject` varchar(255) NOT NULL,
  `body` text NOT NULL,
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `last_updated_at` datetime NOT NULL DEFAULT current_timestamp(),
  PRIMARY KEY (`id`),
  KEY `sender_id` (`sender_id`),
  KEY `reply_to_id` (`reply_to_id`),
  KEY `last_updated_at` (`last_updated_at`),
  CONSTRAINT `emails_ibfk_1` FOREIGN KEY (`sender_id`) REFERENCES `email_accounts` (`id`),
  CONSTRAINT `emails_ibfk_2` FOREIGN KEY (`reply_to_id`) REFERENCES `emails` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.email_accounts
CREATE TABLE IF NOT EXISTS `email_accounts` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `creator_id` bigint(20) NOT NULL,
  `domain_id` int(10) unsigned NOT NULL,
  `username` varchar(70) NOT NULL,
  `password` varchar(255) NOT NULL,
  `address` varchar(100) NOT NULL,
  `signature` text NOT NULL,
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  PRIMARY KEY (`id`),
  KEY `creator_id` (`creator_id`),
  KEY `domain_id` (`domain_id`),
  CONSTRAINT `email_accounts_ibfk_1` FOREIGN KEY (`creator_id`) REFERENCES `characters` (`id`),
  CONSTRAINT `email_accounts_ibfk_2` FOREIGN KEY (`domain_id`) REFERENCES `email_domains` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=25 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.email_contacts
CREATE TABLE IF NOT EXISTS `email_contacts` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `account_id` int(10) unsigned NOT NULL,
  `name` varchar(100) NOT NULL,
  `address` varchar(100) NOT NULL,
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  PRIMARY KEY (`id`),
  KEY `account_id` (`account_id`),
  CONSTRAINT `email_contacts_ibfk_1` FOREIGN KEY (`account_id`) REFERENCES `email_accounts` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.email_domains
CREATE TABLE IF NOT EXISTS `email_domains` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `faction_id` bigint(20) DEFAULT NULL,
  `domain` varchar(20) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `faction_id` (`faction_id`),
  CONSTRAINT `email_domains_ibfk_1` FOREIGN KEY (`faction_id`) REFERENCES `factions` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.email_recipients
CREATE TABLE IF NOT EXISTS `email_recipients` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `email_id` bigint(20) unsigned NOT NULL,
  `recipient_id` int(10) unsigned NOT NULL,
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `seen_at` datetime DEFAULT NULL,
  `deleted_at` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `email_id` (`email_id`),
  KEY `recipient_id` (`recipient_id`,`created_at`),
  CONSTRAINT `email_recipients_ibfk_1` FOREIGN KEY (`email_id`) REFERENCES `emails` (`id`),
  CONSTRAINT `email_recipients_ibfk_2` FOREIGN KEY (`recipient_id`) REFERENCES `email_accounts` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.factions
CREATE TABLE IF NOT EXISTS `factions` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `type` int(1) NOT NULL,
  `name` varchar(64) NOT NULL,
  `official` tinyint(1) NOT NULL,
  `short_name` varchar(9) NOT NULL,
  `message` varchar(300) CHARACTER SET utf8 NOT NULL,
  `money` float(15,2) NOT NULL DEFAULT 0.00,
  `creator` bigint(20) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=227 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.faction_invites
CREATE TABLE IF NOT EXISTS `faction_invites` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `target_character` bigint(20) NOT NULL,
  `source_character` bigint(20) NOT NULL,
  `faction` bigint(20) NOT NULL,
  `timestamp` bigint(20) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=677 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.faction_memberships
CREATE TABLE IF NOT EXISTS `faction_memberships` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `faction_id` bigint(20) NOT NULL,
  `character_id` bigint(20) NOT NULL,
  `rank_index` int(2) unsigned NOT NULL,
  `manager` tinyint(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`),
  KEY `faction_id_memberships` (`faction_id`),
  KEY `character_id_memberships` (`character_id`)
) ENGINE=InnoDB AUTO_INCREMENT=1166 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.faction_ranks
CREATE TABLE IF NOT EXISTS `faction_ranks` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `faction_id` bigint(20) DEFAULT NULL,
  `name` varchar(32) DEFAULT NULL,
  `salary` float DEFAULT NULL,
  `rank_index` int(2) unsigned NOT NULL,
  PRIMARY KEY (`id`),
  KEY `faction_id_ranks` (`faction_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4578 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.fuel_points
CREATE TABLE IF NOT EXISTS `fuel_points` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `x` float NOT NULL,
  `y` float NOT NULL,
  `z` float NOT NULL,
  `dimension` int(11) unsigned NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=189 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.game_accounts
CREATE TABLE IF NOT EXISTS `game_accounts` (
  `account_id` int(11) NOT NULL,
  `app_state` int(1) unsigned DEFAULT NULL,
  `num_apps` int(1) unsigned DEFAULT NULL,
  `app_question1` int(10) unsigned DEFAULT NULL,
  `app_question2` int(10) unsigned DEFAULT NULL,
  `app_question3` int(10) unsigned DEFAULT NULL,
  `app_question4` int(10) unsigned DEFAULT NULL,
  `app_answer1` text DEFAULT NULL,
  `app_answer2` text DEFAULT NULL,
  `app_answer3` text DEFAULT NULL,
  `app_answer4` text DEFAULT NULL,
  `serial` text DEFAULT NULL,
  `admin_notes` text DEFAULT NULL,
  `minutes_played` int(10) unsigned DEFAULT 0,
  `admin_report_count` bigint(20) NOT NULL DEFAULT 0,
  `local_nametag_toggled` tinyint(1) NOT NULL DEFAULT 0,
  `auto_spawn_character` bigint(11) DEFAULT -1,
  `admin_jail_minutes_remaining` int(10) NOT NULL DEFAULT -1,
  `admin_jail_reason` varchar(255) NOT NULL DEFAULT ""
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.game_controls
CREATE TABLE IF NOT EXISTS `game_controls` (
  `account` int(11) NOT NULL,
  `controls` text NOT NULL,
  PRIMARY KEY (`account`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.gangtags
CREATE TABLE IF NOT EXISTS `gangtags` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `owner_char` bigint(11) NOT NULL,
  `x` float NOT NULL,
  `y` float NOT NULL,
  `z` float NOT NULL,
  `rz` float NOT NULL,
  `dim` int(11) unsigned NOT NULL,
  `tagdata` text NOT NULL,
  `progress` float NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=308 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.globals
CREATE TABLE IF NOT EXISTS `globals` (
  `payday_progress` int(11) DEFAULT 0,
  `highest_peak_playercount` int(11) DEFAULT 0,
  `next_property_xp_run_at` bigint(20) unsigned NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.infomarkers
CREATE TABLE IF NOT EXISTS `infomarkers` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `owner_char` bigint(11) NOT NULL,
  `x` float NOT NULL,
  `y` float NOT NULL,
  `z` float NOT NULL,
  `dimension` int(11) unsigned NOT NULL,
  `text` varchar(500) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.inventories
CREATE TABLE IF NOT EXISTS `inventories` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `item_id` bigint(10) NOT NULL,
  `item_value` text NOT NULL,
  `x` float NOT NULL DEFAULT 0,
  `y` float NOT NULL DEFAULT 0,
  `z` float NOT NULL DEFAULT 0,
  `rx` float NOT NULL DEFAULT 0,
  `rz` float NOT NULL DEFAULT 0,
  `ry` float NOT NULL DEFAULT 0,
  `dropped_by` bigint(20) NOT NULL DEFAULT 0,
  `dimension` int(11) unsigned NOT NULL DEFAULT 0,
  `parent_type` int(2) NOT NULL,
  `parent` bigint(20) NOT NULL,
  `current_socket` int(2) NOT NULL DEFAULT 0,
  `stack_size` int(10) unsigned NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=116710 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.keybinds
CREATE TABLE IF NOT EXISTS `keybinds` (
  `id` int(20) unsigned NOT NULL AUTO_INCREMENT,
  `character_id` int(11) NOT NULL DEFAULT -1,
  `account` int(11) NOT NULL,
  `bind_type` int(1) unsigned NOT NULL,
  `bind_action` text CHARACTER SET latin1 NOT NULL,
  `bind_key` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=1305 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.metal_detectors
CREATE TABLE IF NOT EXISTS `metal_detectors` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `position_x` float DEFAULT NULL,
  `position_y` float DEFAULT NULL,
  `position_z` float DEFAULT NULL,
  `rotation` float DEFAULT NULL,
  `dimension` int(6) unsigned DEFAULT 0,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.notifications
CREATE TABLE IF NOT EXISTS `notifications` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `account_id` bigint(20) unsigned NOT NULL,
  `title` varchar(255) NOT NULL,
  `click_event` varchar(255) DEFAULT NULL,
  `body` varchar(255) NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  PRIMARY KEY (`id`),
  KEY `notifications_account_id_created_at_index` (`account_id`,`created_at`)
) ENGINE=InnoDB AUTO_INCREMENT=808 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.perfcaptures
CREATE TABLE IF NOT EXISTS `perfcaptures` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `account_id` bigint(20) unsigned NOT NULL,
  `data` mediumblob NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.phone_contacts
CREATE TABLE IF NOT EXISTS `phone_contacts` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `phone` int(11) DEFAULT NULL,
  `entryName` varchar(50) DEFAULT NULL,
  `entryNumber` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=628 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.phone_sms
CREATE TABLE IF NOT EXISTS `phone_sms` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `from` int(11) NOT NULL,
  `to` int(11) NOT NULL,
  `content` varchar(200) NOT NULL,
  `date` datetime NOT NULL DEFAULT current_timestamp(),
  `viewed` tinyint(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`),
  UNIQUE KEY `ID_UNIQUE` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2596 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.player_admin_history
CREATE TABLE IF NOT EXISTS `player_admin_history` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `account` int(11) NOT NULL,
  `action` text NOT NULL,
  `datetime` timestamp NOT NULL DEFAULT current_timestamp(),
  `amount` int(11) NOT NULL,
  `type` int(5) NOT NULL,
  `admin` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `account_key` (`account`)
) ENGINE=InnoDB AUTO_INCREMENT=67 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.properties
CREATE TABLE IF NOT EXISTS `properties` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `entrance_x` float DEFAULT NULL,
  `entrance_y` float DEFAULT NULL,
  `entrance_z` float DEFAULT NULL,
  `exit_x` float DEFAULT NULL,
  `exit_y` float DEFAULT NULL,
  `exit_z` float DEFAULT NULL,
  `state` int(1) unsigned DEFAULT NULL,
  `buy_price` float unsigned NOT NULL DEFAULT 0,
  `rent_price` float unsigned NOT NULL DEFAULT 0,
  `owner` bigint(20) NOT NULL DEFAULT -1,
  `renter` bigint(20) NOT NULL DEFAULT -1,
  `locked` tinyint(1) NOT NULL DEFAULT 0,
  `owner_type` int(1) NOT NULL DEFAULT 0,
  `renter_type` int(1) NOT NULL DEFAULT 0,
  `name` varchar(48) NOT NULL,
  `entrance_rot` float DEFAULT NULL,
  `exit_rot` float DEFAULT NULL,
  `entrance_dimension` int(6) unsigned DEFAULT 0,
  `interior_id` int(6) DEFAULT -1,
  `payments_made` int(3) unsigned DEFAULT 0,
  `payments_missed` int(1) unsigned DEFAULT 0,
  `payments_remaining` int(3) unsigned NOT NULL DEFAULT 0,
  `entrance_type` int(1) NOT NULL DEFAULT 0,
  `credit_amount` float NOT NULL DEFAULT 0,
  `last_used` bigint(20) NOT NULL DEFAULT 0,
  `scripted_blip` tinyint(1) NOT NULL DEFAULT 0,
  `is_token_purchase` tinyint(1) NOT NULL DEFAULT 0,
  `xp` int(10) unsigned NOT NULL DEFAULT 0,
  `last_mowed_at` bigint(20) unsigned NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`),
  KEY `owner_properties` (`owner`)
) ENGINE=InnoDB AUTO_INCREMENT=1939 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.property_notes
CREATE TABLE IF NOT EXISTS `property_notes` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `property_id` int(10) NOT NULL,
  `creator_id` int(11) NOT NULL,
  `note` varchar(255) NOT NULL,
  `created_at` datetime DEFAULT current_timestamp(),
  PRIMARY KEY (`id`),
  KEY `property_notes_property_id_created_at_index` (`property_id`,`created_at`),
  CONSTRAINT `property_notes_property_id_foreign` FOREIGN KEY (`property_id`) REFERENCES `properties` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=35 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.radios
CREATE TABLE IF NOT EXISTS `radios` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `account` bigint(11) NOT NULL,
  `name` varchar(16) NOT NULL,
  `endpoint` varchar(64) NOT NULL,
  `expiration_time` bigint(20) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.saved_sessions
CREATE TABLE IF NOT EXISTS `saved_sessions` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `ip_address` varchar(32) NOT NULL,
  `account_id` bigint(20) NOT NULL,
  `created_timestamp` bigint(20) NOT NULL DEFAULT 0,
  `serial` text NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6209 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.scooter_rental_shops
CREATE TABLE IF NOT EXISTS `scooter_rental_shops` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `ped_x` float NOT NULL,
  `ped_y` float NOT NULL,
  `ped_z` float NOT NULL,
  `ped_heading` float NOT NULL,
  `ped_dimension` int(10) unsigned NOT NULL,
  `spawn_x` float NOT NULL,
  `spawn_y` float NOT NULL,
  `spawn_z` float NOT NULL,
  `spawn_heading` float NOT NULL,
  `spawn_dimension` int(10) unsigned NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.stores
CREATE TABLE IF NOT EXISTS `stores` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `x` float NOT NULL,
  `y` float NOT NULL,
  `z` float NOT NULL,
  `rz` float NOT NULL,
  `type` int(10) unsigned NOT NULL,
  `dimension` int(11) unsigned NOT NULL,
  `parent_property` int(10) NOT NULL DEFAULT -1,
  `last_robbed_at` bigint(20) NOT NULL DEFAULT 0,
  KEY `id` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=290 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.teleport_places
CREATE TABLE IF NOT EXISTS `teleport_places` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(50) DEFAULT NULL,
  `x` float NOT NULL,
  `y` float NOT NULL,
  `z` float NOT NULL,
  `dimension` int(10) unsigned NOT NULL,
  `admin_creator_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `name` (`name`)
) ENGINE=InnoDB AUTO_INCREMENT=219 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.terminal_crimes
CREATE TABLE IF NOT EXISTS `terminal_crimes` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `type` varchar(255) CHARACTER SET latin1 NOT NULL,
  `crime` varchar(255) CHARACTER SET latin1 NOT NULL,
  `sentence` varchar(255) CHARACTER SET latin1 NOT NULL,
  `address` varchar(255) CHARACTER SET latin1 NOT NULL,
  `region` varchar(255) CHARACTER SET latin1 NOT NULL,
  `character_id` bigint(11) NOT NULL,
  `created_by_id` bigint(11) DEFAULT NULL,
  `created_at` datetime DEFAULT current_timestamp(),
  PRIMARY KEY (`id`),
  KEY `character_id` (`character_id`),
  KEY `created_by_id` (`created_by_id`),
  CONSTRAINT `terminal_crimes_ibfk_1` FOREIGN KEY (`character_id`) REFERENCES `characters` (`id`),
  CONSTRAINT `terminal_crimes_ibfk_2` FOREIGN KEY (`created_by_id`) REFERENCES `characters` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=103 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.terminal_crime_officers_involved
CREATE TABLE IF NOT EXISTS `terminal_crime_officers_involved` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `character_id` bigint(11) NOT NULL,
  `crime_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `character_id` (`character_id`),
  KEY `crime_id` (`crime_id`),
  CONSTRAINT `terminal_crime_officers_involved_ibfk_1` FOREIGN KEY (`character_id`) REFERENCES `characters` (`id`),
  CONSTRAINT `terminal_crime_officers_involved_ibfk_2` FOREIGN KEY (`crime_id`) REFERENCES `terminal_crimes` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.

-- Dumping structure for table gtav.terminal_logs
CREATE TABLE IF NOT EXISTS `terminal_logs` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `log` varchar(255) NOT NULL,
  `fields_changed` longtext DEFAULT NULL,
  `created_at` datetime(6) NOT NULL,
  `character_id` bigint(11) DEFAULT NULL,
  `created_by_id` bigint(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `character_id` (`character_id`),
  KEY `created_by_id` (`created_by_id`),
  CONSTRAINT `terminal_logs_ibfk_1` FOREIGN KEY (`character_id`) REFERENCES `characters` (`id`),
  CONSTRAINT `terminal_logs_ibfk_2` FOREIGN KEY (`created_by_id`) REFERENCES `characters` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Data exporting was unselected.

-- Dumping structure for table gtav.terminal_notes
CREATE TABLE IF NOT EXISTS `terminal_notes` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `note` longtext NOT NULL,
  `created_at` datetime(6) NOT NULL,
  `character_id` bigint(11) NOT NULL,
  `created_by_id` bigint(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `character_id` (`character_id`),
  KEY `created_by_id` (`created_by_id`),
  CONSTRAINT `terminal_notes_ibfk_1` FOREIGN KEY (`character_id`) REFERENCES `characters` (`id`),
  CONSTRAINT `terminal_notes_ibfk_2` FOREIGN KEY (`created_by_id`) REFERENCES `characters` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=latin1;

-- Data exporting was unselected.

-- Dumping structure for table gtav.terminal_person_details
CREATE TABLE IF NOT EXISTS `terminal_person_details` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `phone` varchar(20) DEFAULT NULL,
  `incarceration_status` varchar(255) DEFAULT NULL,
  `parole_status` varchar(255) DEFAULT NULL,
  `wanted_status` varchar(255) DEFAULT NULL,
  `violent_status` varchar(255) DEFAULT NULL,
  `bail_status` varchar(255) DEFAULT NULL,
  `race` varchar(255) DEFAULT NULL,
  `sex` varchar(255) DEFAULT NULL,
  `eye_color` varchar(255) DEFAULT NULL,
  `hair` varchar(255) DEFAULT NULL,
  `height` varchar(255) DEFAULT NULL,
  `weight` varchar(255) DEFAULT NULL,
  `character_id` bigint(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `character_id` (`character_id`),
  CONSTRAINT `terminal_person_details_ibfk_1` FOREIGN KEY (`character_id`) REFERENCES `characters` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=139 DEFAULT CHARSET=latin1;

-- Data exporting was unselected.

-- Dumping structure for table gtav.terminal_users
CREATE TABLE IF NOT EXISTS `terminal_users` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `admin` tinyint(1) NOT NULL,
  `character_id` bigint(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `character_id` (`character_id`),
  CONSTRAINT `terminal_users_ibfk_1` FOREIGN KEY (`character_id`) REFERENCES `characters` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=43 DEFAULT CHARSET=latin1;

-- Data exporting was unselected.

-- Dumping structure for table gtav.tutorial_state
CREATE TABLE IF NOT EXISTS `tutorial_state` (
  `account_id` int(10) NOT NULL,
  `VERSION` int(4) DEFAULT -1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.vehiclerepair_points
CREATE TABLE IF NOT EXISTS `vehiclerepair_points` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `x` float NOT NULL,
  `y` float NOT NULL,
  `z` float NOT NULL,
  `dimension` int(11) unsigned NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.vehicles
CREATE TABLE IF NOT EXISTS `vehicles` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `type` int(1) NOT NULL,
  `owner` bigint(20) NOT NULL,
  `model` int(10) unsigned NOT NULL,
  `spawn_x` float NOT NULL DEFAULT 0,
  `spawn_y` float NOT NULL DEFAULT 0,
  `spawn_z` float NOT NULL DEFAULT 0,
  `spawn_rx` float NOT NULL DEFAULT 0,
  `spawn_ry` float NOT NULL DEFAULT 0,
  `spawn_rz` float NOT NULL DEFAULT 0,
  `plate_type` int(1) NOT NULL DEFAULT 0,
  `plate_text` varchar(12) NOT NULL,
  `fuel` float NOT NULL DEFAULT 100,
  `color1_r` int(3) NOT NULL DEFAULT 0,
  `color1_g` int(3) NOT NULL DEFAULT 0,
  `color1_b` int(3) NOT NULL DEFAULT 0,
  `color2_r` int(3) NOT NULL DEFAULT 0,
  `color2_g` int(3) NOT NULL DEFAULT 0,
  `color2_b` int(3) NOT NULL DEFAULT 0,
  `color_wheel` int(3) unsigned NOT NULL DEFAULT 0,
  `livery` int(2) unsigned NOT NULL DEFAULT 0,
  `dirt` float NOT NULL DEFAULT 0,
  `health` float NOT NULL DEFAULT 1000,
  `x` float NOT NULL DEFAULT 0,
  `y` float NOT NULL DEFAULT 0,
  `z` float NOT NULL DEFAULT 0,
  `rx` float NOT NULL DEFAULT 0,
  `ry` float NOT NULL DEFAULT 0,
  `rz` float NOT NULL DEFAULT 0,
  `locked` tinyint(1) NOT NULL DEFAULT 1,
  `engine` tinyint(1) NOT NULL DEFAULT 0,
  `payments_remaining` int(3) unsigned NOT NULL DEFAULT 0,
  `payments_made` int(3) unsigned NOT NULL DEFAULT 0,
  `payments_missed` int(3) unsigned NOT NULL DEFAULT 0,
  `expiry_time` bigint(20) NOT NULL DEFAULT 0,
  `odometer` float NOT NULL DEFAULT 0,
  `dimension` int(11) unsigned DEFAULT 0,
  `towed` tinyint(1) DEFAULT 0,
  `credit_amount` float NOT NULL DEFAULT 0,
  `neons` tinyint(1) NOT NULL DEFAULT 0,
  `neon_r` int(3) unsigned DEFAULT 0,
  `neon_g` int(3) unsigned DEFAULT 0,
  `neon_b` int(3) unsigned DEFAULT 0,
  `last_used` bigint(20) NOT NULL DEFAULT 0,
  `radio` bigint(20) DEFAULT -1,
  `show_plate` tinyint(1) DEFAULT 1,
  `stolen` tinyint(1) DEFAULT 0,
  `transmission` int(1) NOT NULL DEFAULT 0,
  `is_token_purchase` tinyint(1) NOT NULL DEFAULT 0,
  `pearlescent_color` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`),
  KEY `owner_vehicles` (`owner`)
) ENGINE=InnoDB AUTO_INCREMENT=4189 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.vehicle_extras
CREATE TABLE IF NOT EXISTS `vehicle_extras` (
  `vehicle_id` int(10) unsigned NOT NULL,
  `extra` int(2) unsigned NOT NULL,
  `enabled` boolean NOT NULL DEFAULT TRUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.vehicle_mods
CREATE TABLE IF NOT EXISTS `vehicle_mods` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `vehicle` bigint(20) NOT NULL,
  `category` int(3) NOT NULL,
  `mod_index` int(3) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5256 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.vehicle_notes
CREATE TABLE IF NOT EXISTS `vehicle_notes` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `vehicle_id` bigint(20) NOT NULL,
  `creator_id` int(11) NOT NULL,
  `note` varchar(255) NOT NULL,
  `created_at` datetime DEFAULT current_timestamp(),
  PRIMARY KEY (`id`),
  KEY `vehicle_notes_vehicle_id_created_at_index` (`vehicle_id`,`created_at`),
  CONSTRAINT `vehicle_notes_vehicle_id_foreign` FOREIGN KEY (`vehicle_id`) REFERENCES `vehicles` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

-- Dumping structure for table gtav.world_blips
CREATE TABLE IF NOT EXISTS `world_blips` (
  `id` bigint(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(32) NOT NULL,
  `sprite` int(4) NOT NULL,
  `color` int(2) NOT NULL,
  `x` float NOT NULL,
  `y` float NOT NULL,
  `z` float NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=34 DEFAULT CHARSET=utf8mb4;

-- Data exporting was unselected.

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
