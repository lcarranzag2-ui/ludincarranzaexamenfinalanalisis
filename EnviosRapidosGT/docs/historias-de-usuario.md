# Historias de Usuario - Envios Rapidos GT

---

## HU-01: Registrar nuevo envio

**Como** operador de oficina  
**Quiero** registrar un nuevo paquete en el sistema  
**Para** que el cliente pueda rastrear su envio desde el inicio

**Criterios de aceptacion:**
- El sistema genera automaticamente un codigo de rastreo con formato ENV-YYYYMMDD-XXXX
- La tarifa se calcula automaticamente segun el peso (<=1kg=Q25, 1-5kg=Q45, 5-10kg=Q75, >10kg=Q100)
- Si el remitente o destinatario tiene NIT valido, se aplica 5% de descuento
- El estado inicial siempre es "Registrado"
- Se registra la oficina de origen en el historial

---

## HU-02: Consultar estado de envio por codigo de rastreo

**Como** cliente o operador  
**Quiero** consultar el estado actual de un paquete usando su codigo de rastreo  
**Para** saber en que etapa del proceso se encuentra

**Criterios de aceptacion:**
- Se busca el envio usando el codigo ENV-YYYYMMDD-XXXX
- Si el codigo no existe, se devuelve un mensaje de error claro (404)
- La respuesta incluye: estado actual, oficina actual, fecha de creacion, tarifa y datos del envio

---

## HU-03: Actualizar estado de un envio

**Como** operador de oficina  
**Quiero** cambiar el estado de un envio a medida que avanza en el proceso  
**Para** mantener informado al cliente en tiempo real

**Criterios de aceptacion:**
- Solo se permiten las siguientes transiciones:
  - Registrado -> EnTransito
  - EnTransito -> EnReparto | Devuelto
  - EnReparto -> Entregado | EnDevolucion
  - EnDevolucion -> Devuelto
- Se debe especificar la ubicacion (oficina) donde se realiza el cambio
- Cada cambio queda registrado en el historial con timestamp automatico

---

## HU-04: Registrar intento fallido de entrega

**Como** repartidor  
**Quiero** registrar cuando no logro entregar un paquete  
**Para** que el sistema controle automaticamente los reintentos

**Criterios de aceptacion:**
- Solo se puede registrar intento fallido si el envio esta en estado "EnReparto"
- Al llegar al tercer intento fallido, el estado cambia automaticamente a "EnDevolucion"
- Cada intento queda registrado en el historial con notas opcionales
- El sistema notifica cuantos intentos lleva el paquete

---

## HU-05: Consultar historial completo de un envio

**Como** supervisor o cliente  
**Quiero** ver el historial completo de movimientos de un paquete  
**Para** auditar o resolver problemas con el envio

**Criterios de aceptacion:**
- Se muestra la lista ordenada cronologicamente de todos los cambios de estado
- Cada entrada incluye: estado anterior, estado nuevo, ubicacion, timestamp y notas
- Si el codigo no existe, se devuelve una lista vacia

---

## HU-06: Generar reporte de eficiencia de entrega

**Como** gerente de logistica  
**Quiero** obtener un reporte con estadisticas de entrega  
**Para** evaluar el rendimiento del servicio

**Criterios de aceptacion:**
- El reporte incluye: total de envios, entregados, devueltos y en proceso
- Se calcula el porcentaje de eficiencia (entregados / total * 100)
- Se muestra el total de ingresos generados por tarifas

---

## HU-07: Listar todos los envios registrados

**Como** supervisor de oficina  
**Quiero** ver todos los envios registrados en el sistema  
**Para** tener una vision general del estado operativo

**Criterios de aceptacion:**
- Se listan todos los envios ordenados por fecha de creacion (mas recientes primero)
- Cada registro incluye: codigo de rastreo, remitente, destinatario, estado actual y tarifa

---

## HU-08: Calcular tarifa automaticamente segun el peso

**Como** operador de oficina  
**Quiero** que el sistema calcule la tarifa automaticamente al registrar un envio  
**Para** no tener que calcular manualmente y evitar errores

**Criterios de aceptacion:**
- Peso <= 1 kg: Q25.00
- Peso 1.01 - 5 kg: Q45.00
- Peso 5.01 - 10 kg: Q75.00
- Peso > 10 kg: Q100.00
- Si el cliente tiene NIT valido: descuento del 5% sobre la tarifa calculada

---

## HU-09: Validar NIT para aplicar descuento

**Como** operador de caja  
**Quiero** que el sistema valide si el remitente o destinatario tiene NIT  
**Para** aplicar automaticamente el descuento del 5%

**Criterios de aceptacion:**
- Un NIT se considera valido si tiene al menos 7 caracteres
- El descuento se aplica si al menos uno de los dos (remitente o destinatario) tiene NIT valido
- La tarifa con descuento se muestra en la respuesta al crear el envio

---

## HU-10: Bloquear transiciones de estado invalidas

**Como** sistema  
**Quiero** impedir que un envio cambie a un estado no permitido  
**Para** mantener la integridad del flujo de logistica

**Criterios de aceptacion:**
- El sistema rechaza cualquier intento de cambio de estado no contemplado en el flujo
- Un envio en estado "Entregado" o "Devuelto" no puede cambiar a ningun otro estado
- Se devuelve un mensaje de error claro indicando que la transicion no es valida
