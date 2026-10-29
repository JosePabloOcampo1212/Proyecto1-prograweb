
USE HelpDeskDB;
GO

SET NOCOUNT ON;


INSERT INTO dbo.EstadosTicket (Id, Nombre)
VALUES (1, N'Abierto'),
       (2, N'En Proceso'),
       (3, N'Resuelto'),
       (4, N'Cerrado');

DECLARE @Ahora DATETIME2(0) = SYSDATETIME();

INSERT INTO dbo.Usuarios (Nombre, Email, ContrasenaHash, Rol, FechaRegistro, Estado)
VALUES
 (N'Laura Méndez',  N'laura.mendez@helpdesk.test',  N'PBKDF2-SHA256$600000$7rlzY2SVjPK7sYJeMTcmrg==$+RDSjwFlJGz8i4HOyMG/a6Aioh7M5Ft00b/oDTl9prI=', N'Administrador', DATEADD(DAY, -90, @Ahora), 1),
 (N'Carlos Rojas',  N'carlos.rojas@helpdesk.test',  N'PBKDF2-SHA256$600000$HA0GTB7orWh8YuKC8Ko69A==$ZqwhcHy2Aw6qh2ysy6Pnnk5WRlAj3nKlY6d+ZXc5VXo=', N'Administrador', DATEADD(DAY, -90, @Ahora), 1),
 (N'Ana Vargas',    N'ana.vargas@helpdesk.test',    N'PBKDF2-SHA256$600000$qbfYZtifxL1KWWJ+CpL5PQ==$8NcqH0Uooja8Fx7MkM9f0nGH5X8fEnySUop//no9Hxc=', N'Usuario',       DATEADD(DAY, -85, @Ahora), 1),
 (N'Diego Solano',  N'diego.solano@helpdesk.test',  N'PBKDF2-SHA256$600000$97FRD2K96npYbvRmsQLV7A==$9L71ySM/Pq+2AQ8vnLxfo5KlmODx2P/40FO4EO59bHA=', N'Usuario',       DATEADD(DAY, -85, @Ahora), 1),
 (N'Marta Jiménez', N'marta.jimenez@helpdesk.test', N'PBKDF2-SHA256$600000$FskNy9RmNELKLXP4FjNEJQ==$1W4t4ijodg5qTB5dbyCDb06uNES82DxnqJShc80nf/I=', N'Usuario',       DATEADD(DAY, -85, @Ahora), 0);

DECLARE @Laura  INT = (SELECT Id FROM dbo.Usuarios WHERE Email = N'laura.mendez@helpdesk.test');
DECLARE @Carlos INT = (SELECT Id FROM dbo.Usuarios WHERE Email = N'carlos.rojas@helpdesk.test');
DECLARE @Ana    INT = (SELECT Id FROM dbo.Usuarios WHERE Email = N'ana.vargas@helpdesk.test');
DECLARE @Diego  INT = (SELECT Id FROM dbo.Usuarios WHERE Email = N'diego.solano@helpdesk.test');
DECLARE @Marta  INT = (SELECT Id FROM dbo.Usuarios WHERE Email = N'marta.jimenez@helpdesk.test');

DECLARE @Creado DATETIME2(0);
DECLARE @IdTicket INT;


SET @Creado = DATEADD(DAY, -40, @Ahora);
INSERT INTO dbo.Tickets (Titulo, Descripcion, IdUsuarioSolicitante, IdUsuarioAsignado, IdEstado, FechaCreacion, FechaActualizacion, Categoria, FechaResolucion)
VALUES (N'Configurar el correo institucional en el teléfono',
        N'Necesito tener el correo de la empresa en el teléfono para revisar mensajes cuando estoy fuera de la oficina.',
        @Marta, @Carlos, 4, @Creado, DATEADD(HOUR, 30, @Creado), N'Software', DATEADD(HOUR, 6, @Creado));
SET @IdTicket = SCOPE_IDENTITY();
INSERT INTO dbo.Comentarios (IdTicket, IdUsuario, Texto, Fecha)
VALUES (@IdTicket, @Carlos, N'Se configuró la cuenta con la aplicación de correo autorizada. Queda pendiente su confirmación.', DATEADD(HOUR, 6, @Creado));


