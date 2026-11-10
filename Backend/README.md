# ShopEasy CRM - Backend

Backend del CRM ShopEasy, desarrollado como proyecto universitario con una
arquitectura de cuatro capas.

## Tecnologias

- C# y ASP.NET Core Web API con .NET 8
- Entity Framework Core 8 (Database First)
- MySQL 8 y Pomelo.EntityFrameworkCore.MySql
- JWT para la autenticacion
- BCrypt para el hash de contrasenas

## Estructura

```text
Backend/
├── ShopEasy.API/    Controladores, Program.cs, JWT, CORS y configuracion
├── ShopEasy.BC/     Servicios y reglas de negocio
├── ShopEasy.DA/     Entidades, DbContext y acceso a MySQL
├── ShopEasy.BW/     DTOs de Auth, Users y Dashboard
└── ShopEasy.sln
```

Referencias entre proyectos:

```text
API -> BC, DA, BW
BC  -> DA, BW
DA  -> (sin referencias a otros proyectos)
BW  -> (sin referencias a otros proyectos)
```

Responsabilidades:

- **ShopEasy.API:** recibe las solicitudes HTTP y devuelve las respuestas. Aqui
  estan los controladores, la configuracion de JWT, CORS y la inyeccion de
  dependencias.
- **ShopEasy.BC:** contiene `AuthService`, `UserService` y `DashboardService`
  con las reglas de login, validacion, creacion de usuarios y estadisticas.
- **ShopEasy.DA:** contiene las 22 entidades, `ShopEasyDbContext`, la
  configuracion de Pomelo y las consultas a MySQL (`AuthDA`, `UserDA`,
  `DashboardDA`).
- **ShopEasy.BW:** contiene los DTOs que se transportan entre las capas.

## Base de datos

El esquema se encuentra en `DataBase/ShopEasy.sql` y no debe modificarse. Incluye
las 22 tablas y los roles `Administrador` y `Empleado`.

Desde MySQL Workbench:

1. Abrir el archivo `DataBase/ShopEasy.sql`.
2. Ejecutarlo completo con el boton de rayo.
3. Verificar que se haya creado la base de datos `ShopEasy`.

Desde la terminal, en la raiz del repositorio:

```powershell
cmd /c "mysql -u root -p < DataBase\ShopEasy.sql"
```

## Configuracion

Cada integrante debe configurar su propia conexion. No se suben contrasenas al
repositorio.

1. Copiar el archivo de ejemplo:

   ```powershell
   Copy-Item Backend\ShopEasy.API\appsettings.Development.example.json Backend\ShopEasy.API\appsettings.Development.json
   ```

2. Editar `Backend/ShopEasy.API/appsettings.Development.json`:

   ```json
   {
     "ConnectionStrings": {
       "ShopEasy": "Server=localhost;Port=3306;Database=ShopEasy;User=root;Password=TU_CONTRASENA;"
     },
     "Jwt": {
       "Key": "UNA_CLAVE_LARGA_DE_AL_MENOS_32_CARACTERES"
     }
   }
   ```

   `appsettings.Development.json` esta en `.gitignore`, por lo que no se sube a
   GitHub. La clave JWT puede ser cualquier texto de al menos 32 caracteres. En
   PowerShell se puede generar una con:

   ```powershell
   [guid]::NewGuid().ToString('N') + [guid]::NewGuid().ToString('N')
   ```

3. Si se prefiere User Secrets, tambien se puede ejecutar:

   ```powershell
   dotnet user-secrets set "ConnectionStrings:ShopEasy" "Server=localhost;Port=3306;Database=ShopEasy;User=root;Password=TU_CONTRASENA;" --project Backend\ShopEasy.API
   dotnet user-secrets set "Jwt:Key" "UNA_CLAVE_LARGA_DE_AL_MENOS_32_CARACTERES" --project Backend\ShopEasy.API
   ```

## Datos iniciales

El sistema no tiene registro publico ni comandos de inicializacion. Antes de
usar la API, crea directamente en MySQL la persona, su correo y el usuario con su
rol, guardando la contrasena como hash BCrypt.

## Ejecutar

```powershell
cd Backend\ShopEasy.API
dotnet run
```

La API queda disponible en `http://localhost:5080` y Swagger en
`http://localhost:5080/swagger`.

## Endpoints

