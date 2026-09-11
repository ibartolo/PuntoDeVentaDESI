CREATE TABLE [dbo].[Categoria]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [CategoriaPadreId] BIGINT NULL,
    [Nombre] NVARCHAR (100) NOT NULL,
    [Descripcion] NVARCHAR (MAX) NULL,
    [Area] NVARCHAR (100) NULL,
    [Orden] INT CONSTRAINT [DF_Categoria_Orden] DEFAULT ((0)) NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_Categoria_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_Categoria_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_Categoria] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Categoria_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [FK_Categoria_CategoriaPadre] FOREIGN KEY ([CategoriaPadreId]) REFERENCES [dbo].[Categoria] ([Id]),
    CONSTRAINT [CK_Categoria_Nombre_NotEmpty] CHECK (LEN(LTRIM(RTRIM([Nombre]))) > 0),
    CONSTRAINT [CK_Categoria_Nombre_MaxLen] CHECK (LEN([Nombre]) <= 100)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_Categoria_EmpresaId_Nombre_Activa]
    ON [dbo].[Categoria]([EmpresaId] ASC, [Nombre] ASC)
    WHERE [Estatus] = 1;

GO
CREATE NONCLUSTERED INDEX [IX_Categoria_EmpresaId_CategoriaPadreId]
    ON [dbo].[Categoria]([EmpresaId] ASC, [CategoriaPadreId] ASC);
