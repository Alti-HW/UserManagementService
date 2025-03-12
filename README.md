# UserManagementService

## Project Overview
The User Management API provides secure authentication, authorization, and access control through modular APIs. All endpoints require authentication. These APIs act as proxy APIs that internally communicate with Keycloak's REST APIs.

Keycloak is used as the IAM (Identity and Access Management) provider.

**Key Components**

- **Authentication API:** Manages user login and logout using access and refresh tokens.
- **SSO API:** Supports Single Sign-On authentication via external providers.
- **Users API:** Handles user records (create, retrieve, update, delete) and user invitations.
- **Role Management API:** Manages role creation, retrieval, updates, and deletion, including composite roles.
- **Role Mapping API:** Handles user role assignments (assign/unassign roles).
- **Permission API:** Manages user permissions (create, retrieve, delete permissions).

## Installation Steps
