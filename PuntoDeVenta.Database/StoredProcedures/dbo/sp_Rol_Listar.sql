CREATE PROCEDURE [dbo].[sp_Rol_Listar]
    @Usuario NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        R.[Id],
        R.[EmpresaId],
        R.[Nombre],
        R.[Descripcion],
        R.[PuedeAutorizar],
        R.[Estatus],
        R.[CreadoPor],
        R.[FechaCreacion],
        R.[ModificadoPor],
        R.[FechaModificacion]
    FROM [dbo].[Rol] R
    INNER JOIN [dbo].[Usuario] U
        ON U.[EmpresaId] = R.[EmpresaId]
    WHERE U.[NombreUsuario] = @Usuario
      AND U.[Estatus] = 1
      AND R.[Estatus] = 1
    ORDER BY R.[Nombre] ASC;
END;
