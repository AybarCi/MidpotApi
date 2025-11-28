-- Seed Data for Event System (Post Schema)
-- Run this SQL in pgAdmin or any PostgreSQL client

START TRANSACTION;

-- Insert sample interests
INSERT INTO post."Interests" ("Id", "Name") VALUES 
('11111111-1111-1111-1111-111111111111', 'Board Games'),
('22222222-2222-2222-2222-222222222222', 'Hiking'),
('33333333-3333-3333-3333-333333333333', 'Coffee'),
('44444444-4444-4444-4444-444444444444', 'Sports'),
('55555555-5555-5555-5555-555555555555', 'Music'),
('66666666-6666-6666-6666-666666666666', 'Art'),
('77777777-7777-7777-7777-777777777777', 'Food'),
('88888888-8888-8888-8888-888888888888', 'Tech')
ON CONFLICT ("Id") DO NOTHING;

-- Insert sample credit products
INSERT INTO post."CreditProducts" ("Id", "Credits", "PriceCents") VALUES
('credits_5', 5, 499),
('credits_10', 10, 899),
('credits_20', 20, 1599)
ON CONFLICT ("Id") DO NOTHING;

COMMIT;
