CREATE TABLE [dbo].[Pagina]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [Nombre] NVARCHAR (100) NOT NULL,
    [NombreVisible] NVARCHAR (150) NULL,
    [Descripcion] NVARCHAR (250) NULL,
    [Tipo] NVARCHAR (20) CONSTRAINT [DF_Pagina_Tipo] DEFAULT (N'Menu') NOT NULL,
    [Direccion] NVARCHAR (250) NULL,
    [PermisosPadreId] BIGINT NULL,
    [Logo] NVARCHAR (100) NULL,
    [OrdenB] INT CONSTRAINT [DF_Pagina_OrdenB] DEFAULT ((0)) NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_Pagina_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_Pagina_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_Pagina] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Pagina_Pagina_Padre] FOREIGN KEY ([PermisosPadreId]) REFERENCES [dbo].[Pagina] ([Id]),
    CONSTRAINT [CK_Pagina_Nombre_NotEmpty] CHECK (LEN(LTRIM(RTRIM([Nombre]))) > 0),
    CONSTRAINT [CK_Pagina_Nombre_MaxLen] CHECK (LEN([Nombre]) <= 100)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_Pagina_Nombre_Activa]
    ON [dbo].[Pagina]([Nombre] ASC)
    WHERE [Estatus] = 1;
