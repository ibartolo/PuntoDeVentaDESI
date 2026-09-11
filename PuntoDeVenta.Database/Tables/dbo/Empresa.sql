CREATE TABLE [dbo].[Empresa]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [NombreComercial] NVARCHAR (250) NOT NULL,
    [RazonSocial] NVARCHAR (250) NOT NULL,
    [RFC] NVARCHAR (50) NOT NULL,
    [Responsable] NVARCHAR (250) NOT NULL,
    [Direccion] NVARCHAR (500) NOT NULL,
    [Ciudad] NVARCHAR (100) NULL,
    [Estado] NVARCHAR (100) NULL,
    [CodigoPostal] NVARCHAR (10) NULL,
    [Telefono] NVARCHAR (50) NULL,
    [CorreoContacto] NVARCHAR (250) NOT NULL,
    [FechaVigenciaInicio] DATETIME NOT NULL,
    [FechaVigenciaFin] DATETIME NOT NULL,
    [EsPeriodoPrueba] BIT CONSTRAINT [DF_Empresa_EsPeriodoPrueba] DEFAULT ((1)) NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_Empresa_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    [LogoUrl] NVARCHAR (500) NULL,
    CONSTRAINT [PK_Empresa] PRIMARY KEY CLUSTERED ([Id] ASC)
);
