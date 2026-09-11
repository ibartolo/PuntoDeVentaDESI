# Autenticación login Specification

## Purpose

Permitir acceso mínimo seguro sin definir autorización funcional.

## Requirements

### Requirement: Autenticación y empresa activa

El sistema MUST autenticar credenciales válidas mediante OAuth2 y establecer FormsAuthentication. Al autenticar, MUST resolver del servidor la Empresa activa y vigente mediante la FK directa `Usuario.EmpresaId`; el token/cookie MUST portar ese `EmpresaId`. El bearer MUST usarse exclusivamente en la comunicación servidor MVC a WebApi y MUST NOT exponerse al navegador. El sistema MUST NOT aceptar EmpresaId desde formulario, URL, cookie manipulable ni request como fuente confiable. Debe rechazar credenciales inválidas o un usuario cuya única Empresa relacionada esté inactiva o vencida, sin sesión autenticada. Este change MUST NOT resolver roles, permisos ni selección de sucursal.

#### Scenario: Inicio de sesión empresarial exitoso
- GIVEN un usuario de desarrollo válido con Empresa activa y vigente
- WHEN envía sus credenciales correctas en el login
- THEN obtiene una sesión autenticada con el EmpresaId resuelto por el servidor
- AND puede acceder únicamente al catálogo de su Empresa

#### Scenario: Credenciales inválidas
- GIVEN una persona no autenticada
- WHEN envía usuario o contraseña inválidos
- THEN el acceso es rechazado
- AND no se crea sesión autenticada

#### Scenario: Empresa no habilitada
- GIVEN un usuario válido con Empresa inactiva o vencida
- WHEN envía credenciales correctas
- THEN el acceso es rechazado y no se emite token ni cookie autenticados

### Requirement: Usuario inicial de desarrollo protegido

El usuario inicial MUST crearse exclusivamente mediante script SQL de desarrollo, asociado a una Empresa semilla activa y vigente. La contraseña MUST almacenarse con PBKDF2-SHA256, salt único y al menos 100000 iteraciones; MUST NOT almacenarse ni versionarse en texto plano ni como credencial productiva. El sistema MUST verificarla contra el valor almacenado.

#### Scenario: Sembrado de desarrollo seguro
- GIVEN una base de datos de desarrollo preparada
- WHEN se ejecuta el script de usuario inicial
- THEN se crea un usuario asociado a la Empresa semilla
- AND su contraseña se almacena con PBKDF2-SHA256, salt único y al menos 100000 iteraciones
