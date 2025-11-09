-- Midpot Dating App PostgreSQL Database Creation Script
-- Bu script eksiksiz olarak veritabanını oluşturur

-- 1. Database ve Schema Oluşturma
CREATE DATABASE socialdb;
\c socialdb;

-- Gerekli uzantıları yükle
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "cube";
CREATE EXTENSION IF NOT EXISTS "earthdistance";

-- Schema'ları oluştur
CREATE SCHEMA IF NOT EXISTS identity;
CREATE SCHEMA IF NOT EXISTS post;

-- 2. Identity Tabloları (ASP.NET Core Identity)

-- Roles Tablosu
CREATE TABLE identity."AspNetRoles" (
    "Id" BIGSERIAL PRIMARY KEY,
    "Name" VARCHAR(256) NULL,
    "NormalizedName" VARCHAR(256) NULL,
    "ConcurrencyStamp" TEXT NULL
);

-- Users Tablosu
CREATE TABLE identity."AspNetUsers" (
    "Id" BIGSERIAL PRIMARY KEY,
    "UserName" VARCHAR(256) NULL,
    "NormalizedUserName" VARCHAR(256) NULL,
    "Email" VARCHAR(256) NULL,
    "NormalizedEmail" VARCHAR(256) NULL,
    "EmailConfirmed" BOOLEAN NOT NULL DEFAULT FALSE,
    "PasswordHash" TEXT NULL,
    "SecurityStamp" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL,
    "PhoneNumber" VARCHAR(20) NULL UNIQUE,
    "PhoneNumberConfirmed" BOOLEAN NOT NULL DEFAULT FALSE,
    "TwoFactorEnabled" BOOLEAN NOT NULL DEFAULT FALSE,
    "LockoutEnd" TIMESTAMP WITH TIME ZONE NULL,
    "LockoutEnabled" BOOLEAN NOT NULL DEFAULT FALSE,
    "AccessFailedCount" INTEGER NOT NULL DEFAULT 0,
    "Gender" BOOLEAN NOT NULL,
    "PreferredGender" BOOLEAN NOT NULL,
    "BirthDate" TIMESTAMP NOT NULL,
    "ConfirmCode" VARCHAR(10) NULL,
    "PersonName" VARCHAR(100) NULL,
    "Description" VARCHAR(1000) NULL,
    "ProfilePhoto" TEXT NULL,
    "DeviceToken" TEXT NULL,
    "Platform" BOOLEAN NOT NULL DEFAULT FALSE,
    "Latitude" DOUBLE PRECISION NOT NULL DEFAULT 0,
    "Longitude" DOUBLE PRECISION NOT NULL DEFAULT 0,
    "LastLoginDate" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "FromAge" INTEGER NOT NULL DEFAULT 18,
    "UntilAge" INTEGER NOT NULL DEFAULT 99,
    "RefreshToken" TEXT NULL,
    "RefreshTokenExpireDate" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "School" VARCHAR(250) NULL,
    "Job" VARCHAR(250) NULL,
    "IsDelete" BOOLEAN NOT NULL DEFAULT FALSE,
    "CreateDate" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "GhostMode" BOOLEAN NOT NULL DEFAULT FALSE
);

-- User Claims Tablosu
CREATE TABLE identity."AspNetUserClaims" (
    "Id" BIGSERIAL PRIMARY KEY,
    "UserId" BIGINT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" 
        FOREIGN KEY ("UserId") 
        REFERENCES identity."AspNetUsers" ("Id") 
        ON DELETE CASCADE
);

-- User Logins Tablosu
CREATE TABLE identity."AspNetUserLogins" (
    "LoginProvider" TEXT NOT NULL,
    "ProviderKey" TEXT NOT NULL,
    "ProviderDisplayName" TEXT NULL,
    "UserId" BIGINT NOT NULL,
    PRIMARY KEY ("LoginProvider", "ProviderKey"),
    CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" 
        FOREIGN KEY ("UserId") 
        REFERENCES identity."AspNetUsers" ("Id") 
        ON DELETE CASCADE
);

-- User Tokens Tablosu
CREATE TABLE identity."AspNetUserTokens" (
    "UserId" BIGINT NOT NULL,
    "LoginProvider" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Value" TEXT NULL,
    PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" 
        FOREIGN KEY ("UserId") 
        REFERENCES identity."AspNetUsers" ("Id") 
        ON DELETE CASCADE
);

-- Role Claims Tablosu
CREATE TABLE identity."AspNetRoleClaims" (
    "Id" BIGSERIAL PRIMARY KEY,
    "RoleId" BIGINT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" 
        FOREIGN KEY ("RoleId") 
        REFERENCES identity."AspNetRoles" ("Id") 
        ON DELETE CASCADE
);

