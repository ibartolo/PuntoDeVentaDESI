CREATE TABLE [dbo].[UsuarioPagina]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [UsuarioId] BIGINT NULL,
    [PaginaId] BIGINT NULL,
    [Estatus] BIT CONSTRAINT [DF_UsuarioPagina_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_UsuarioPagina_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_UsuarioPagina] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UsuarioPagina_Usuario] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuario] ([Id]),
    CONSTRAINT [FK_UsuarioPagina_Pagina] FOREIGN KEY ([PaginaId]) REFERENCES [dbo].[Pagina] ([Id])
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_UsuarioPagina_UsuarioId_PaginaId_Activo]
    ON [dbo].[UsuarioPagina]([UsuarioId] ASC, [PaginaId] ASC)
    WHERE [Estatus] = 1;
