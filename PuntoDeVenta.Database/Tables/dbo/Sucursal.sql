CREATE TABLE [dbo].[Sucursal]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [Nombre] NVARCHAR (100) NOT NULL,
    [Descripcion] NVARCHAR (MAX) NULL,
    [Calle] NVARCHAR (200) NULL,
    [Ciudad] NVARCHAR (100) NULL,
    [Colonia] NVARCHAR (100) NULL,
    [CodigoPostal] NVARCHAR (10) NULL,
    [Estatus] BIT CONSTRAINT [DF_Sucursal_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_Sucursal_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_Sucursal] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Sucursal_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [CK_Sucursal_Nombre_NotEmpty] CHECK (LEN(LTRIM(RTRIM([Nombre]))) > 0),
    CONSTRAINT [CK_Sucursal_Nombre_MaxLen] CHECK (LEN([Nombre]) <= 100)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_Sucursal_EmpresaId_Nombre_Activa]
    ON [dbo].[Sucursal]([EmpresaId] ASC, [Nombre] ASC)
    WHERE [Estatus] = 1;