-- User Roles Tablosu
CREATE TABLE identity."AspNetUserRoles" (
    "UserId" BIGINT NOT NULL,
    "RoleId" BIGINT NOT NULL,
    PRIMARY KEY ("UserId", "RoleId"),
    CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" 
        FOREIGN KEY ("UserId") 
        REFERENCES identity."AspNetUsers" ("Id") 
        ON DELETE CASCADE,
    CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" 
        FOREIGN KEY ("RoleId") 
        REFERENCES identity."AspNetRoles" ("Id") 
        ON DELETE CASCADE
);

-- 3. Post Schema Tabloları

-- Match Tablosu
CREATE TABLE post."Match" (
    "MatchId" BIGSERIAL PRIMARY KEY,
    "MaleUser" BIGINT NOT NULL,
    "FemaleUser" BIGINT NOT NULL,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreateDate" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdateDate" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "FK_Match_MaleUser" 
        FOREIGN KEY ("MaleUser") 
        REFERENCES identity."AspNetUsers" ("Id"),
    CONSTRAINT "FK_Match_FemaleUser" 
        FOREIGN KEY ("FemaleUser") 
        REFERENCES identity."AspNetUsers" ("Id")
);

-- Message Tablosu
CREATE TABLE post."Message" (
    "MessageId" BIGSERIAL PRIMARY KEY,
    "MatchId" BIGINT NOT NULL,
    "UserId" BIGINT NOT NULL,
    "Chat" VARCHAR(144) NOT NULL,
    "CreateDate" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "IpAddress" INET NULL,
    "IsRead" BOOLEAN NOT NULL DEFAULT FALSE,
    CONSTRAINT "FK_Message_Match" 
        FOREIGN KEY ("MatchId") 
        REFERENCES post."Match" ("MatchId") 
        ON DELETE CASCADE,
    CONSTRAINT "FK_Message_User" 
        FOREIGN KEY ("UserId") 
        REFERENCES identity."AspNetUsers" ("Id")
);

-- Gallery Tablosu
CREATE TABLE post."Gallery" (
    "GalleryId" SERIAL PRIMARY KEY,
    "UserId" BIGINT NOT NULL,
    "Url" TEXT NOT NULL,
    "IsDelete" BOOLEAN NOT NULL DEFAULT FALSE,
    CONSTRAINT "FK_Gallery_User" 
        FOREIGN KEY ("UserId") 
        REFERENCES identity."AspNetUsers" ("Id") 
        ON DELETE CASCADE
);

-- PremiumUser Tablosu
CREATE TABLE post."PremiumUser" (
    "PremiumUserId" BIGSERIAL PRIMARY KEY,
    "UserId" BIGINT NOT NULL,
    "PurchaseDate" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "ExpiresDate" TIMESTAMP NOT NULL,
    "ProductId" VARCHAR(100) NOT NULL,
    "TransactionId" VARCHAR(100) NOT NULL,
    CONSTRAINT "FK_PremiumUser_User" 
        FOREIGN KEY ("UserId") 
        REFERENCES identity."AspNetUsers" ("Id") 
        ON DELETE CASCADE
);

-- Setting Tablosu
CREATE TABLE post."Setting" (
    "SettingId" SERIAL PRIMARY KEY,
    "Key" VARCHAR(100) NOT NULL,
    "Value" TEXT NOT NULL
);

-- Report Tablosu
CREATE TABLE post."Report" (
    "ReportId" SERIAL PRIMARY KEY,
    "Description" VARCHAR(1000) NOT NULL,
    "MatchId" BIGINT NOT NULL,
    "UserId" BIGINT NOT NULL,
    CONSTRAINT "FK_Report_Match" 
        FOREIGN KEY ("MatchId") 
        REFERENCES post."Match" ("MatchId") 
        ON DELETE CASCADE,
    CONSTRAINT "FK_Report_User" 
        FOREIGN KEY ("UserId") 
        REFERENCES identity."AspNetUsers" ("Id") 
        ON DELETE CASCADE
);

-- Story Tablosu
CREATE TABLE post."Story" (
    "StoryId" BIGSERIAL PRIMARY KEY,
    "UserId" BIGINT NOT NULL,
    "PhotoUrl" TEXT NOT NULL,
    "CreateDate" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "FK_Story_User" 
        FOREIGN KEY ("UserId") 
        REFERENCES identity."AspNetUsers" ("Id") 
        ON DELETE CASCADE
);

-- Privacy Tablosu
CREATE TABLE post."Privacy" (
    "Id" BIGSERIAL PRIMARY KEY,
    "LanguageCode" VARCHAR(10) NOT NULL,
    "ContentKey" VARCHAR(100) NOT NULL,
    "Content" TEXT NOT NULL
);

