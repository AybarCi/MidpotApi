-- Erkek Kullanıcılar İçin Eşleşme Fonksiyonu
-- Bu fonksiyon erkek kullanıcılar için kadın eşleşmeleri bulur

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
        ON u."Id" = m."FemaleUser" AND m."MaleUser" = userId
    WHERE 
        u."LockoutEnabled" = FALSE 
        AND u."Gender" = FALSE -- Kadın kullanıcı
        AND u."PreferredGender" = TRUE -- Erkek tercih eden
        AND u."Id" != userId -- Kendi kullanıcı hariç
        AND (m."CreateDate" + INTERVAL '1 day' < NOW() OR m."CreateDate" IS NULL) -- 1 gün cooldown
        AND (u."FromAge" <= myage AND u."UntilAge" >= myage) -- Yaş aralığı kontrolü
        AND (fromDate >= u."BirthDate" AND untilDate <= u."BirthDate") -- Doğum tarihi kontrolü
        AND (m."MaleUser" != userId OR m."MaleUser" IS NULL) -- Daha önce eşleşmemiş
        AND (point(u."Longitude", u."Latitude") <@> point(longitude, latitude)) * 1.609344 < distanceLimit -- Mesafe kontrolü
    ORDER BY u."LastLoginDate" DESC 
    FETCH FIRST 1 ROWS ONLY;
END
$$ LANGUAGE plpgsql;

-- Test sorgusu
-- SELECT * FROM get_male_match(17, 40.981000, 28.779258, 28, 100, '2000-07-18 00:00:00', '1980-07-18 00:00:00');