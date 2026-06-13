# Informe de Utilizacion de IA - Examen Final Analisis de Sistemas I

**Alumno:** Ludin Carranza  
**Fecha:** 13/Jun/2026  
**Curso:** Analisis de Sistemas I  

---

## Descripcion general

Durante el desarrollo de este examen utilice herramientas de inteligencia artificial de manera complementaria para apoyarme en algunos aspectos del proyecto. Sin embargo, la mayor parte del analisis, diseño y toma de decisiones fue realizada por mi mismo.

---

## Prompts enviados

### Prompt 1
> "Como se calcula el porcentaje de eficiencia en un reporte de logistica?"

**Uso:** Solo use esto para confirmar la formula matematica basica (entregados / total * 100). La implementacion la hice yo.

### Prompt 2
> "Cual es la diferencia entre xUnit y NUnit para pruebas en .NET?"

**Uso:** Queria confirmar cual era mas comun en proyectos .NET 8. La IA explico brevemente ambos. Yo ya sabia que usariamos xUnit por ser el mas estandar.

### Prompt 3
> "Como se conecta SQLite con Dapper en .NET?"

**Uso:** Pedi un ejemplo rapido de la sintaxis de conexion. La implementacion concreta del servicio la hice yo basandome en lo que hemos visto en clase.

---

## Reflexion

La IA fue una herramienta de consulta rapida, similar a buscar en documentacion. Las decisiones de arquitectura (separar en capas, usar el patron Service + Controller, definir las transiciones de estado) las tome yo basandome en los conocimientos del curso. El diseño de las historias de usuario tambien es mio, adaptado al caso de negocio descrito en el examen.

En general, la IA no me ayudo a resolver el examen como tal, sino que fue util para aclarar dudas tecnicas puntuales que de otra forma hubiera resuelto revisando apuntes o buscando en internet.

---

## Correcciones realizadas

- Inicialmente habia planteado guardar el historial dentro del mismo objeto Envio, pero decidi separarlo en una tabla propia para cumplir mejor con el requerimiento de auditoria.
- Corregi la logica del descuento: en un primer borrador solo aplicaba si el remitente tenia NIT, pero el requerimiento dice "remitente O destinatario".

---

*Este informe fue elaborado con honestidad academica.*
