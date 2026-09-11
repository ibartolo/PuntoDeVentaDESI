CREATE PROCEDURE [dbo].[sp_Pagina_Obtener]
    @PaginaId BIGINT
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
    WHERE [Id] = @PaginaId;
END;
