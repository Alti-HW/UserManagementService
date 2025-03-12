# UserManagementService

## Project Overview
- ### Name UserManagement.Api
- ### Description
  The User Management API provides secure authentication, authorization, and access control through modular APIs. All endpoints require authentication. These APIs act as proxy APIs that internally communicate   with Keycloak's REST APIs.

  Keycloak is used as the IAM (Identity and Access Management) provider.

  **Key Components**

  - **Authentication API:** Manages user login and logout using access and refresh tokens.
  - **SSO API:** Supports Single Sign-On authentication via external providers.
  - **Users API:** Handles user records (create, retrieve, update, delete) and user invitations.
  - **Role Management API:** Manages role creation, retrieval, updates, and deletion, including composite roles.
  - **Role Mapping API:** Handles user role assignments (assign/unassign roles).
  - **Permission API:** Manages user permissions (create, retrieve, delete permissions).
  
- ### Technology Stack
    | Category          | Technology Used                                                                                         |
    |------------------|------------------------------------------------------------------------------------------------------|
    | Backend         | .NET 8, RestSharp                                                                                      |
    | Database       | PostgreSQL                                                                                            |
    | IAM Provider   | Keycloak                                                                                               |
    | Packages       | **Authentication:** Microsoft.AspNetCore.Authentication.JwtBearer  |
    |                | **API Documentation:** Swashbuckle.AspNetCore, Swashbuckle.AspNetCore.Annotations, Swashbuckle.AspNetCore.Filters |
    |                | **Object Mapping:** AutoMapper |
    |                | **Validation:** FluentValidation, FluentValidation.AspNetCore |
    |                | **Dependency Injection:** Microsoft.Extensions.DependencyInjection |
    |                | **Options Pattern:** Microsoft.Extensions.Options |
    |                | **Security Tokens:** Microsoft.IdentityModel.Tokens, System.IdentityModel.Tokens.Jwt |
    |                | **JSON Handling:** Newtonsoft.Json |
    |                | **Email Handling:** NETCore.MailKit |


## Installation Steps  
#### **1. Clone the repository** 
  ```
  git clone <repository_url>
  cd <repository_name>
  ```

#### **2. Install Required Tools**  

- **.NET 8 SDK** → [Download](https://dotnet.microsoft.com/en-us/download)  
  - Verify installation:  
    ```sh
    dotnet --version
    ```
  
- **PostgreSQL** → [Download](https://www.postgresql.org/download/)  
  - Verify installation:  
    ```sh
    psql -U postgres
    ```

- **Keycloak**  
  - Use the following custom URL to install Keycloak:  
    [Keycloak Installation Guide](https://github.com/Alti-HW/IdentityAndAccessManagement/blob/master/README.md)

- **Development Tools**  
  - Visual Studio 2022+ / VS Code (with C# extension)  
  - Postman / Swagger (for API testing)  
  - Docker (if containerizing the application)  

---

#### **3. Configure the Database**  

- **Create a PostgreSQL database**:  
  ```sql
  CREATE DATABASE my_database;
  ```

- **Install Entity Framework Core (if using EF Core)**:  
  ```sh
  dotnet tool install --global dotnet-ef
  dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
  ```
---

#### **4. Configure Keycloak**  
  Use this [guide](https://github.com/Alti-HW/IdentityAndAccessManagement/blob/master/KeycloakConfiguration.md) to configure keycloak


#### **5. Environment Configuration**
  Ensure that the following settings are configured in `app.settings`:

- ##### - Keycloak Service

    | Key      |Value     |
    |-------------|-------------------------------------------------|
    | Realm       | Alti-EMS     |
    | ClientId    | EMS          |
    | ClientSecret | q66B31Pde4NfC9LNM6EULSGOzfRJj0Re   |
    | TokenUrl    | http://localhost:8080/realms/Alti-EMS/protocol/openid-connect/token   |
    | ServerUrl   | http://localhost:8080   |
    | Username    | mvgokul@altimetrik.com   |
    | Password    | 123          |
    | RedirectUri | http://localhost:5000/api/sso/callback  |

- ##### EMS UI

    | Key | Value      |
    |---------|---------------------------------------------------|
    | BaseUrl | http://localhost:3000  |

- ##### SSO Providers Hint Path

    | Provider | Key     |
    |----------|--------|
    | Google   | google |
    | GitHub   | github |

#### **6. Run the Project**

- ##### Start the Project

  ```sh
  dotnet run
  ```
- Test the API Endpoints using Postman or Swagger.

## API Documentation

  | Feature             | URL/Instructions                        |
  |---------------------|----------------------------------------|
  | Swagger UI         | [http://localhost:5000/swagger/index.html](http://localhost:5000/swagger/index.html) |
  | Postman Collection | [User Management Service.postman_collection](https://github.com/Alti-HW/UserManagementService/blob/master/Postman%20Collection/User%20Management%20Service.postman_collection)     |
  | Postman Environment | [UserManagement Service Dev.postman_environment](https://github.com/Alti-HW/UserManagementService/blob/master/Postman%20Collection/UserManagement%20Service%20Dev.postman_environment)  |

  Please find detailed API documentation below
  - [Authentication API](https://github.com/Alti-HW/UserManagementService/blob/master/Docs/AuthenticationAPI.md)
  - [Permission API](https://github.com/Alti-HW/UserManagementService/blob/master/Docs/PermissionAPI.md)
  - [Role Management API](https://github.com/Alti-HW/UserManagementService/blob/master/Docs/RoleManagementAPI.md)
  - [Role Mapping API](https://github.com/Alti-HW/UserManagementService/blob/master/Docs/RoleMappingAPI.md)
  - [Single Sign-On (SSO) API](https://github.com/Alti-HW/UserManagementService/blob/master/Docs/SingleSignOnAPI.md)
  - [User API](https://github.com/Alti-HW/UserManagementService/blob/master/Docs/UsersAPI.md)

### Import Postman
  Use the following Postman setup guide to import the collection and environment into your local Postman:

  [Postman Setup Guide](https://github.com/Alti-HW/UserManagementService/blob/master/Docs/PostmanSetupGuide.md)

  