SET @Creado = DATEADD(DAY, -25, @Ahora);
INSERT INTO dbo.Tickets (Titulo, Descripcion, IdUsuarioSolicitante, IdUsuarioAsignado, IdEstado, FechaCreacion, FechaActualizacion, Categoria, FechaResolucion)
VALUES (N'La impresora del segundo piso no imprime',
        N'Desde esta mañana los documentos quedan en la cola de impresión y no salen. Ya se intentó apagar y encender la impresora.',
        @Ana, @Carlos, 4, @Creado, DATEADD(HOUR, 26, @Creado), N'Hardware', DATEADD(HOUR, 5, @Creado));
SET @IdTicket = SCOPE_IDENTITY();
INSERT INTO dbo.Comentarios (IdTicket, IdUsuario, Texto, Fecha)
VALUES (@IdTicket, @Carlos, N'Se revisó la impresora: tenía papel atascado en la bandeja 2. Se limpió y se reinició la cola de impresión.', DATEADD(HOUR, 4, @Creado)),
       (@IdTicket, @Ana,    N'Ya imprime bien, muchas gracias.', DATEADD(HOUR, 5, @Creado));


SET @Creado = DATEADD(DAY, -18, @Ahora);
INSERT INTO dbo.Tickets (Titulo, Descripcion, IdUsuarioSolicitante, IdUsuarioAsignado, IdEstado, FechaCreacion, FechaActualizacion, Categoria, FechaResolucion)
VALUES (N'Outlook no sincroniza el correo desde ayer',
        N'Los correos nuevos no llegan a la bandeja de entrada. En el teléfono sí aparecen, pero en la computadora no.',
        @Diego, @Laura, 3, @Creado, DATEADD(HOUR, 28, @Creado), N'Software', DATEADD(HOUR, 28, @Creado));
SET @IdTicket = SCOPE_IDENTITY();
INSERT INTO dbo.Comentarios (IdTicket, IdUsuario, Texto, Fecha)
VALUES (@IdTicket, @Laura, N'¿Aparece algún mensaje de error en la barra inferior de Outlook?', DATEADD(HOUR, 2, @Creado)),
       (@IdTicket, @Diego, N'Sí, dice que está desconectado del servidor.', DATEADD(HOUR, 3, @Creado)),
       (@IdTicket, @Laura, N'Se volvió a crear el perfil de Outlook y ya sincroniza. Si vuelve a pasar, favor avisar en este ticket.', DATEADD(HOUR, 28, @Creado));


SET @Creado = DATEADD(DAY, -14, @Ahora);
INSERT INTO dbo.Tickets (Titulo, Descripcion, IdUsuarioSolicitante, IdUsuarioAsignado, IdEstado, FechaCreacion, FechaActualizacion, Categoria, FechaResolucion)
VALUES (N'El monitor parpadea constantemente',
        N'El monitor de mi puesto parpadea cada pocos segundos y se pone negro por momentos. Es difícil trabajar así.',
        @Diego, @Laura, 4, @Creado, DATEADD(HOUR, 72, @Creado), N'Hardware', DATEADD(HOUR, 50, @Creado));
SET @IdTicket = SCOPE_IDENTITY();
INSERT INTO dbo.Comentarios (IdTicket, IdUsuario, Texto, Fecha)
VALUES (@IdTicket, @Laura, N'Se cambió el cable de video y se reemplazó el monitor por uno de bodega.', DATEADD(HOUR, 50, @Creado));


SET @Creado = DATEADD(DAY, -10, @Ahora);
INSERT INTO dbo.Tickets (Titulo, Descripcion, IdUsuarioSolicitante, IdUsuarioAsignado, IdEstado, FechaCreacion, FechaActualizacion, Categoria, FechaResolucion)
VALUES (N'Restablecer la contraseña del ERP',
        N'El sistema ERP bloqueó mi usuario después de varios intentos fallidos. Necesito que se restablezca la contraseña.',
        @Ana, @Carlos, 3, @Creado, DATEADD(HOUR, 3, @Creado), N'Accesos', DATEADD(HOUR, 3, @Creado));
