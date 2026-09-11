CREATE PROCEDURE [dbo].[sp_Usuario_Listar]
    @EmpresaId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
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
    WHERE U.[EmpresaId] = @EmpresaId
      AND U.[Estatus] = 1
    ORDER BY U.[NombreUsuario] ASC;
END;
