-- Kadın Kullanıcılar İçin Eşleşme Fonksiyonu
-- Bu fonksiyon kadın kullanıcılar için erkek eşleşmeleri bulur

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
        ON u."Id" = m."MaleUser" AND m."FemaleUser" = userId
    WHERE 
        u."LockoutEnabled" = FALSE 
        AND u."Gender" = TRUE -- Erkek kullanıcı
        AND u."PreferredGender" = FALSE -- Kadın tercih eden
        AND u."Id" != userId -- Kendi kullanıcı hariç
        AND (m."CreateDate" + INTERVAL '1 day' < NOW() OR m."CreateDate" IS NULL) -- 1 gün cooldown
        AND (u."FromAge" <= myage AND u."UntilAge" >= myage) -- Yaş aralığı kontrolü
        AND (fromDate >= u."BirthDate" AND untilDate <= u."BirthDate") -- Doğum tarihi kontrolü
        AND (m."FemaleUser" != userId OR m."FemaleUser" IS NULL) -- Daha önce eşleşmemiş
        AND (point(u."Longitude", u."Latitude") <@> point(longitude, latitude)) * 1.609344 < distanceLimit -- Mesafe kontrolü
    ORDER BY u."LastLoginDate" DESC 
    FETCH FIRST 1 ROWS ONLY;
END
$$ LANGUAGE plpgsql;

-- Test sorgusu
-- SELECT * FROM get_female_match(5, 40.981000, 28.779258, 21, 100, '2000-07-18 00:00:00', '1980-07-18 00:00:00');