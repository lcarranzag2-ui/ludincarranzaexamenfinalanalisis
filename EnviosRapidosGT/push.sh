#!/bin/bash
# Script para inicializar el repo y subir todo a GitHub
# Ejecutar desde la carpeta raiz del proyecto

set -e

REPO_URL="https://github.com/lcarranzag2-ui/ludincarranzaexamenfinalanalisis.git"

echo "Inicializando repositorio Git..."
git init
git add .
git commit -m "Examen Final - Analisis de Sistemas I - Envios Rapidos GT"

echo "Configurando remote..."
git remote add origin $REPO_URL 2>/dev/null || git remote set-url origin $REPO_URL

echo "Subiendo a GitHub..."
git branch -M main
git push -u origin main --force

echo "Listo! Repositorio subido a: $REPO_URL"
