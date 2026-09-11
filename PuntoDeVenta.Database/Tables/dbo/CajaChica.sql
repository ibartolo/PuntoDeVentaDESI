CREATE TABLE [dbo].[CajaChica]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [SucursalId] BIGINT NOT NULL,
    [UsuarioId] BIGINT NOT NULL,
    [MontoInicial] DECIMAL (18, 4) CONSTRAINT [DF_CajaChica_MontoInicial] DEFAULT ((0)) NOT NULL,
    [IngresosTotales] DECIMAL (18, 4) CONSTRAINT [DF_CajaChica_IngresosTotales] DEFAULT ((0)) NOT NULL,
    [SalidasTotales] DECIMAL (18, 4) CONSTRAINT [DF_CajaChica_SalidasTotales] DEFAULT ((0)) NOT NULL,
    [FechaApertura] DATETIME CONSTRAINT [DF_CajaChica_FechaApertura] DEFAULT (GETDATE()) NOT NULL,
    [FechaCierre] DATETIME NULL,
    [Estado] NVARCHAR (15) CONSTRAINT [DF_CajaChica_Estado] DEFAULT (N'Abierta') NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_CajaChica_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_CajaChica_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_CajaChica] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CajaChica_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [FK_CajaChica_Sucursal] FOREIGN KEY ([SucursalId]) REFERENCES [dbo].[Sucursal] ([Id]),
    CONSTRAINT [FK_CajaChica_Usuario] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuario] ([Id]),
    CONSTRAINT [CK_CajaChica_Estado] CHECK ([Estado] IN (N'Abierta', N'Cerrada'))
);