-- Location Tablosu
CREATE TABLE identity."Location" (
    "Id" BIGSERIAL PRIMARY KEY,
    "Latitude" REAL NOT NULL,
    "Longitude" REAL NOT NULL,
    "City" VARCHAR(100) NOT NULL
);

-- 4. Index'ler

-- Performance index'leri
CREATE INDEX "IX_AspNetUsers_PhoneNumber" ON identity."AspNetUsers" ("PhoneNumber");
CREATE INDEX "IX_AspNetUsers_Gender_PreferredGender" ON identity."AspNetUsers" ("Gender", "PreferredGender");
CREATE INDEX "IX_AspNetUsers_Latitude_Longitude" ON identity."AspNetUsers" ("Latitude", "Longitude");
CREATE INDEX "IX_AspNetUsers_LastLoginDate" ON identity."AspNetUsers" ("LastLoginDate" DESC);
CREATE INDEX "IX_AspNetUsers_BirthDate" ON identity."AspNetUsers" ("BirthDate");
CREATE INDEX "IX_AspNetUsers_LockoutEnabled" ON identity."AspNetUsers" ("LockoutEnabled");
CREATE INDEX "IX_AspNetUsers_IsDelete" ON identity."AspNetUsers" ("IsDelete");

CREATE INDEX "IX_Match_MaleUser" ON post."Match" ("MaleUser");
CREATE INDEX "IX_Match_FemaleUser" ON post."Match" ("FemaleUser");
CREATE INDEX "IX_Match_IsActive" ON post."Match" ("IsActive");
CREATE INDEX "IX_Match_CreateDate" ON post."Match" ("CreateDate" DESC);

CREATE INDEX "IX_Message_MatchId" ON post."Message" ("MatchId");
CREATE INDEX "IX_Message_UserId" ON post."Message" ("UserId");
CREATE INDEX "IX_Message_CreateDate" ON post."Message" ("CreateDate" DESC);

CREATE INDEX "IX_Gallery_UserId" ON post."Gallery" ("UserId");
CREATE INDEX "IX_Gallery_IsDelete" ON post."Gallery" ("IsDelete");

CREATE INDEX "IX_PremiumUser_UserId" ON post."PremiumUser" ("UserId");
CREATE INDEX "IX_PremiumUser_ExpiresDate" ON post."PremiumUser" ("ExpiresDate");

CREATE INDEX "IX_Story_UserId" ON post."Story" ("UserId");
CREATE INDEX "IX_Story_CreateDate" ON post."Story" ("CreateDate" DESC);

-- 5. Matching Stored Procedures

