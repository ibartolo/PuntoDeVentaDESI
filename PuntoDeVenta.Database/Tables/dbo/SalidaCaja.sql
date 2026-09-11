CREATE TABLE [dbo].[SalidaCaja]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [CajaChicaId] BIGINT NOT NULL,
    [Monto] DECIMAL (18, 4) NOT NULL,
    [Comentario] NVARCHAR (300) NULL,
    [FechaHora] DATETIME CONSTRAINT [DF_SalidaCaja_FechaHora] DEFAULT (GETDATE()) NOT NULL,
    [Justificada] BIT CONSTRAINT [DF_SalidaCaja_Justificada] DEFAULT ((0)) NOT NULL,
    [EvidenciaUrl] NVARCHAR (300) NULL,
    [TipoSalida] NVARCHAR (30) NULL,
    [Estatus] BIT CONSTRAINT [DF_SalidaCaja_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_SalidaCaja_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_SalidaCaja] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SalidaCaja_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [FK_SalidaCaja_CajaChica] FOREIGN KEY ([CajaChicaId]) REFERENCES [dbo].[CajaChica] ([Id])
);
