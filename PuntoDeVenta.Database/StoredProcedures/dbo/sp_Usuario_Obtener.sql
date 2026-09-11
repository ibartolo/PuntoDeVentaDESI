CREATE PROCEDURE [dbo].[sp_Usuario_Obtener]
    @EmpresaId BIGINT,
    @UsuarioId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        U.[Id],
        U.[EmpresaId],
        U.[NombreUsuario],
        U.[Correo],
        U.[Telefono],
        U.[ImagenPerfil],
        U.[Estatus],
        U.[CreadoPor],
        U.[FechaCreacion],
        U.[ModificadoPor],
        U.[FechaModificacion],
        E.[NombreComercial] AS [EmpresaNombre]
    FROM [dbo].[Usuario] U
    INNER JOIN [dbo].[Empresa] E ON E.[Id] = U.[EmpresaId]
    WHERE U.[Id] = @UsuarioId
      AND U.[EmpresaId] = @EmpresaId;
END;
