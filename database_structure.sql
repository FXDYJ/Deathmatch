CREATE DATABASE IF NOT EXISTS deathmatch;

USE deathmatch;

CREATE TABLE IF NOT EXISTS configs (
    UserId VARCHAR(255) PRIMARY KEY,
    primary INT,
    secondary INT,
    tertiary INT,
    rage_enabled BOOLEAN,
    role INT,
    killstreak_mode VARCHAR(255)
);

CREATE TABLE IF NOT EXISTS users (
    UserId VARCHAR(255) PRIMARY KEY,
    TrackingId BIGINT
);

CREATE TABLE IF NOT EXISTS ranks (
    UserId VARCHAR(255) PRIMARY KEY,
    state INT,
    placement_matches INT,
    rating FLOAT,
    rd FLOAT,
    rv FLOAT
);

CREATE TABLE IF NOT EXISTS experiences (
    UserId VARCHAR(255) PRIMARY KEY,
    value INT,
    level INT,
    stage INT,
    tier INT
);

CREATE TABLE IF NOT EXISTS leader_board (
    UserId VARCHAR(255) PRIMARY KEY,
    total_kills INT,
    highest_killstreak INT,
    killstreak_tag VARCHAR(255),
    total_play_time INT
);

CREATE TABLE IF NOT EXISTS hits (
    HitId BIGINT PRIMARY KEY,
    health TINYINT,
    damage TINYINT,
    hitbox TINYINT,
    weapon TINYINT
);

CREATE TABLE IF NOT EXISTS kills (
    KillId BIGINT PRIMARY KEY,
    time FLOAT,
    hitbox INT,
    weapon INT,
    attachment_code INT
);

CREATE TABLE IF NOT EXISTS loadouts (
    LoadoutId BIGINT PRIMARY KEY,
    killstreak_mode VARCHAR(255),
    primary INT,
    primary_attachment_code INT,
    secondary INT,
    secondary_attachment_code INT,
    tertiary INT,
    tertiary_attachment_code INT
);

CREATE TABLE IF NOT EXISTS lives (
    LifeId BIGINT PRIMARY KEY,
    role INT,
    shots INT,
    time FLOAT,
    loadout BIGINT
);

CREATE TABLE IF NOT EXISTS rounds (
    RoundId BIGINT PRIMARY KEY,
    start DATETIME,
    end DATETIME,
    max_players INT
);

CREATE TABLE IF NOT EXISTS sessions (
    SessionId BIGINT PRIMARY KEY,
    nickname VARCHAR(255),
    connect DATETIME,
    disconnect DATETIME,
    round BIGINT
);

CREATE TABLE IF NOT EXISTS tracking (
    TrackingId BIGINT PRIMARY KEY
);
