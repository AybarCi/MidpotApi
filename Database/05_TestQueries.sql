-- Test Sorguları ve Örnek Kullanımlar

-- 1. Erkek kullanıcı testi
SELECT 'Erkek Kullanıcı Testi' AS TestType;
SELECT * FROM get_male_match(17, 40.981000, 28.779258, 28, 100, '2000-07-18 00:00:00', '1980-07-18 00:00:00');

-- 2. Kadın kullanıcı testi  
SELECT 'Kadın Kullanıcı Testi' AS TestType;
SELECT * FROM get_female_match(5, 40.981000, 28.779258, 21, 100, '2000-07-18 00:00:00', '1980-07-18 00:00:00');

-- 3. Eşcinsel testleri
SELECT 'Eşcinsel Kadın-Kadın Testi' AS TestType;
SELECT * FROM get_ec_match(5, false, 40.981000, 28.779258, 18, 100, '2000-07-18 00:00:00', '1980-07-18 00:00:00');

SELECT 'Eşcinsel Erkek-Erkek Testi' AS TestType;
SELECT * FROM get_ec_match(5, true, 40.981000, 28.779258, 18, 100, '2000-07-18 00:00:00', '1980-07-18 00:00:00');

-- 4. Fonksiyonları kontrol et
SELECT 'Mevcut Fonksiyonlar' AS Info;
SELECT proname, proargnames, prosrc 
FROM pg_proc 
WHERE proname IN ('get_male_match', 'get_female_match', 'get_ec_match');

-- 5. Parametre detayları
SELECT 'get_male_match Parametreleri' AS FunctionName;
SELECT 
    proname as function_name,
    pg_get_function_identity_arguments(oid) as parameters,
    pg_get_function_result(oid) as return_type
FROM pg_proc 
WHERE proname = 'get_male_match';

SELECT 'get_female_match Parametreleri' AS FunctionName;
SELECT 
    proname as function_name,
    pg_get_function_identity_arguments(oid) as parameters,
    pg_get_function_result(oid) as return_type
FROM pg_proc 
WHERE proname = 'get_female_match';

SELECT 'get_ec_match Parametreleri' AS FunctionName;
SELECT 
    proname as function_name,
    pg_get_function_identity_arguments(oid) as parameters,
    pg_get_function_result(oid) as return_type
FROM pg_proc 
WHERE proname = 'get_ec_match';