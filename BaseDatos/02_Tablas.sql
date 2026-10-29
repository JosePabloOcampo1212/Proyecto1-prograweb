

USE HelpDeskDB;
GO


CREATE TABLE dbo.EstadosTicket
(
    Id      INT           NOT NULL,
    Nombre  NVARCHAR(30)  NOT NULL,

    CONSTRAINT PK_EstadosTicket PRIMARY KEY (Id),
    CONSTRAINT UQ_EstadosTicket_Nombre UNIQUE (Nombre)
);
GO


CREATE TABLE dbo.Usuarios
(
    Id              INT            IDENTITY(1,1) NOT NULL,
    Nombre          NVARCHAR(100)  NOT NULL,
    Email           NVARCHAR(150)  NOT NULL,
    ContrasenaHash  NVARCHAR(256)  NOT NULL,
    Rol             NVARCHAR(20)   NOT NULL,
    FechaRegistro   DATETIME2(0)   NOT NULL CONSTRAINT DF_Usuarios_FechaRegistro DEFAULT (SYSDATETIME()),
    Estado          BIT            NOT NULL CONSTRAINT DF_Usuarios_Estado DEFAULT (1),

    CONSTRAINT PK_Usuarios PRIMARY KEY (Id),
    CONSTRAINT UQ_Usuarios_Email UNIQUE (Email),
    CONSTRAINT CK_Usuarios_Rol CHECK (Rol IN (N'Usuario', N'Administrador'))
);
GO


CREATE TABLE dbo.Tickets
(
    Id                    INT            IDENTITY(1,1) NOT NULL,
    Titulo                NVARCHAR(150)  NOT NULL,
    Descripcion           NVARCHAR(MAX)  NOT NULL,
    IdUsuarioSolicitante  INT            NOT NULL,
    IdUsuarioAsignado     INT            NULL,
    IdEstado              INT            NOT NULL CONSTRAINT DF_Tickets_IdEstado DEFAULT (1),
    FechaCreacion         DATETIME2(0)   NOT NULL CONSTRAINT DF_Tickets_FechaCreacion DEFAULT (SYSDATETIME()),
    FechaActualizacion    DATETIME2(0)   NOT NULL CONSTRAINT DF_Tickets_FechaActualizacion DEFAULT (SYSDATETIME()),
    Categoria             NVARCHAR(50)   NOT NULL,
    FechaResolucion       DATETIME2(0)   NULL,

    CONSTRAINT PK_Tickets PRIMARY KEY (Id),
    CONSTRAINT FK_Tickets_UsuarioSolicitante FOREIGN KEY (IdUsuarioSolicitante) REFERENCES dbo.Usuarios (Id),
    CONSTRAINT FK_Tickets_UsuarioAsignado    FOREIGN KEY (IdUsuarioAsignado)    REFERENCES dbo.Usuarios (Id),
    CONSTRAINT FK_Tickets_EstadosTicket      FOREIGN KEY (IdEstado)             REFERENCES dbo.EstadosTicket (Id),
    CONSTRAINT CK_Tickets_Categoria CHECK (Categoria IN (N'Hardware', N'Software', N'Red', N'Accesos', N'Otro')),
    CONSTRAINT CK_Tickets_FechaActualizacion CHECK (FechaActualizacion >= FechaCreacion),
    CONSTRAINT CK_Tickets_FechaResolucion CHECK (FechaResolucion IS NULL OR FechaResolucion >= FechaCreacion)
);
GO


CREATE TABLE dbo.Comentarios
(
    Id         INT             IDENTITY(1,1) NOT NULL,
    IdTicket   INT             NOT NULL,
    IdUsuario  INT             NOT NULL,
    Texto      NVARCHAR(1000)  NOT NULL,
    Fecha      DATETIME2(0)    NOT NULL CONSTRAINT DF_Comentarios_Fecha DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_Comentarios PRIMARY KEY (Id),
    CONSTRAINT FK_Comentarios_Tickets  FOREIGN KEY (IdTicket)  REFERENCES dbo.Tickets (Id) ON DELETE CASCADE,
    CONSTRAINT FK_Comentarios_Usuarios FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuarios (Id),
    CONSTRAINT CK_Comentarios_Texto CHECK (LEN(Texto) > 0)
);
GO


CREATE INDEX IX_Tickets_IdUsuarioSolicitante ON dbo.Tickets (IdUsuarioSolicitante);
CREATE INDEX IX_Tickets_IdUsuarioAsignado    ON dbo.Tickets (IdUsuarioAsignado);
CREATE INDEX IX_Tickets_IdEstado             ON dbo.Tickets (IdEstado);
CREATE INDEX IX_Tickets_FechaCreacion        ON dbo.Tickets (FechaCreacion);
CREATE INDEX IX_Tickets_FechaResolucion      ON dbo.Tickets (FechaResolucion);
CREATE INDEX IX_Comentarios_IdTicket         ON dbo.Comentarios (IdTicket, Fecha);
CREATE INDEX IX_Comentarios_IdUsuario        ON dbo.Comentarios (IdUsuario);
GO
