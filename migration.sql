START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    DROP TABLE post."Conversation";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    ALTER TABLE post."Message" DROP COLUMN "ConversationId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
        IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'event') THEN
            CREATE SCHEMA event;
        END IF;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    ALTER TABLE post."PremiumUser" ALTER COLUMN "PurchaseDate" TYPE timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    ALTER TABLE post."PremiumUser" ALTER COLUMN "ExpiresDate" TYPE timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    ALTER TABLE post."Message" ALTER COLUMN "CreateDate" TYPE timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    ALTER TABLE post."Message" ADD "IsRead" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    ALTER TABLE post."Message" ADD "MatchId" bigint NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    ALTER TABLE post."Match" ALTER COLUMN "CreateDate" TYPE timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    ALTER TABLE post."Match" ADD "UpdateDate" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    UPDATE identity."AspNetUserTokens" SET "Name" = '' WHERE "Name" IS NULL;
    ALTER TABLE identity."AspNetUserTokens" ALTER COLUMN "Name" SET NOT NULL;
    ALTER TABLE identity."AspNetUserTokens" ALTER COLUMN "Name" SET DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    UPDATE identity."AspNetUserTokens" SET "LoginProvider" = '' WHERE "LoginProvider" IS NULL;
    ALTER TABLE identity."AspNetUserTokens" ALTER COLUMN "LoginProvider" SET NOT NULL;
    ALTER TABLE identity."AspNetUserTokens" ALTER COLUMN "LoginProvider" SET DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    ALTER TABLE identity."AspNetUsers" ALTER COLUMN "LastLoginDate" TYPE timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    ALTER TABLE identity."AspNetUsers" ALTER COLUMN "Description" TYPE character varying(1000);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    ALTER TABLE identity."AspNetUsers" ALTER COLUMN "BirthDate" TYPE timestamp with time zone;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    ALTER TABLE identity."AspNetUsers" ADD "CreateDate" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    ALTER TABLE identity."AspNetUsers" ADD "GhostMode" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    ALTER TABLE identity."AspNetUsers" ADD "IsDelete" boolean NOT NULL DEFAULT FALSE;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    ALTER TABLE identity."AspNetUsers" ADD "IsSuspendedUntil" timestamp with time zone NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    ALTER TABLE identity."AspNetUsers" ADD "Job" character varying(250) NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    ALTER TABLE identity."AspNetUsers" ADD "MissedEventCount" integer NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    ALTER TABLE identity."AspNetUsers" ADD "RefreshToken" text NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    ALTER TABLE identity."AspNetUsers" ADD "RefreshTokenExpireDate" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    ALTER TABLE identity."AspNetUsers" ADD "School" character varying(250) NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    UPDATE identity."AspNetUserLogins" SET "ProviderKey" = '' WHERE "ProviderKey" IS NULL;
    ALTER TABLE identity."AspNetUserLogins" ALTER COLUMN "ProviderKey" SET NOT NULL;
    ALTER TABLE identity."AspNetUserLogins" ALTER COLUMN "ProviderKey" SET DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    UPDATE identity."AspNetUserLogins" SET "LoginProvider" = '' WHERE "LoginProvider" IS NULL;
    ALTER TABLE identity."AspNetUserLogins" ALTER COLUMN "LoginProvider" SET NOT NULL;
    ALTER TABLE identity."AspNetUserLogins" ALTER COLUMN "LoginProvider" SET DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    CREATE TABLE event."CreditProducts" (
        "Id" text NOT NULL,
        "Credits" integer NOT NULL,
        "PriceCents" integer NOT NULL,
        CONSTRAINT "PK_CreditProducts" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    CREATE TABLE event."CreditTransactions" (
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
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    CREATE TABLE event."Interests" (
        "Id" uuid NOT NULL,
        "Name" text NOT NULL,
        CONSTRAINT "PK_Interests" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    CREATE TABLE identity."Locations" (
        "Id" bigint GENERATED BY DEFAULT AS IDENTITY,
        "Latitude" real NOT NULL,
        "Longitude" real NOT NULL,
        "City" text NULL,
        CONSTRAINT "PK_Locations" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    CREATE TABLE post."Privacy" (
        "Id" bigint GENERATED BY DEFAULT AS IDENTITY,
        "LanguageCode" text NULL,
        "ContentKey" text NULL,
        "Content" text NULL,
        CONSTRAINT "PK_Privacy" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    CREATE TABLE post."Report" (
        "ReportId" integer GENERATED BY DEFAULT AS IDENTITY,
        "Description" character varying(1000) NULL,
        "MatchId" bigint NOT NULL,
        "UserId" bigint NOT NULL,
        CONSTRAINT "PK_Report" PRIMARY KEY ("ReportId")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    CREATE TABLE post."Story" (
        "StoryId" bigint GENERATED BY DEFAULT AS IDENTITY,
        "UserId" bigint NOT NULL,
        "PhotoUrl" text NULL,
        "CreateDate" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_Story" PRIMARY KEY ("StoryId")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    CREATE TABLE event."Events" (
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
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    CREATE TABLE event."UserInterests" (
        "UserId" bigint NOT NULL,
        "InterestId" uuid NOT NULL,
        CONSTRAINT "PK_UserInterests" PRIMARY KEY ("UserId", "InterestId"),
        CONSTRAINT "FK_UserInterests_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES identity."AspNetUsers" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_UserInterests_Interests_InterestId" FOREIGN KEY ("InterestId") REFERENCES event."Interests" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    CREATE TABLE event."EventParticipants" (
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
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    CREATE TABLE event."MissedEventsHistory" (
        "Id" uuid NOT NULL,
        "UserId" bigint NOT NULL,
        "EventId" uuid NOT NULL,
        "OccurredAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_MissedEventsHistory" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_MissedEventsHistory_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES identity."AspNetUsers" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_MissedEventsHistory_Events_EventId" FOREIGN KEY ("EventId") REFERENCES event."Events" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    CREATE UNIQUE INDEX "IX_AspNetUsers_PhoneNumber" ON identity."AspNetUsers" ("PhoneNumber");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    CREATE INDEX "IX_CreditTransactions_UserId" ON event."CreditTransactions" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    CREATE INDEX "IX_EventParticipants_UserId" ON event."EventParticipants" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    CREATE INDEX "IX_Events_CreatorId" ON event."Events" ("CreatorId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    CREATE INDEX "IX_Events_InterestId" ON event."Events" ("InterestId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    CREATE UNIQUE INDEX "IX_Interests_Name" ON event."Interests" ("Name");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    CREATE INDEX "IX_MissedEventsHistory_EventId" ON event."MissedEventsHistory" ("EventId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    CREATE INDEX "IX_MissedEventsHistory_UserId" ON event."MissedEventsHistory" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    CREATE INDEX "IX_UserInterests_InterestId" ON event."UserInterests" ("InterestId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251127154953_AddEventSystem') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251127154953_AddEventSystem', '7.0.0');
    END IF;
END $EF$;
COMMIT;

