# Base de datos Northwind2

Esta carpeta contiene el respaldo de la base de datos SQL Server utilizada por el proyecto **NorthwindTradersV9WebMVC**.
La versión de SQL Server es la versión 2022

## Archivo de respaldo


| Propiedad | Descripción |
|---|---|
| Nombre | Northwind2SqlServer-21Nov2026-1908.bak |
| Base de datos | Northwind2 |
| Fecha del respaldo | 21 de noviembre de 2026 |
| Hora del respaldo | 19:08 |
| Tamaño | 20.1 MB |
| Formato | SQL Server Backup (`.bak`) |

## Restauración de la base de datos

Para restaurar la base de datos:

1. Abrir SQL Server Management Studio (SSMS).
2. Conectarse a la instancia de SQL Server.
3. Hacer clic derecho en **Databases**.
4. Seleccionar **Restore Database...**.
5. Seleccionar **Device** como origen del respaldo.
6. Localizar el archivo `Northwind2SqlServer-21Nov2026-1908.bak`.
7. Escribir `Northwind2` como nombre de la base de datos de destino.
8. Ejecutar la restauración.

También es posible restaurar la base de datos mediante un comando SQL Server `RESTORE DATABASE`.

## Configuración de conexión

La aplicación utiliza la cadena de conexión identificada con la clave:

`NorthwindConnection`

El nombre de la base de datos configurado para la aplicación es:

`Northwind2`

La cadena de conexión debe configurarse de acuerdo con la instancia de SQL Server del entorno donde se ejecute la aplicación.

## Consideraciones

- Este respaldo corresponde a una versión de la base de datos utilizada por el proyecto.
- Se recomienda restaurar el respaldo en un entorno de desarrollo o pruebas.
- No se deben almacenar contraseñas ni credenciales de conexión en este repositorio.
- Se recomienda conservar respaldos adicionales antes de realizar modificaciones en la base de datos.