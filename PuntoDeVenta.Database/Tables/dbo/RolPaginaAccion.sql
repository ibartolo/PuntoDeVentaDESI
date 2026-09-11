CREATE TABLE [dbo].[RolPaginaAccion]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [RolId] BIGINT NOT NULL,
    [PaginaId] BIGINT NOT NULL,
    [PuedeLeer] BIT CONSTRAINT [DF_RolPaginaAccion_PuedeLeer] DEFAULT ((0)) NOT NULL,
    [PuedeCrear] BIT CONSTRAINT [DF_RolPaginaAccion_PuedeCrear] DEFAULT ((0)) NOT NULL,
    [PuedeEditar] BIT CONSTRAINT [DF_RolPaginaAccion_PuedeEditar] DEFAULT ((0)) NOT NULL,
    [PuedeEliminar] BIT CONSTRAINT [DF_RolPaginaAccion_PuedeEliminar] DEFAULT ((0)) NOT NULL,
    [PuedeExportar] BIT CONSTRAINT [DF_RolPaginaAccion_PuedeExportar] DEFAULT ((0)) NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_RolPaginaAccion_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_RolPaginaAccion_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_RolPaginaAccion] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_RolPaginaAccion_Rol] FOREIGN KEY ([RolId]) REFERENCES [dbo].[Rol] ([Id]),
    CONSTRAINT [FK_RolPaginaAccion_Pagina] FOREIGN KEY ([PaginaId]) REFERENCES [dbo].[Pagina] ([Id])
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_RolPaginaAccion_RolId_PaginaId_Activo]
    ON [dbo].[RolPaginaAccion]([RolId] ASC, [PaginaId] ASC)
    WHERE [Estatus] = 1;
