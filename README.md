# CoursePlatform API

## Descripción
Este proyecto es una API robusta para la gestión de cursos y lecciones, construida siguiendo los principios de **Arquitectura Hexagonal** (Clean Architecture). La API implementa funcionalidades avanzadas de autenticación mediante JWT y Refresh Tokens, gestión de contenidos (CRUD), y documentación interactiva con Swagger.

## Docker

El proyecto incluye un `Dockerfile` para facilitar su despliegue en contenedores.

### Construir la imagen
```bash
docker build -t course-platform-api .
```

### Ejecutar el contenedor
```bash
docker run -d -p 8080:8080 --name course-api --env-file CoursePlatform.API/.env course-platform-api
```

## Características Principales
- **Arquitectura Hexagonal**: Separación clara de responsabilidades entre Dominio, Aplicación, Infraestructura y API.
- **Autenticación Segura**: Implementación de JWT con tokens de acceso de corta duración y Refresh Tokens persistidos.
- **Gestión de Cursos**: CRUD completo con soporte para borrado lógico (Soft Delete) y estados de publicación.
- **Gestión de Lecciones**: Ordenamiento inteligente de lecciones dentro de cursos con validaciones de unicidad.
- **Documentación API**: Swagger UI configurado y funcional en la raíz del proyecto.
- **Base de Datos**: MySQL con Entity Framework Core (Code First).

---

## Requisitos Previos
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [MySQL](https://www.mysql.com/downloads/)
- [Git](https://git-scm.com/)

---

## Configuración e Instalación

1. **Clonar el repositorio**:
   ```bash
   git clone https://github.com/JMoralesNunez/AssessmentTecnico.git
   cd CoursePlatform
   ```

2. **Configurar el entorno**:
   Crea un archivo `.env` en la raíz de `CoursePlatform.API/` usando como base el archivo `.env.example`.
   ```env
   # Database
   ConnectionStrings__DefaultConnection="server=localhost;port=3306;database=CoursePlatform;user=root;password=tu_password"

   # JWT Settings
   JwtSettings__SecretKey="UnaClaveSecretaMuyLargaYSeguraDeAlMenos32Caracteres"
   JwtSettings__Issuer="CoursePlatform"
   JwtSettings__Audience="CoursePlatform"
   ```

3. **Restaurar dependencias**:
   ```bash
   dotnet restore
   ```

4. **Aplicar migraciones**:
   El proyecto está configurado para aplicar las migraciones automáticamente al iniciar. No obstante, puedes hacerlo manualmente:
   ```bash
   dotnet ef database update --project CoursePlatform.Infrastructure --startup-project CoursePlatform.API
   ```

---

## Cómo Correr el Proyecto

1. **Desde la terminal**:
   ```bash
   dotnet run --project CoursePlatform.API
   ```

2. **Acceder a la documentación (Swagger)**:
   Una vez el proyecto esté corriendo, abre tu navegador en:
   [http://localhost:5028/index.html](http://localhost:5028/index.html)

---

## Estructura del Proyecto

El proyecto sigue una estructura de capas para asegurar la mantenibilidad y escalabilidad:

- **CoursePlatform.Domain**: Contiene las entidades de negocio, enums e interfaces de repositorio. Es el núcleo del sistema y no depende de ninguna otra capa.
- **CoursePlatform.Application**: Contiene la lógica de negocio, servicios e interfaces. Define los DTOs (Data Transfer Objects) para la comunicación.
- **CoursePlatform.Infrastructure**: Implementación de la persistencia de datos (EF Core, Repositorios) y configuraciones externas.
- **CoursePlatform.API**: Capa de entrada que contiene los controladores, configuración de middlewares, autenticación y Swagger.

---

## Endpoints Principales

### Autenticación (`/api/auth`)
- `POST /api/auth/register`: Registra un nuevo usuario.
- `POST /api/auth/login`: Inicia sesión y devuelve Access Token + Refresh Token.
- `POST /api/auth/refresh`: Renueva el Access Token usando un Refresh Token válido.
- `POST /api/auth/logout`: Invalida el Refresh Token para cerrar la sesión.

### Cursos (`/api/courses`)
- `GET /api/courses/search`: Búsqueda paginada por título y estado.
- `GET /api/courses/{id}/summary`: Obtiene un resumen del curso y sus lecciones.
- `POST /api/courses`: Crea un nuevo curso.
- `PUT /api/courses/{id}`: Actualiza la información de un curso.
- `DELETE /api/courses/{id}`: Borrado lógico de un curso.
- `PATCH /api/courses/{id}/publish`: Cambia el estado a Publicado.
- `PATCH /api/courses/{id}/unpublish`: Cambia el estado a Borrador.

### Lecciones (`/api/lessons`)
- `GET /api/lessons/course/{courseId}`: Lista lecciones de un curso específico.
- `POST /api/lessons/course/{courseId}`: Agrega una lección a un curso.
- `PUT /api/lessons/{id}`: Actualiza una lección.
- `DELETE /api/lessons/{id}`: Elimina (borrado lógico) una lección.
- `POST /api/lessons/course/{courseId}/reorder`: Reordena masivamente las lecciones de un curso.

---

## Seguridad y Flujo de Autenticación

1. El cliente envía credenciales a `/login`.
2. El servidor responde con un `Token` (JWT - 15 min exp) y un `RefreshToken` (7 días exp).
3. El cliente usa el `Token` en el header `Authorization: Bearer <token>` para peticiones protegidas.
4. Cuando el `Token` expira, el cliente llama a `/refresh` enviando el `Token` expirado y el `RefreshToken`.
5. Si el `RefreshToken` es válido y no ha sido usado ni revocado, se emite un nuevo par de tokens.
6. Al llamar a `/logout`, el `RefreshToken` se marca como revocado en la base de datos.

---

## Pruebas
Para ejecutar las pruebas unitarias:
```bash
dotnet test
```
Las pruebas cubren las reglas de negocio críticas, como la unicidad del orden de las lecciones y el comportamiento del borrado lógico.
