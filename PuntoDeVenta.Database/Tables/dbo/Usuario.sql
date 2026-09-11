CREATE TABLE [dbo].[Usuario]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [NombreUsuario] NVARCHAR (25) NOT NULL,
    [ContrasenaHash] NVARCHAR (256) NOT NULL,
    [ContrasenaSalt] NVARCHAR (256) NOT NULL,
    [ContrasenaIteraciones] INT NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_Usuario_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_Usuario_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_Usuario] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Usuario_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [UQ_Usuario_NombreUsuario] UNIQUE NONCLUSTERED ([NombreUsuario] ASC),
    CONSTRAINT [CK_Usuario_NombreUsuario_NotEmpty] CHECK (LEN(LTRIM(RTRIM([NombreUsuario]))) > 0),
    CONSTRAINT [CK_Usuario_ContrasenaIteraciones_Min] CHECK ([ContrasenaIteraciones] >= 100000)
);
