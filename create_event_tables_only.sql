-- Event System Tables Only
-- Run this SQL in pgAdmin or any PostgreSQL client

START TRANSACTION;

-- Create event schema
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'event') THEN
        CREATE SCHEMA event;
    END IF;
END $EF$;

-- Add Event-related columns to AspNetUsers
ALTER TABLE identity."AspNetUsers" ADD COLUMN IF NOT EXISTS "IsSuspendedUntil" timestamp with time zone NULL;
ALTER TABLE identity."AspNetUsers" ADD COLUMN IF NOT EXISTS "MissedEventCount" integer NOT NULL DEFAULT 0;

-- Create Interests table
CREATE TABLE IF NOT EXISTS event."Interests" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    CONSTRAINT "PK_Interests" PRIMARY KEY ("Id")
);

-- Create Events table
CREATE TABLE IF NOT EXISTS event."Events" (
    "Id" uuid NOT NULL,
    "CreatorId" bigint NULL,
    "Title" text NOT NULL,
    "Description" text NULL,
    "InterestId" uuid NOT NULL,
    "PlaceId" text NULL,
    "PlaceName" text NULL,
    "PlaceAddress" text NULL,
    "Lat" double precision NOT NULL,
    "Lng" double precision NOT NULL,
    "StartsAt" timestamp with time zone NOT NULL,
    "EndsAt" timestamp with time zone NOT NULL,
    "Capacity" integer NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "Status" text NOT NULL,
    "CreditsSpent" integer NOT NULL,
    CONSTRAINT "PK_Events" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Events_AspNetUsers_CreatorId" FOREIGN KEY ("CreatorId") REFERENCES identity."AspNetUsers" ("Id"),
    CONSTRAINT "FK_Events_Interests_InterestId" FOREIGN KEY ("InterestId") REFERENCES event."Interests" ("Id") ON DELETE CASCADE
);

-- Create UserInterests table
CREATE TABLE IF NOT EXISTS event."UserInterests" (
    "UserId" bigint NOT NULL,
    "InterestId" uuid NOT NULL,
    CONSTRAINT "PK_UserInterests" PRIMARY KEY ("UserId", "InterestId"),
    CONSTRAINT "FK_UserInterests_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES identity."AspNetUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_UserInterests_Interests_InterestId" FOREIGN KEY ("InterestId") REFERENCES event."Interests" ("Id") ON DELETE CASCADE
);

-- Create EventParticipants table
CREATE TABLE IF NOT EXISTS event."EventParticipants" (
    "EventId" uuid NOT NULL,
    "UserId" bigint NOT NULL,
    "JoinedAt" timestamp with time zone NOT NULL,
    "Status" text NOT NULL,
    "RatingByCreator" integer NULL,
    "RatingByUser" integer NULL,
    "CheckedIn" boolean NOT NULL,
    CONSTRAINT "PK_EventParticipants" PRIMARY KEY ("EventId", "UserId"),
    CONSTRAINT "FK_EventParticipants_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES identity."AspNetUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_EventParticipants_Events_EventId" FOREIGN KEY ("EventId") REFERENCES event."Events" ("Id") ON DELETE CASCADE
);

-- Create CreditTransactions table
CREATE TABLE IF NOT EXISTS event."CreditTransactions" (
    "Id" uuid NOT NULL,
    "UserId" bigint NOT NULL,
    "Change" integer NOT NULL,
    "BalanceAfter" integer NOT NULL,
    "Type" text NOT NULL,
    "Metadata" text NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_CreditTransactions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_CreditTransactions_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES identity."AspNetUsers" ("Id") ON DELETE CASCADE
);

-- Create CreditProducts table
CREATE TABLE IF NOT EXISTS event."CreditProducts" (
    "Id" text NOT NULL,
    "Credits" integer NOT NULL,
    "PriceCents" integer NOT NULL,
    CONSTRAINT "PK_CreditProducts" PRIMARY KEY ("Id")
);

-- Create MissedEventsHistory table
CREATE TABLE IF NOT EXISTS event."MissedEventsHistory" (
    "Id" uuid NOT NULL,
    "UserId" bigint NOT NULL,
    "EventId" uuid NOT NULL,
    "OccurredAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_MissedEventsHistory" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_MissedEventsHistory_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES identity."AspNetUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_MissedEventsHistory_Events_EventId" FOREIGN KEY ("EventId") REFERENCES event."Events" ("Id") ON DELETE CASCADE
);

-- Create indexes
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Interests_Name" ON event."Interests" ("Name");
CREATE INDEX IF NOT EXISTS "IX_Events_CreatorId" ON event."Events" ("CreatorId");
CREATE INDEX IF NOT EXISTS "IX_Events_InterestId" ON event."Events" ("InterestId");
CREATE INDEX IF NOT EXISTS "IX_UserInterests_InterestId" ON event."UserInterests" ("InterestId");
CREATE INDEX IF NOT EXISTS "IX_EventParticipants_UserId" ON event."EventParticipants" ("UserId");
CREATE INDEX IF NOT EXISTS "IX_CreditTransactions_UserId" ON event."CreditTransactions" ("UserId");
CREATE INDEX IF NOT EXISTS "IX_MissedEventsHistory_UserId" ON event."MissedEventsHistory" ("UserId");
CREATE INDEX IF NOT EXISTS "IX_MissedEventsHistory_EventId" ON event."MissedEventsHistory" ("EventId");

-- Insert sample interests (optional)
INSERT INTO event."Interests" ("Id", "Name") VALUES 
('11111111-1111-1111-1111-111111111111', 'Board Games'),
('22222222-2222-2222-2222-222222222222', 'Hiking'),
('33333333-3333-3333-3333-333333333333', 'Coffee'),
('44444444-4444-4444-4444-444444444444', 'Sports'),
('55555555-5555-5555-5555-555555555555', 'Music'),
('66666666-6666-6666-6666-666666666666', 'Art'),
('77777777-7777-7777-7777-777777777777', 'Food'),
('88888888-8888-8888-8888-888888888888', 'Tech')
ON CONFLICT ("Id") DO NOTHING;

-- Insert sample credit products (optional)
INSERT INTO event."CreditProducts" ("Id", "Credits", "PriceCents") VALUES
('credits_5', 5, 499),
('credits_10', 10, 899),
('credits_20', 20, 1599)
ON CONFLICT ("Id") DO NOTHING;

-- Mark migration as applied
CREATE TABLE IF NOT EXISTS public."__EFMigrationsHistory" (
    "MigrationId" varchar(150) NOT NULL,
    "ProductVersion" varchar(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

INSERT INTO public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") 
VALUES ('20210518145755_initial', '7.0.0')
ON CONFLICT DO NOTHING;

INSERT INTO public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") 
VALUES ('20251127154953_AddEventSystem', '7.0.0')
ON CONFLICT DO NOTHING;

COMMIT;
