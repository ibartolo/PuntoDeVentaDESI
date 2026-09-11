CREATE TABLE [dbo].[Proveedor]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [Nombre] NVARCHAR (150) NOT NULL,
    [Contacto] NVARCHAR (150) NULL,
    [Correo] NVARCHAR (250) NULL,
    [Telefono] NVARCHAR (50) NULL,
    [Direccion] NVARCHAR (500) NULL,
    [Estatus] BIT CONSTRAINT [DF_Proveedor_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_Proveedor_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_Proveedor] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Proveedor_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [CK_Proveedor_Nombre_NotEmpty] CHECK (LEN(LTRIM(RTRIM([Nombre]))) > 0),
    CONSTRAINT [CK_Proveedor_Nombre_MaxLen] CHECK (LEN([Nombre]) <= 150)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_Proveedor_EmpresaId_Nombre_Activa]
    ON [dbo].[Proveedor]([EmpresaId] ASC, [Nombre] ASC)
    WHERE [Estatus] = 1;
