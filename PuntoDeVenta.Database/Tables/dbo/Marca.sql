CREATE TABLE [dbo].[Marca]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [Nombre] NVARCHAR (100) NOT NULL,
    [Descripcion] NVARCHAR (MAX) NULL,
    [Estatus] BIT CONSTRAINT [DF_Marca_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_Marca_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_Marca] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Marca_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [CK_Marca_Nombre_NotEmpty] CHECK (LEN(LTRIM(RTRIM([Nombre]))) > 0),
    CONSTRAINT [CK_Marca_Nombre_MaxLen] CHECK (LEN([Nombre]) <= 100)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_Marca_EmpresaId_Nombre_Activa]
    ON [dbo].[Marca]([EmpresaId] ASC, [Nombre] ASC)
    WHERE [Estatus] = 1;
