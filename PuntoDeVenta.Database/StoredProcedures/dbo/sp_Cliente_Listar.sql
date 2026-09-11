CREATE PROCEDURE [dbo].[sp_Cliente_Listar]
    @EmpresaId BIGINT
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
      AND [Estatus] = 1
    ORDER BY [EsPublicoGeneral] DESC, [Nombre] ASC;
END;
