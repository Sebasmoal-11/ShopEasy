# ShopEasy CRM

Sistema de gestion de relaciones con clientes (CRM) para una tienda de comercio
electronico ficticia.

## Objetivo

Centralizar la informacion de clientes, pedidos, productos e interacciones para
mejorar la atencion al cliente y facilitar la toma de decisiones.

## Tecnologias

- C# y ASP.NET Core Web API (.NET 8)
- Entity Framework Core 8 (Database First)
- MySQL 8
- JWT y BCrypt
- HTML, CSS y Bootstrap (frontend)
- Git y GitHub

## Estructura

- `Backend/ShopEasy.API`: controladores, configuracion y Program.cs.
- `Backend/ShopEasy.BC`: servicios y reglas de negocio.
- `Backend/ShopEasy.DA`: entidades, DbContext y acceso a MySQL.
- `Backend/ShopEasy.BW`: DTOs de Auth, Users y Dashboard.
- `Backend/ShopEasy.sln`: solucion con los cuatro proyectos.
- `FrontEnd/`: interfaz de usuario (pendiente).
- `DataBase/ShopEasy.sql`: esquema de 22 tablas y roles del sistema.

La documentacion completa del backend esta en
[Backend/README.md](Backend/README.md).

## Inicio rapido

1. Importar `DataBase/ShopEasy.sql` en MySQL Workbench.
2. Copiar `Backend/ShopEasy.API/appsettings.Development.example.json` a
   `Backend/ShopEasy.API/appsettings.Development.json`.
3. Editar la cadena de conexion y la clave JWT en ese archivo.
4. Ejecutar la API:

   ```powershell
   cd Backend\ShopEasy.API
   dotnet run
   ```

La API queda en `http://localhost:5080` y Swagger en
`http://localhost:5080/swagger`.

## Estado

Proyecto academico en desarrollo. El backend esta implementado y probado contra
MySQL; el frontend aun no se ha desarrollado.
