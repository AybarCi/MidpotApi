-- Eşcinsel Kullanıcılar İçin Eşleşme Fonksiyonu
-- Bu fonksiyon eşcinsel kullanıcılar için aynı cinsiyetten eşleşmeleri bulur

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
    Job TEXT
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
        u."Job"
    FROM "identity"."AspNetUsers" u
    LEFT JOIN "post"."Match" m 
        ON (u."Id" = m."MaleUser" OR u."Id" = m."FemaleUser") 
        AND (m."FemaleUser" = userId OR m."MaleUser" = userId)
    WHERE 
        u."LockoutEnabled" = FALSE 
        AND u."Gender" = gender -- Aynı cinsiyet
        AND u."PreferredGender" = gender -- Aynı cinsiyeti tercih eden
        AND u."Id" != userId -- Kendi kullanıcı hariç
        AND (m."CreateDate" + INTERVAL '1 day' < NOW() OR m."CreateDate" IS NULL) -- 1 gün cooldown
        AND (u."FromAge" <= myage AND u."UntilAge" >= myage) -- Yaş aralığı kontrolü
        AND (fromDate >= u."BirthDate" AND untilDate <= u."BirthDate") -- Doğum tarihi kontrolü
        AND (m."MaleUser" != userId OR m."MaleUser" IS NULL) -- Daha önce eşleşmemiş
        AND (m."FemaleUser" != userId OR m."FemaleUser" IS NULL) -- Daha önce eşleşmemiş
        AND (point(u."Longitude", u."Latitude") <@> point(longitude, latitude)) * 1.609344 < distanceLimit -- Mesafe kontrolü
    ORDER BY u."LastLoginDate" DESC 
    FETCH FIRST 1 ROWS ONLY;
END
$$ LANGUAGE plpgsql;

-- Test sorguları
-- SELECT * FROM get_ec_match(5, false, 40.981000, 28.779258, 18, 100, '2000-07-18 00:00:00', '1980-07-18 00:00:00'); -- Kadın-kadın
-- SELECT * FROM get_ec_match(5, true, 40.981000, 28.779258, 18, 100, '2000-07-18 00:00:00', '1980-07-18 00:00:00'); -- Erkek-erkek