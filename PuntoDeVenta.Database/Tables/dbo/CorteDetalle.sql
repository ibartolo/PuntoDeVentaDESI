CREATE TABLE [dbo].[CorteDetalle]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [CorteId] BIGINT NOT NULL,
    [MetodoPago] NVARCHAR (20) NOT NULL,
    [Monto] DECIMAL (18, 4) CONSTRAINT [DF_CorteDetalle_Monto] DEFAULT ((0)) NOT NULL,
    [FolioCobro] NVARCHAR (100) NULL,
    [TicketCobroUrl] NVARCHAR (300) NULL,
    [TicketSistemaUrl] NVARCHAR (300) NULL,
    [ComprobanteUrl] NVARCHAR (300) NULL,
    [Estatus] BIT CONSTRAINT [DF_CorteDetalle_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_CorteDetalle_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_CorteDetalle] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CorteDetalle_Corte] FOREIGN KEY ([CorteId]) REFERENCES [dbo].[Corte] ([Id]),
    CONSTRAINT [CK_CorteDetalle_MetodoPago] CHECK ([MetodoPago] IN (N'Efectivo', N'Tarjeta', N'Transferencia'))
);
