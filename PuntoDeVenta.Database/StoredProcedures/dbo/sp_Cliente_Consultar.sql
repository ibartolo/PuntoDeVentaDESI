CREATE PROCEDURE [dbo].[sp_Cliente_Consultar]
    @EmpresaId BIGINT,
    @ClienteId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [EmpresaId],
        [Nombre],
        [Correo],
        [Telefono],
        [Direccion],
        [EsPublicoGeneral],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion]
    FROM [dbo].[Cliente]
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @ClienteId;
END;