-- Erkek kullanıcılar için match stored procedure
CREATE OR REPLACE FUNCTION get_male_match(
    userId BIGINT,
    latitude DOUBLE PRECISION,
    longitude DOUBLE PRECISION,
    myage INTEGER,
    distanceLimit INTEGER,
    fromDate TIMESTAMP,
    untilDate TIMESTAMP
)
RETURNS TABLE (
    Id BIGINT,
    PersonName TEXT,
    BirthDate TIMESTAMP,
    ProfilePhoto TEXT,
    Description TEXT,
    School TEXT,
    Job TEXT,
    DeviceToken TEXT
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        u."Id", 
        u."PersonName", 
        u."BirthDate", 
        u."ProfilePhoto", 
        u."Description", 
        u."School", 
        u."Job",
        u."DeviceToken"
    FROM identity."AspNetUsers" u  
    LEFT JOIN post."Match" m 
        ON u."Id" = m."FemaleUser" AND m."MaleUser" = userId
    WHERE 
        u."LockoutEnabled" = FALSE 
        AND u."Gender" = FALSE 
        AND u."PreferredGender" = TRUE 
        AND u."Id" != userId 
        AND (m."CreateDate" + INTERVAL '1 day' < NOW() OR m."CreateDate" IS NULL) 
        AND (u."FromAge" <= myage AND u."UntilAge" >= myage) 
        AND (fromDate >= u."BirthDate" AND untilDate <= u."BirthDate") 
        AND (m."MaleUser" != userId OR m."MaleUser" IS NULL)
        AND (point(u."Longitude", u."Latitude") <@> point(longitude, latitude)) * 1.609344 < distanceLimit
    ORDER BY u."LastLoginDate" DESC 
    FETCH FIRST 1 ROWS ONLY;
END
$$ LANGUAGE plpgsql;

-- Kadın kullanıcılar için match stored procedure
CREATE OR REPLACE FUNCTION get_female_match(
    userId BIGINT,
    latitude DOUBLE PRECISION,
    longitude DOUBLE PRECISION,
    myage INTEGER,
    distanceLimit INTEGER,
    fromDate TIMESTAMP,
    untilDate TIMESTAMP
)
RETURNS TABLE (
    Id BIGINT,
    PersonName TEXT,
    BirthDate TIMESTAMP,
    ProfilePhoto TEXT,
    Description TEXT,
    School TEXT,
    Job TEXT,
    DeviceToken TEXT
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        u."Id", 
        u."PersonName", 
        u."BirthDate", 
        u."ProfilePhoto", 
        u."Description", 
        u."School", 
        u."Job",
        u."DeviceToken"
    FROM identity."AspNetUsers" u  
    LEFT JOIN post."Match" m 
        ON u."Id" = m."MaleUser" AND m."FemaleUser" = userId
    WHERE 
        u."LockoutEnabled" = FALSE 
        AND u."Gender" = TRUE 
        AND u."PreferredGender" = FALSE 
        AND u."Id" != userId 
        AND (m."CreateDate" + INTERVAL '1 day' < NOW() OR m."CreateDate" IS NULL) 
        AND (u."FromAge" <= myage AND u."UntilAge" >= myage) 
        AND (fromDate >= u."BirthDate" AND untilDate <= u."BirthDate") 
        AND (m."FemaleUser" != userId OR m."FemaleUser" IS NULL)
        AND (point(u."Longitude", u."Latitude") <@> point(longitude, latitude)) * 1.609344 < distanceLimit
    ORDER BY u."LastLoginDate" DESC 
    FETCH FIRST 1 ROWS ONLY;
END
$$ LANGUAGE plpgsql;

-- Eşcinsel match stored procedure
CREATE OR REPLACE FUNCTION get_ec_match(
    userId BIGINT,
    gender BOOLEAN,
    latitude DOUBLE PRECISION,
    longitude DOUBLE PRECISION,
    myage INTEGER,
    distanceLimit INTEGER,
    fromDate TIMESTAMP,
    untilDate TIMESTAMP
)
RETURNS TABLE (
    Id BIGINT,
    PersonName TEXT,
    BirthDate TIMESTAMP,
    ProfilePhoto TEXT,
    Description TEXT,
    School TEXT,
    Job TEXT,
    DeviceToken TEXT
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        u."Id", 
        u."PersonName", 
        u."BirthDate", 
        u."ProfilePhoto", 
        u."Description", 
        u."School", 
        u."Job",
        u."DeviceToken"
    FROM identity."AspNetUsers" u  
    LEFT JOIN post."Match" m 
        ON (u."Id" = m."MaleUser" OR u."Id" = m."FemaleUser") 
        AND (m."FemaleUser" = userId OR m."MaleUser" = userId)  
    WHERE 
        u."LockoutEnabled" = FALSE 
        AND u."Gender" = gender 
        AND u."PreferredGender" = gender 
        AND u."Id" != userId 
        AND (m."CreateDate" + INTERVAL '1 day' < NOW() OR m."CreateDate" IS NULL) 
        AND (u."FromAge" <= myage AND u."UntilAge" >= myage) 
        AND (fromDate >= u."BirthDate" AND untilDate <= u."BirthDate") 
        AND (m."MaleUser" != userId OR m."MaleUser" IS NULL)
        AND (m."FemaleUser" != userId OR m."FemaleUser" IS NULL)
        AND (point(u."Longitude", u."Latitude") <@> point(longitude, latitude)) * 1.609344 < distanceLimit
    ORDER BY u."LastLoginDate" DESC 
    FETCH FIRST 1 ROWS ONLY;
END
$$ LANGUAGE plpgsql;

-- 6. Default Veriler

-- Dil ayarları için default veriler
INSERT INTO post."Privacy" ("LanguageCode", "ContentKey", "Content") VALUES
('tr', 'privacy_policy', 'Gizlilik Politikası içeriği'),
('tr', 'user_agreement', 'Kullanıcı Sözleşmesi içeriği'),
('en', 'privacy_policy', 'Privacy Policy content'),
('en', 'user_agreement', 'User Agreement content');

-- Sistem ayarları
INSERT INTO post."Setting" ("Key", "Value") VALUES
('DistanceLimit', '100'),
('MatchCooldownHours', '24'),
('MaxDailyMatches', '10'),
('MessageMaxLength', '144'),
('StoryExpirationHours', '24');

-- 7. Kullanım Talimatları

-- Scripti çalıştırmak için:
-- psql -U postgres -d postgres -f CreateDatabase.sql

-- Veya tek tek:
-- psql -U postgres
-- CREATE DATABASE socialdb;
-- \c socialdb
-- -- Sonra scripti çalıştır

-- Test sorguları:
-- SELECT COUNT(*) FROM identity."AspNetUsers";
-- SELECT * FROM post."Match" LIMIT 5;
-- SELECT * FROM get_male_match(1, 40.981000, 28.779258, 25, 100, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);