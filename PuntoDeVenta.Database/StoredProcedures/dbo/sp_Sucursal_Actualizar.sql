CREATE PROCEDURE [dbo].[sp_Sucursal_Actualizar]
    @EmpresaId BIGINT,
    @SucursalId BIGINT,
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(MAX) = NULL,
    @Calle NVARCHAR(200) = NULL,
    @Ciudad NVARCHAR(100) = NULL,
    @Colonia NVARCHAR(100) = NULL,
    @CodigoPostal NVARCHAR(10) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Sucursal]
    SET
        [Nombre] = LTRIM(RTRIM(@Nombre)),
        [Descripcion] = @Descripcion,
        [Calle] = @Calle,
        [Ciudad] = @Ciudad,
        [Colonia] = @Colonia,
        [CodigoPostal] = @CodigoPostal,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @SucursalId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;
