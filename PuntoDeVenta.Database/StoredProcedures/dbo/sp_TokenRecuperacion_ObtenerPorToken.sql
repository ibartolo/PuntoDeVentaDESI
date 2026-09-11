CREATE PROCEDURE [dbo].[sp_TokenRecuperacion_ObtenerPorToken]
    @Token NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        T.[Id],
        T.[UsuarioId],
        T.[Token],
        T.[FechaExpiracion],
        T.[Usado],
        T.[Estatus],
        T.[CreadoPor],
        T.[FechaCreacion],
        T.[ModificadoPor],
        T.[FechaModificacion],
        U.[NombreUsuario] AS [NombreUsuario]
    FROM [dbo].[TokenRecuperacion] T
    INNER JOIN [dbo].[Usuario] U
        ON T.[UsuarioId] = U.[Id]
    WHERE T.[Token] = @Token
      AND T.[Estatus] = 1
      AND T.[Usado] = 0
      AND T.[FechaExpiracion] >= GETDATE();
END;
