/*
  Script legado de compatibilidad.
  Fuente de verdad del esquema SQL: PuntoDeVenta.Database/*.sqlproj

  Para evitar duplicación de objetos SQL, este archivo referencia los archivos
  versionados del proyecto SSDT mediante SQLCMD (habilitar SQLCMD Mode en SSMS).

  Este script no contiene conexión ni credenciales.
*/

:r ..\..\PuntoDeVenta.Database\Tables\dbo\Empresa.sql
:r ..\..\PuntoDeVenta.Database\Tables\dbo\Usuario.sql
:r ..\..\PuntoDeVenta.Database\Tables\dbo\Marca.sql
:r ..\..\PuntoDeVenta.Database\StoredProcedures\dbo\sp_Usuario_ObtenerParaLogin.sql
:r ..\..\PuntoDeVenta.Database\StoredProcedures\dbo\sp_Marca_Insertar.sql
:r ..\..\PuntoDeVenta.Database\StoredProcedures\dbo\sp_Marca_Consultar.sql
:r ..\..\PuntoDeVenta.Database\StoredProcedures\dbo\sp_Marca_Listar.sql
:r ..\..\PuntoDeVenta.Database\StoredProcedures\dbo\sp_Marca_Actualizar.sql
:r ..\..\PuntoDeVenta.Database\StoredProcedures\dbo\sp_Marca_EliminarLogico.sql
