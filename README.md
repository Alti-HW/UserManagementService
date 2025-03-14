# UserManagementService

## Project Overview
- ### Name
  UserManagementService
- ### Description
    The User Management Service serves as an orchestration layer even though Keycloak manages user authentication and role assignments. The User Management API provides secure authentication, authorization, and access control through modular APIs. All endpoints require authentication. These APIs act as proxy APIs that internally communicate with Keycloak's REST APIs. Keycloak is used as the IAM (Identity and Access Management) provider.
    
    Why is the .NET Microservice Needed as an Orchestration Layer?
    **Centralized API Gateway for Clients**
    Clients (Frontend, Mobile, Other Services) never directly interact with Keycloak/Okta.
    Instead, they call UMS, which acts as a gateway for:
    Authentication (via Keycloak)
    User profile management
    Fetching role-based permissions

  **Key Components**

  - **Authentication API:** Manages user login and logout using access and refresh tokens.
  - **SSO API:** Supports Single Sign-On authentication via external providers.
  - **Users API:** Handles user records (create, retrieve, update, delete) and user invitations.
  - **Role Management API:** Manages role creation, retrieval, updates, and deletion, including composite roles.
  - **Role Mapping API:** Handles user role assignments (assign/unassign roles).
  - **Permission API:** Manages user permissions (create, retrieve, delete permissions).
  
## Technology Stack
| Category       | Technology Used                                                                                      |
|----------------|------------------------------------------------------------------------------------------------------|
| Backend        | .NET 8, RestSharp                                                                                    |
| Database       | PostgreSQL                                                                                           |
| IAM Provider   | Keycloak                                                                                             |
| Packages       | **Authentication:** Microsoft.AspNetCore.Authentication.JwtBearer                                    |
|                | **API Documentation:** Swashbuckle.AspNetCore, Swashbuckle.AspNetCore.Annotations, Swashbuckle.AspNetCore.Filters |
|                | **Object Mapping:** AutoMapper                                                                       |
|                | **Validation:** FluentValidation, FluentValidation.AspNetCore                                        |
|                | **Dependency Injection:** Microsoft.Extensions.DependencyInjection                                   |
|                | **Options Pattern:** Microsoft.Extensions.Options                                                    |
|                | **Security Tokens:** Microsoft.IdentityModel.Tokens, System.IdentityModel.Tokens.Jwt                 |
|                | **JSON Handling:** Newtonsoft.Json                                                                   |
|                | **Email Handling:** NETCore.MailKit                                                                  |

## Contribution Guidelines  
 
- **Pull Requests:** Please refer to the following document for the pull request process when making any changes: [Pull Request Process](https://github.com/Alti-HW/UserManagementService/blob/master/Docs/Pull%20Requests%20Process.md).  
- **License:** MIT License.  

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

- ##### Keycloak Service

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

  ---

### LDAP and SSO Integration
  Refer to the following setup guide for a detailed understanding of how LDAP and SSO function.

  [Apache DS LDAP and SSO.pdf](https://github.com/Alti-HW/UserManagementService/blob/master/Docs/Apache%20DS%20LDAP%20and%20SSO.pdf)
  
 ---

## Testing (README_TESTS.md)

### Unit Tests
Implementation pending.

### Integration Tests
Implementation pending.

---

## Flow Diagrams

### High-Level Design (HLD)

  ![image](https://github.com/user-attachments/assets/198ed5be-7ba3-44fb-8dda-6e1f45b2ae63)

---

### Low-Level Design

  - #### Normal API flow

      ![image](https://github.com/user-attachments/assets/28cde670-c911-4a4c-9f05-225f91ddfef2)
    
---
    
  - #### Keycloak and LDAP Authentication Flow

      ![image](https://github.com/user-attachments/assets/2bad3276-6a77-4b61-bd84-5e3da0a6335e)
   
---
    
  - #### SSO Flow
    ![image](https://github.com/user-attachments/assets/c702b4de-ebba-485f-ac5b-ae72b489490f)
   
---
    


     



  




