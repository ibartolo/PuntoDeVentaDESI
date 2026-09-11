CREATE PROCEDURE [dbo].[sp_Usuario_ConsultarPorNombreUsuario]
    @NombreUsuario NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        [Id],
        [EmpresaId],
        [NombreUsuario],
        [ContrasenaHash],
        [ContrasenaSalt],
        [ContrasenaIteraciones],
        [Correo],
        [Telefono],
        [ImagenPerfil],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion]
    FROM [dbo].[Usuario]
    WHERE [NombreUsuario] = @NombreUsuario
      AND [Estatus] = 1;
END;
