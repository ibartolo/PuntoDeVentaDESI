CREATE TABLE [dbo].[Rol]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [Nombre] NVARCHAR (50) NOT NULL,
    [Descripcion] NVARCHAR (250) NULL,
    [PuedeAutorizar] BIT CONSTRAINT [DF_Rol_PuedeAutorizar] DEFAULT ((0)) NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_Rol_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_Rol_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_Rol] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Rol_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [CK_Rol_Nombre_NotEmpty] CHECK (LEN(LTRIM(RTRIM([Nombre]))) > 0),
    CONSTRAINT [CK_Rol_Nombre_MaxLen] CHECK (LEN([Nombre]) <= 50)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_Rol_EmpresaId_Nombre_Activo]
    ON [dbo].[Rol]([EmpresaId] ASC, [Nombre] ASC)
    WHERE [Estatus] = 1;