| Metodo | Ruta | Acceso | Descripcion |
| --- | --- | --- | --- |
| `POST` | `/api/auth/login` | Publico | Inicia sesion y devuelve un JWT |
| `POST` | `/api/users` | Administrador | Crea una persona y su usuario |
| `GET` | `/api/users/roles` | Administrador | Lista los roles de MySQL |
| `GET` | `/api/dashboard` | Usuario autenticado | Devuelve los conteos reales |

### Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "correo": "admin@example.com",
  "contrasena": "Admin_ShopEasy_2026!"
}
```

Respuesta `200`:

```json
{
  "token": "eyJ...",
  "tokenType": "Bearer",
  "expiresAtUtc": "2026-11-10T20:30:00Z",
  "user": {
    "idUsuario": 1,
    "cedula": "101110111",
    "nombreCompleto": "Ana Mora",
    "correo": "admin@example.com",
    "rol": "Administrador"
  }
}
```

Si el correo no existe, la contrasena es incorrecta, el usuario esta inactivo o
el correo esta inactivo, se devuelve `401` con el mensaje
`Correo o contrasena incorrectos.` No se devuelve la contrasena ni el hash.

### Crear usuario

```http
POST /api/users
Authorization: Bearer <token de administrador>
Content-Type: application/json

{
  "cedula": "202220222",
  "nombre": "Carlos",
  "apellidos": "Rojas Vega",
  "correo": "empleado@example.com",
  "contrasena": "Empleado_ShopEasy_2026!",
  "idRol": 2
}
```

La operacion guarda en `personas`, `correos_persona` y `usuarios` dentro de la
misma operacion de Entity Framework. Devuelve `201`, `409` si la cedula o el
correo ya existen, y `400` si los datos o el rol no son validos.

### Roles

```http
GET /api/users/roles
Authorization: Bearer <token de administrador>
```

Devuelve los roles reales de la tabla `roles`.

### Dashboard

```http
GET /api/dashboard
Authorization: Bearer <token>
```

```json
{
  "generatedAtUtc": "2026-11-10T19:59:49Z",
  "user": {
    "idUsuario": 1,
    "cedula": "101110111",
    "nombreCompleto": "Ana Mora",
    "correo": "admin@example.com",
    "rol": "Administrador"
  },
  "stats": {
    "totalClientes": 1,
    "totalPedidos": 1,
    "totalCasosCliente": 1,
    "totalCampanas": 1
  }
}
```

Los conteos se consultan directamente en `clientes`, `pedidos`,
`casos_cliente` y `campanas`. No hay datos simulados.

## Errores

Los errores se devuelven en un formato sencillo:

```json
{
  "status": 409,
  "message": "El correo electronico ya esta registrado."
}
```

Se utilizan los codigos `400`, `401`, `403`, `404`, `409` y `500`. El detalle de
los errores inesperados se escribe en el log y no se muestra al cliente.

## Probar las funcionalidades

El archivo `Backend/ShopEasy.API/ShopEasy.API.http` contiene solicitudes de
ejemplo para login, dashboard, roles, creacion de usuarios y casos de error. Se
puede abrir con Visual Studio o con la extension REST Client de VS Code.

Tambien se puede usar Swagger:

1. Ejecutar `POST /api/auth/login`.
2. Copiar el valor de `token`.
3. Pulsar **Authorize** y escribir el token.
4. Probar `GET /api/dashboard`, `GET /api/users/roles` y `POST /api/users`.

Para comprobar que solo un administrador puede crear usuarios, iniciar sesion con
un usuario de rol `Empleado` y probar `POST /api/users`: debe devolver `403`.

## Verificacion realizada

- `dotnet build Backend\ShopEasy.sln`: 0 errores y 0 advertencias.
- Proyectos verificados en `net8.0`: API, BC, DA y BW.
- Referencias verificadas: API -> BC/DA/BW, BC -> DA/BW, DA y BW sin referencias
  a otros proyectos.
- Pruebas ejecutadas contra una instancia MySQL 8.0.46 creada con
  `DataBase/ShopEasy.sql`:
  - login de administrador y empleado con JWT;
  - dashboard con conteos reales `1/1/1/1`;
  - creacion de usuario desde una cuenta administradora;
  - respuesta `403` al intentar crear un usuario con una cuenta empleado;
  - respuestas `409` para cedula y correo duplicados;
  - respuestas `400` para rol y datos invalidos;
  - `401` sin token, con token invalido, con usuario inactivo y con correo
    inactivo;
  - `404` para rutas inexistentes y registro publico.
- No se incluye un proyecto de pruebas automatizadas; las pruebas se hicieron
  contra la API y MySQL en ejecucion.
