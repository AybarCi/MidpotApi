#!/bin/bash
# GitHub Container Registry'dan image çekmek için authenticate ol ve deploy et

echo "GitHub Container Registry'e giriş yapılıyor..."
echo "GitHub personal access token'ını girmen gerekiyor:"
echo "Token oluşturmak için: GitHub Settings > Developer settings > Personal access tokens > Tokens (classic) > Generate new token"
echo "Scopes: read:packages (en azından)"
echo ""

# GitHub username'ı al
echo -n "GitHub username: "
read GITHUB_USER

# Token'ı gizli şekilde al
echo -n "GitHub personal access token: "
read -s GITHUB_TOKEN
echo ""

# GHCR'a login ol
echo $GITHUB_TOKEN | docker login ghcr.io -u $GITHUB_USER --password-stdin

if [ $? -eq 0 ]; then
    echo "✅ GitHub Container Registry'e başarıyla giriş yapıldı"
    
    # Stack'i deploy et
    echo "🚀 Stack deploy ediliyor..."
    docker-compose -f docker-compose.prod.yml up -d
    
    if [ $? -eq 0 ]; then
        echo "✅ Stack başarıyla deploy edildi!"
        echo "Servisleri kontrol etmek için: docker-compose -f docker-compose.prod.yml ps"
    else
        echo "❌ Stack deploy edilirken hata oluştu"
        exit 1
    fi
else
    echo "❌ GitHub Container Registry'e giriş yapılamadı"
    exit 1
fi