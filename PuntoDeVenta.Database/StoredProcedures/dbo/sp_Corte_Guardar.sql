CREATE PROCEDURE [dbo].[sp_Corte_Guardar]
    @EmpresaId BIGINT,
    @SucursalId BIGINT,
    @UsuarioId BIGINT,
    @CajaChicaId BIGINT,
    @MontoInicial DECIMAL(18,4) = 0,
    @EfectivoContado DECIMAL(18,4) = 0,
    @Observaciones NVARCHAR(MAX) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @FechaApertura DATETIME;
    DECLARE @Salidas DECIMAL(18,4) = 0;
    DECLARE @TotalEfectivo DECIMAL(18,4) = 0;
    DECLARE @TotalTarjeta DECIMAL(18,4) = 0;
    DECLARE @TotalTransferencia DECIMAL(18,4) = 0;
    DECLARE @TotalVentas DECIMAL(18,4) = 0;
    DECLARE @Esperado DECIMAL(18,4);
    DECLARE @Diferencia DECIMAL(18,4);
    DECLARE @CorteId BIGINT;

    SELECT @FechaApertura = [FechaApertura], @Salidas = [SalidasTotales]
    FROM [dbo].[CajaChica]
    WHERE [Id] = @CajaChicaId AND [EmpresaId] = @EmpresaId AND [Estado] = N'Abierta' AND [Estatus] = 1;

    IF @FechaApertura IS NULL
    BEGIN
        RAISERROR('Caja chica no encontrada o ya cerrada para corte.', 16, 1);
        RETURN;
    END;

    SELECT
        @TotalEfectivo = ISNULL(SUM(CASE WHEN [MetodoPago] = N'Efectivo' THEN [Total] ELSE 0 END), 0),
        @TotalTarjeta = ISNULL(SUM(CASE WHEN [MetodoPago] = N'Tarjeta' THEN [Total] ELSE 0 END), 0),
        @TotalTransferencia = ISNULL(SUM(CASE WHEN [MetodoPago] = N'Transferencia' THEN [Total] ELSE 0 END), 0)
    FROM [dbo].[Venta]
    WHERE [EmpresaId] = @EmpresaId
      AND [SucursalId] = @SucursalId
      AND [CajaChicaId] = @CajaChicaId
      AND [Estatus] = 1
      AND [FechaVenta] >= @FechaApertura;

    SET @TotalVentas = @TotalEfectivo + @TotalTarjeta + @TotalTransferencia;
    SET @Esperado = @MontoInicial + @TotalEfectivo - @Salidas;
    SET @Diferencia = @EfectivoContado - @Esperado;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO [dbo].[Corte]
        ([EmpresaId], [SucursalId], [UsuarioId], [CajaChicaId], [FechaCorte], [MontoInicial],
         [TotalVentas], [TotalEfectivo], [TotalTarjeta], [TotalTransferencia], [Salidas],
         [EfectivoEsperado], [EfectivoContado], [Diferencia], [Observaciones],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        VALUES
        (@EmpresaId, @SucursalId, @UsuarioId, @CajaChicaId, GETDATE(), @MontoInicial,
         @TotalVentas, @TotalEfectivo, @TotalTarjeta, @TotalTransferencia, @Salidas,
         @Esperado, @EfectivoContado, @Diferencia, @Observaciones,
         1, @Actor, GETDATE(), NULL, NULL);

        SET @CorteId = CAST(SCOPE_IDENTITY() AS BIGINT);

        INSERT INTO [dbo].[CorteDetalle]
        ([CorteId], [MetodoPago], [Monto], [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        VALUES
        (@CorteId, N'Efectivo', @TotalEfectivo, 1, @Actor, GETDATE(), NULL, NULL),
        (@CorteId, N'Tarjeta', @TotalTarjeta, 1, @Actor, GETDATE(), NULL, NULL),
        (@CorteId, N'Transferencia', @TotalTransferencia, 1, @Actor, GETDATE(), NULL, NULL);

        UPDATE [dbo].[CajaChica]
        SET [Estado] = N'Cerrada',
            [FechaCierre] = GETDATE(),
            [IngresosTotales] = @TotalVentas,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [Id] = @CajaChicaId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH

    SELECT @CorteId AS [Id];
END;
