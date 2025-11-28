-- Step 1: Create migration history table if not exists and add initial migration
CREATE TABLE IF NOT EXISTS public."__EFMigrationsHistory" (
    "MigrationId" varchar(150) NOT NULL,
    "ProductVersion" varchar(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

-- Step 2: Mark initial migration as applied
INSERT INTO public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") 
VALUES ('20210518145755_initial', '7.0.0')
ON CONFLICT ("MigrationId") DO NOTHING;

-- Now only Event migration will be applied when you run: dotnet ef database update
