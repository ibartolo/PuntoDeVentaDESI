CREATE PROCEDURE [dbo].[sp_Cliente_Actualizar]
    @EmpresaId BIGINT,
    @ClienteId BIGINT,
    @Nombre NVARCHAR(150),
    @Correo NVARCHAR(250) = NULL,
    @Telefono NVARCHAR(50) = NULL,
    @Direccion NVARCHAR(500) = NULL,
    @EsPublicoGeneral BIT = 0,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Cliente]
    SET
        [Nombre] = LTRIM(RTRIM(@Nombre)),
        [Correo] = @Correo,
        [Telefono] = @Telefono,
        [Direccion] = @Direccion,
        [EsPublicoGeneral] = @EsPublicoGeneral,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @ClienteId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;
