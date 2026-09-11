CREATE TABLE [dbo].[TokenRecuperacion]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [UsuarioId] BIGINT NOT NULL,
    [Token] NVARCHAR (200) NOT NULL,
    [FechaExpiracion] DATETIME NOT NULL,
    [Usado] BIT CONSTRAINT [DF_TokenRecuperacion_Usado] DEFAULT ((0)) NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_TokenRecuperacion_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_TokenRecuperacion_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_TokenRecuperacion] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TokenRecuperacion_Usuario] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuario] ([Id]),
    CONSTRAINT [CK_TokenRecuperacion_Token_NotEmpty] CHECK (LEN(LTRIM(RTRIM([Token]))) > 0)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_TokenRecuperacion_Token]
    ON [dbo].[TokenRecuperacion]([Token] ASC);
