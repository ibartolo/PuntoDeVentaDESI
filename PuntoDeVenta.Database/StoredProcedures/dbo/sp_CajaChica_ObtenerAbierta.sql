CREATE PROCEDURE [dbo].[sp_CajaChica_ObtenerAbierta]
    @EmpresaId BIGINT,
    @SucursalId BIGINT,
    @UsuarioId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        C.[Id], C.[EmpresaId], C.[SucursalId], S.[Nombre] AS [SucursalNombre],
        C.[UsuarioId], U.[NombreUsuario] AS [UsuarioNombre], C.[MontoInicial], C.[IngresosTotales],
        C.[SalidasTotales], C.[FechaApertura], C.[FechaCierre], C.[Estado],
        C.[Estatus], C.[CreadoPor], C.[FechaCreacion], C.[ModificadoPor], C.[FechaModificacion]
    FROM [dbo].[CajaChica] C
    INNER JOIN [dbo].[Sucursal] S ON S.[Id] = C.[SucursalId]
    INNER JOIN [dbo].[Usuario] U ON U.[Id] = C.[UsuarioId]
    WHERE C.[EmpresaId] = @EmpresaId
      AND C.[SucursalId] = @SucursalId
      AND C.[UsuarioId] = @UsuarioId
      AND C.[Estado] = N'Abierta'
      AND C.[Estatus] = 1
    ORDER BY C.[FechaApertura] DESC;
END;
