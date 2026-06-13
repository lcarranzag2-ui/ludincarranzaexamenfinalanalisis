# Diagrama de Flujo - Proceso de Resolucion del Examen

```
INICIO
  |
  v
[Leer el caso de negocio: Envios Rapidos GT]
  |
  v
[Identificar entidades principales]
  - Envio
  - HistorialEstado
  - Oficina (como campo de texto)
  |
  v
[Definir reglas de negocio]
  - Calculo de tarifas por peso
  - Flujo de estados permitidos
  - Maximo 3 intentos fallidos
  - Descuento por NIT
  - Formato codigo de rastreo
  |
  v
[Disenar Historias de Usuario (10)]
  - Formato: Como / Quiero / Para / Criterios
  |
  v
[Disenar arquitectura de la API]
  - Capas: Controllers -> Services -> Data
  - SQLite + Dapper (simplificado)
  - Models y DTOs separados
  |
  v
[Crear modelos (Models)]
  - Envio.cs
  - HistorialEstado.cs
  - Dtos.cs (CrearEnvioDto, ActualizarEstadoDto, etc.)
  |
  v
[Implementar logica de negocio (Services)]
  - EnvioService.cs
  - Calculos de tarifa
  - Validacion de transiciones de estado
  - Logica de intentos fallidos
  |
  v
[Implementar endpoints REST (Controllers)]
  - POST /api/envios
  - GET /api/envios
  - GET /api/envios/{codigo}
  - PUT /api/envios/{codigo}/estado
  - POST /api/envios/{codigo}/intento-fallido
  - GET /api/envios/{codigo}/historial
  - GET /api/envios/reporte/eficiencia
  |
  v
[Escribir pruebas unitarias (xUnit)]
  - TarifaCalculatorTests (12 pruebas)
  - EstadoValidatorTests (9 pruebas)
  - EnvioServiceTests (10 pruebas)
  |
  v
[Configurar despliegue en Render.com]
  - render.yaml con configuracion del servicio web
  |
  v
[Documentar en README.md]
  - Descripcion del proyecto
  - Instrucciones de instalacion
  - Como correr pruebas
  - Endpoints disponibles
  |
  v
[Subir a GitHub]
  Repositorio: ludincarranzaexamenfinalanalisis
  |
  v
FIN
```
