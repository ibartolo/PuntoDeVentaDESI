CREATE TABLE [dbo].[UsuarioSucursal]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [UsuarioId] BIGINT NOT NULL,
    [SucursalId] BIGINT NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_UsuarioSucursal_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_UsuarioSucursal_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_UsuarioSucursal] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UsuarioSucursal_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [FK_UsuarioSucursal_Usuario] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuario] ([Id]),
    CONSTRAINT [FK_UsuarioSucursal_Sucursal] FOREIGN KEY ([SucursalId]) REFERENCES [dbo].[Sucursal] ([Id])
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_UsuarioSucursal_Usuario_Sucursal_Activo]
    ON [dbo].[UsuarioSucursal]([UsuarioId] ASC, [SucursalId] ASC)
    WHERE [Estatus] = 1;
