# Envios Rapidos GT - API REST

Sistema de gestion de envios y paqueteria para la empresa **Envios Rapidos GT**, desarrollado como proyecto final del curso Analisis de Sistemas I.

**Alumno:** Ludin Carranza  
**Carnet:** [0907-22-5817]  
**Fecha:** 13/Jun/2026

---

## Descripcion del proyecto

API REST desarrollada en **.NET 8** con **SQLite + Dapper** que permite gestionar el ciclo de vida completo de un envio: desde su registro hasta la entrega o devolucion. Incluye calculo automatico de tarifas, control de intentos fallidos de entrega, historial de estados y reporte de eficiencia.

---

## Tecnologias utilizadas

- .NET 8 Web API
- SQLite (base de datos embebida)
- Dapper (micro ORM)
- Swagger / OpenAPI
- xUnit (pruebas unitarias)
- Render.com (despliegue)

---

## Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Git

---

## Instalacion y ejecucion local

```bash
# 1. Clonar el repositorio
git clone https://github.com/lcarranzag2-ui/ludincarranzaexamenfinalanalisis.git
cd ludincarranzaexamenfinalanalisis

# 2. Restaurar dependencias
dotnet restore

# 3. Ejecutar la API
dotnet run --project src/EnviosRapidosGT.API

# 4. Abrir Swagger en el navegador
# http://localhost:5000
```

La base de datos SQLite se crea automaticamente al iniciar la aplicacion.

---

## Ejecutar pruebas unitarias

```bash
dotnet test src/EnviosRapidosGT.Tests
```

Las pruebas cubren:
- Calculo de tarifas por peso (TarifaCalculatorTests)
- Validacion de transiciones de estado (EstadoValidatorTests)
- Logica del servicio con base de datos en memoria (EnvioServiceTests)

---

## Endpoints disponibles

| Metodo | Endpoint | Descripcion |
|--------|----------|-------------|
| POST | `/api/envios` | Crear un nuevo envio |
| GET | `/api/envios` | Listar todos los envios |
| GET | `/api/envios/{codigo}` | Obtener envio por codigo de rastreo |
| PUT | `/api/envios/{codigo}/estado` | Actualizar estado del envio |
| POST | `/api/envios/{codigo}/intento-fallido` | Registrar intento fallido de entrega |
| GET | `/api/envios/{codigo}/historial` | Ver historial completo de estados |
| GET | `/api/envios/reporte/eficiencia` | Reporte de eficiencia de entregas |

---

## Ejemplo de uso

### Crear un envio

```json
POST /api/envios
{
  "remitente": "Juan Perez",
  "destinatario": "Maria Lopez",
  "nitRemitente": "1234567-8",
  "pesoKg": 3.5,
  "oficinaOrigen": "Oficina Central Guatemala"
}
```

**Respuesta:**
```json
{
  "id": 1,
  "codigoRastreo": "ENV-20260613-4821",
  "tarifa": 42.75,
  "estado": "Registrado",
  "oficinaActual": "Oficina Central Guatemala"
}
```

### Actualizar estado

```json
PUT /api/envios/ENV-20260613-4821/estado
{
  "nuevoEstado": "EnTransito",
  "ubicacion": "Bodega Central Zona 7",
  "notas": "Paquete recibido en bodega"
}
```

---

## Flujo de estados

```
Registrado --> EnTransito --> EnReparto --> Entregado
                    |               |
                    v               v
                 Devuelto      EnDevolucion --> Devuelto
```

**Regla importante:** Al tercer intento fallido de entrega, el envio pasa automaticamente a `EnDevolucion`.

---

## Calculo de tarifas

| Peso | Tarifa |
|------|--------|
| <= 1 kg | Q25.00 |
| 1.01 - 5 kg | Q45.00 |
| 5.01 - 10 kg | Q75.00 |
| > 10 kg | Q100.00 |

Con NIT valido (remitente o destinatario): **5% de descuento**.

---

## Despliegue en Render.com

El proyecto incluye `render.yaml` para despliegue automatico. La base de datos persiste en un disco de 1GB montado en `/data`.

---

## Estructura del proyecto

```
EnviosRapidosGT/
├── src/
│   ├── EnviosRapidosGT.API/
│   │   ├── Controllers/
│   │   │   └── EnviosController.cs
│   │   ├── Models/
│   │   │   ├── Envio.cs
│   │   │   ├── HistorialEstado.cs
│   │   │   └── Dtos.cs
│   │   ├── Services/
│   │   │   ├── IEnvioService.cs
│   │   │   └── EnvioService.cs
│   │   ├── Helpers/
│   │   │   ├── Calculators.cs
│   │   │   └── EstadoValidator.cs
│   │   ├── Data/
│   │   │   └── DatabaseInitializer.cs
│   │   ├── Program.cs
│   │   └── appsettings.json
│   └── EnviosRapidosGT.Tests/
│       ├── TarifaCalculatorTests.cs
│       ├── EstadoValidatorTests.cs
│       └── EnvioServiceTests.cs
├── docs/
│   ├── historias-de-usuario.md
│   ├── informe-uso-ia.md
│   └── diagrama-flujo-proceso.md
├── render.yaml
└── README.md
```
