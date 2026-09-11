CREATE PROCEDURE [dbo].[sp_SalidaCaja_Registrar]
    @EmpresaId BIGINT,
    @CajaChicaId BIGINT,
    @Monto DECIMAL(18,4),
    @Comentario NVARCHAR(300) = NULL,
    @Justificada BIT = 0,
    @EvidenciaUrl NVARCHAR(300) = NULL,
    @TipoSalida NVARCHAR(30) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Ingresos DECIMAL(18,4);
    DECLARE @Salidas DECIMAL(18,4);

    SELECT @Ingresos = [IngresosTotales], @Salidas = [SalidasTotales]
    FROM [dbo].[CajaChica]
    WHERE [Id] = @CajaChicaId AND [EmpresaId] = @EmpresaId AND [Estado] = N'Abierta' AND [Estatus] = 1;

    IF @Ingresos IS NULL
    BEGIN
        RAISERROR('Caja chica no encontrada o ya cerrada.', 16, 1);
        RETURN;
    END;

    -- Limite: las salidas no pueden superar el 50% de los ingresos totales.
    IF (@Salidas + @Monto) > (@Ingresos * 0.5)
    BEGIN
        RAISERROR('La salida supera el limite permitido (50%% de los ingresos totales).', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO [dbo].[SalidaCaja]
        ([EmpresaId], [CajaChicaId], [Monto], [Comentario], [FechaHora], [Justificada], [EvidenciaUrl], [TipoSalida],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        VALUES
        (@EmpresaId, @CajaChicaId, @Monto, @Comentario, GETDATE(), @Justificada, @EvidenciaUrl, @TipoSalida,
         1, @Actor, GETDATE(), NULL, NULL);

        UPDATE [dbo].[CajaChica]
        SET [SalidasTotales] = [SalidasTotales] + @Monto,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [Id] = @CajaChicaId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
END;
