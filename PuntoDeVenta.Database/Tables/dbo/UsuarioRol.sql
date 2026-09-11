CREATE TABLE [dbo].[UsuarioRol]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [UsuarioId] BIGINT NOT NULL,
    [RolId] BIGINT NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_UsuarioRol_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_UsuarioRol_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_UsuarioRol] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UsuarioRol_Usuario] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuario] ([Id]),
    CONSTRAINT [FK_UsuarioRol_Rol] FOREIGN KEY ([RolId]) REFERENCES [dbo].[Rol] ([Id])
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_UsuarioRol_UsuarioId_RolId_Activo]
    ON [dbo].[UsuarioRol]([UsuarioId] ASC, [RolId] ASC)
    WHERE [Estatus] = 1;
