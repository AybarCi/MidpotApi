-- Tüm Matching Fonksiyonlarını Sırayla Çalıştırma Scripti
-- Bu script tüm matching fonksiyonlarını sırayla yükler

-- 1. Gerekli uzantıları yükle
\i 01_Extensions.sql

-- 2. Erkek-kadın eşleşme fonksiyonu
\i 02_GetMaleMatch.sql

-- 3. Kadın-erkek eşleşme fonksiyonu  
\i 03_GetFemaleMatch.sql

-- 4. Eşcinsel eşleşme fonksiyonu
\i 04_GetEcMatch.sql

-- 5. Test sorgularını çalıştır (isteğe bağlı)
-- \i 05_TestQueries.sql

-- Başarı mesajı
SELECT 'Tüm matching fonksiyonları başarıyla yüklendi!' AS Status;