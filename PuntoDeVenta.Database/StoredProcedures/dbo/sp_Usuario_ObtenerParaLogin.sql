CREATE PROCEDURE [dbo].[sp_Usuario_ObtenerParaLogin]
    @NombreUsuario NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        U.[Id],
        U.[EmpresaId],
        U.[NombreUsuario],
        U.[ContrasenaHash],
        U.[ContrasenaSalt],
        U.[ContrasenaIteraciones],
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
    INNER JOIN [dbo].[Empresa] E
        ON U.[EmpresaId] = E.[Id]
    WHERE U.[NombreUsuario] = @NombreUsuario
      AND U.[Estatus] = 1
      AND E.[Estatus] = 1
      AND GETDATE() >= E.[FechaVigenciaInicio]
      AND GETDATE() <= E.[FechaVigenciaFin];
END;
