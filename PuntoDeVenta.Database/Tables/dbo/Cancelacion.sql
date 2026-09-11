CREATE TABLE [dbo].[Cancelacion]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [SucursalId] BIGINT NOT NULL,
    [VentaId] BIGINT NOT NULL,
    [Tipo] NVARCHAR (10) NOT NULL,
    [Motivo] NVARCHAR (300) NULL,
    [UsuarioCancela] NVARCHAR (25) NULL,
    [UsuarioAutoriza] NVARCHAR (25) NULL,
    [FechaCancelacion] DATETIME CONSTRAINT [DF_Cancelacion_FechaCancelacion] DEFAULT (GETDATE()) NOT NULL,
    [TotalReembolsado] DECIMAL (18, 4) CONSTRAINT [DF_Cancelacion_TotalReembolsado] DEFAULT ((0)) NOT NULL,
    [MetodoReembolso] NVARCHAR (20) NULL,
    [Estatus] BIT CONSTRAINT [DF_Cancelacion_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_Cancelacion_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_Cancelacion] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Cancelacion_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [FK_Cancelacion_Sucursal] FOREIGN KEY ([SucursalId]) REFERENCES [dbo].[Sucursal] ([Id]),
    CONSTRAINT [FK_Cancelacion_Venta] FOREIGN KEY ([VentaId]) REFERENCES [dbo].[Venta] ([Id]),
    CONSTRAINT [CK_Cancelacion_Tipo] CHECK ([Tipo] IN (N'Total', N'Parcial'))
);
