-- PostgreSQL Gerekli Uzantılar
-- Bu script matching fonksiyonları için gerekli uzantıları yükler

-- Eğer daha önce oluşturulmadıysa cube uzantısını yükle
CREATE EXTENSION IF NOT EXISTS "cube";

-- Eğer daha önce oluşturulmadıysa earthdistance uzantısını yükle  
CREATE EXTENSION IF NOT EXISTS "earthdistance";

-- Test için
SELECT 'Extensions loaded successfully' AS Status;