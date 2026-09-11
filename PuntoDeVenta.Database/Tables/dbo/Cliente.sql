CREATE TABLE [dbo].[Cliente]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [Nombre] NVARCHAR (150) NOT NULL,
    [Correo] NVARCHAR (250) NULL,
    [Telefono] NVARCHAR (50) NULL,
    [Direccion] NVARCHAR (500) NULL,
    [EsPublicoGeneral] BIT CONSTRAINT [DF_Cliente_EsPublicoGeneral] DEFAULT ((0)) NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_Cliente_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_Cliente_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_Cliente] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Cliente_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [CK_Cliente_Nombre_NotEmpty] CHECK (LEN(LTRIM(RTRIM([Nombre]))) > 0),
    CONSTRAINT [CK_Cliente_Nombre_MaxLen] CHECK (LEN([Nombre]) <= 150)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_Cliente_EmpresaId_Nombre_Activa]
    ON [dbo].[Cliente]([EmpresaId] ASC, [Nombre] ASC)
    WHERE [Estatus] = 1;
