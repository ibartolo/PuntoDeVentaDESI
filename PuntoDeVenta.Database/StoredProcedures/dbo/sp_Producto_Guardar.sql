CREATE PROCEDURE [dbo].[sp_Producto_Guardar]
    @EmpresaId BIGINT,
    @ProductoId BIGINT = 0,
    @CategoriaId BIGINT = NULL,
    @MarcaId BIGINT = NULL,
    @MarcaTexto NVARCHAR(100) = NULL,
    @Nombre NVARCHAR(150),
    @Descripcion NVARCHAR(MAX) = NULL,
    @FotoUrl NVARCHAR(300) = NULL,
    @Codigo NVARCHAR(100) = NULL,
    @TipoProducto NVARCHAR(20) = N'Comprado',
    @UnidadMedida NVARCHAR(30) = NULL,
    @StockMinimo DECIMAL(18,4) = 0,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Empresa] WHERE [Id] = @EmpresaId AND [Estatus] = 1)
    BEGIN
        RAISERROR('Empresa no valida para operacion de Producto.', 16, 1);
        RETURN;
    END;

    IF @ProductoId IS NULL OR @ProductoId = 0
    BEGIN
        INSERT INTO [dbo].[Producto]
        ([EmpresaId], [CategoriaId], [MarcaId], [MarcaTexto], [Nombre], [Descripcion], [FotoUrl],
         [Codigo], [TipoProducto], [UnidadMedida], [StockMinimo],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        VALUES
        (@EmpresaId, @CategoriaId, @MarcaId, @MarcaTexto, LTRIM(RTRIM(@Nombre)), @Descripcion, @FotoUrl,
         @Codigo, @TipoProducto, @UnidadMedida, @StockMinimo,
         1, @Actor, GETDATE(), NULL, NULL);

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
    END
    ELSE
    BEGIN
        UPDATE [dbo].[Producto]
        SET [CategoriaId] = @CategoriaId,
            [MarcaId] = @MarcaId,
            [MarcaTexto] = @MarcaTexto,
            [Nombre] = LTRIM(RTRIM(@Nombre)),
            [Descripcion] = @Descripcion,
            [FotoUrl] = @FotoUrl,
            [Codigo] = @Codigo,
            [TipoProducto] = @TipoProducto,
            [UnidadMedida] = @UnidadMedida,
            [StockMinimo] = @StockMinimo,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [EmpresaId] = @EmpresaId
          AND [Id] = @ProductoId
          AND [Estatus] = 1;

        SELECT @ProductoId AS [Id];
    END
END;
