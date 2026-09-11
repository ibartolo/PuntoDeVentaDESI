CREATE PROCEDURE [dbo].[sp_Pagina_Listar]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [Nombre],
        [NombreVisible],
        [Descripcion],
        [Tipo],
        [Direccion],
        [PermisosPadreId],
        [Logo],
        [OrdenB],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion]
    FROM [dbo].[Pagina]
    WHERE [Estatus] = 1
    ORDER BY [OrdenB] ASC, [Nombre] ASC;
END;
