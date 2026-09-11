CREATE PROCEDURE [dbo].[sp_CajaChica_Abrir]
    @EmpresaId BIGINT,
    @SucursalId BIGINT,
    @UsuarioId BIGINT,
    @MontoInicial DECIMAL(18,4),
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    -- Sin corte previo no se puede abrir una nueva caja chica.
    IF EXISTS
    (
        SELECT 1 FROM [dbo].[CajaChica]
        WHERE [EmpresaId] = @EmpresaId
          AND [SucursalId] = @SucursalId
          AND [UsuarioId] = @UsuarioId
          AND [Estado] = N'Abierta'
          AND [Estatus] = 1
    )
    BEGIN
        RAISERROR('Ya existe una caja chica abierta para este usuario y sucursal. Realice el corte antes de abrir otra.', 16, 1);
        RETURN;
    END;

    INSERT INTO [dbo].[CajaChica]
    ([EmpresaId], [SucursalId], [UsuarioId], [MontoInicial], [IngresosTotales], [SalidasTotales],
     [FechaApertura], [FechaCierre], [Estado], [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
    VALUES
    (@EmpresaId, @SucursalId, @UsuarioId, @MontoInicial, 0, 0,
     GETDATE(), NULL, N'Abierta', 1, @Actor, GETDATE(), NULL, NULL);

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
END;
