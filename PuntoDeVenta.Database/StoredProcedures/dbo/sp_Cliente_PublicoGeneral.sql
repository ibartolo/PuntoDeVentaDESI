CREATE PROCEDURE [dbo].[sp_Cliente_PublicoGeneral]
    @EmpresaId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        [Id], [EmpresaId], [Nombre], [Correo], [Telefono], [Direccion], [EsPublicoGeneral],
        [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
    FROM [dbo].[Cliente]
    WHERE [EmpresaId] = @EmpresaId
      AND [EsPublicoGeneral] = 1
      AND [Estatus] = 1
    ORDER BY [Id] ASC;
END;