SET @IdTicket = SCOPE_IDENTITY();
INSERT INTO dbo.Comentarios (IdTicket, IdUsuario, Texto, Fecha)
VALUES (@IdTicket, @Carlos, N'Se desbloqueó el usuario y se envió una contraseña temporal al correo.', DATEADD(HOUR, 3, @Creado));


SET @Creado = DATEADD(DAY, -6, @Ahora);
INSERT INTO dbo.Tickets (Titulo, Descripcion, IdUsuarioSolicitante, IdUsuarioAsignado, IdEstado, FechaCreacion, FechaActualizacion, Categoria, FechaResolucion)
VALUES (N'Acceso a la carpeta compartida de Finanzas',
        N'Por cambio de funciones necesito permiso de lectura en la carpeta compartida de Finanzas.',
        @Ana, @Laura, 2, @Creado, DATEADD(HOUR, 20, @Creado), N'Accesos', NULL);
SET @IdTicket = SCOPE_IDENTITY();
INSERT INTO dbo.Comentarios (IdTicket, IdUsuario, Texto, Fecha)
VALUES (@IdTicket, @Laura, N'Se envió la solicitud de aprobación a la jefatura de Finanzas. Apenas respondan se asigna el permiso.', DATEADD(HOUR, 20, @Creado));


SET @Creado = DATEADD(DAY, -4, @Ahora);
INSERT INTO dbo.Tickets (Titulo, Descripcion, IdUsuarioSolicitante, IdUsuarioAsignado, IdEstado, FechaCreacion, FechaActualizacion, Categoria, FechaResolucion)
VALUES (N'La red Wi-Fi se desconecta cada pocos minutos',
        N'En el tercer piso la conexión inalámbrica se cae varias veces por hora. Le pasa a todo el equipo de ventas.',
        @Diego, @Carlos, 2, @Creado, DATEADD(HOUR, 6, @Creado), N'Red', NULL);
SET @IdTicket = SCOPE_IDENTITY();
INSERT INTO dbo.Comentarios (IdTicket, IdUsuario, Texto, Fecha)
VALUES (@IdTicket, @Carlos, N'Se programó una revisión del punto de acceso del tercer piso.', DATEADD(HOUR, 6, @Creado));


SET @Creado = DATEADD(HOUR, -30, @Ahora);
INSERT INTO dbo.Tickets (Titulo, Descripcion, IdUsuarioSolicitante, IdUsuarioAsignado, IdEstado, FechaCreacion, FechaActualizacion, Categoria, FechaResolucion)
VALUES (N'Instalar programa para diagramas de procesos',
        N'El área de proyectos necesita un programa para hacer diagramas de procesos en las computadoras nuevas.',
        @Ana, @Laura, 1, @Creado, DATEADD(HOUR, 2, @Creado), N'Software', NULL);


SET @Creado = DATEADD(HOUR, -20, @Ahora);
INSERT INTO dbo.Tickets (Titulo, Descripcion, IdUsuarioSolicitante, IdUsuarioAsignado, IdEstado, FechaCreacion, FechaActualizacion, Categoria, FechaResolucion)
VALUES (N'Error al abrir el sistema de planillas',
        N'Al abrir el sistema de planillas aparece un mensaje de error 500 y la ventana se cierra sola.',
        @Diego, NULL, 1, @Creado, @Creado, N'Software', NULL);

SET @Creado = DATEADD(HOUR, -3, @Ahora);
INSERT INTO dbo.Tickets (Titulo, Descripcion, IdUsuarioSolicitante, IdUsuarioAsignado, IdEstado, FechaCreacion, FechaActualizacion, Categoria, FechaResolucion)
VALUES (N'Teclado con varias teclas dañadas',
        N'Las teclas E, R y la barra espaciadora no responden bien. Hay que presionarlas varias veces.',
        @Ana, NULL, 1, @Creado, @Creado, N'Hardware', NULL);
GO


SELECT t.Id, t.Titulo, e.Nombre AS Estado, s.Nombre AS Solicitante, a.Nombre AS Asignado
FROM dbo.Tickets t
INNER JOIN dbo.EstadosTicket e ON e.Id = t.IdEstado
INNER JOIN dbo.Usuarios s ON s.Id = t.IdUsuarioSolicitante
LEFT JOIN dbo.Usuarios a ON a.Id = t.IdUsuarioAsignado
ORDER BY t.Id;
GO
