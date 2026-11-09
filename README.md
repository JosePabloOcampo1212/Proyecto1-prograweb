# Help Desk – Sistema de Gestión de Solicitudes de Soporte

Proyecto 1 del curso 03101 Programación avanzada en web (UNED).
Aplicación web en ASP.NET MVC 5 (.NET Framework 4.8), SQL Server, JavaScript y AJAX (jQuery).

## Requisitos

- Windows con Visual Studio 2022 y la carga de trabajo "Desarrollo de ASP.NET y web".
- .NET Framework 4.8 (Developer Pack).
- SQL Server 2019 o 2022 (Express, Developer o LocalDB) y SQL Server Management Studio.

## Puesta en marcha

1. Abrir SQL Server Management Studio y ejecutar, en este orden, los scripts de la carpeta `BaseDatos`:
   1. `01_CrearBaseDatos.sql`
   2. `02_Tablas.sql`
   3. `03_DatosIniciales.sql`
2. Abrir `HelpDesk.sln` en Visual Studio.
3. Revisar la cadena de conexión `HelpDeskDB` en `HelpDesk/Web.config` y ajustar `Data Source` según la instancia:
   - `localhost` para una instancia por defecto.
   - `.\SQLEXPRESS` para SQL Server Express.
   - `(localdb)\MSSQLLocalDB` para LocalDB.
4. Compilar (Visual Studio restaura automáticamente los paquetes NuGet) y ejecutar con F5 (IIS Express).

## Usuarios de prueba

| Rol | Correo | Contraseña |
|---|---|---|
| Administrador | laura.mendez@helpdesk.test | Soporte2026* |
| Administrador | carlos.rojas@helpdesk.test | Soporte2026* |
| Usuario | ana.vargas@helpdesk.test | Usuario2026* |
| Usuario | diego.solano@helpdesk.test | Usuario2026* |
| Usuario (inactivo) | marta.jimenez@helpdesk.test | Usuario2026* |

## Estructura

```
BaseDatos/                 Scripts SQL (creación, tablas y datos de prueba)
HelpDesk/
  Models/Entidades/        Clases que representan las tablas
  Models/ViewModels/       Modelos de las vistas
  Datos/                   Acceso a datos con ADO.NET (consultas parametrizadas)
  Negocio/                 Reglas de negocio y permisos
  Seguridad/               Hash de contraseñas, usuario autenticado y filtro de roles
  Helpers/                 Formato de fechas, respuestas JSON y de error
  Controllers/             Cuenta, Tickets, Comentarios y Reportes
  Views/                   Vistas Razor
  Scripts/app/             JavaScript de las funciones AJAX
```
