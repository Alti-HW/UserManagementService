# Role Management API Documentation

## Overview

The Role Management API allows users to create, retrieve, update, and delete roles. It supports operations for managing composite roles and updating role permissions.

---

## Base URL
```
http://localhost:5000/api/roles
```

## Endpoints

### 1. Create Role

**Endpoint:** `POST /api/roles/create`  
**Description:** Creates a new client role.

#### Request:
```bash
curl -X POST "http://localhost:5000/api/roles/create" \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer <token>" \
     -d '{
         "Name": "Admin",
         "Description": "Administrator role",
         "CompositeRoles": []
     }'
```
##### Related Models
1. [Role Request Dto](#rolerequestdto)
2. [Client Role Dto](#clientroledto)

#### Response Fields:

| Field   | Type   | Description                  |
|---------|--------|------------------------------|
| Success | bool   | Indicates success or failure |
| Message | string | Response message             |
| Data    | T      | true if created, false otherwise                |

#### Sample Response:
```json
{
  "success": true,
  "message": "Role created successfully",
  "data": true
}
```

### 2. Get Roles

**Endpoint:** `GET /api/roles/list`  
**Description:** Retrieves a list of all client roles.

#### Request:
```bash
curl -X GET "http://localhost:5000/api/roles/list" \
     -H "Authorization: Bearer <token>"
```

#### Response Fields:
| Field    | Type   | Description                |
|----------|--------|----------------------------|
| success  | bool   | Status of the request      |
| message  | string | Response message           |
| data     | array  | List of [roles](#roleresponsedto)              |

#### Sample Response:
```json
{
  "success": true,
  "message": "Roles retrieved successfully",
  "data": [
    {
      "Id": "12345",
      "Name": "Admin",
      "Description": "Administrator role"
    },
    {
      "Id": "67890",
      "Name": "User",
      "Description": "Standard user role"
    }
  ]
}
```

### 3. Get Role by ID

**Endpoint:** `GET /api/roles/get?roleId={roleId}`  
**Description:** Retrieves a specific client role by its ID.

#### Request:
```bash
curl -X GET "http://localhost:5000/api/roles/get?roleId=12345" \
     -H "Authorization: Bearer <token>"
```

#### Response Fields:
| Field    | Type   | Description                |
|----------|--------|----------------------------|
| success  | bool   | Status of the request      |
| message  | string | Response message           |
| data     | object | [Role](https://github.com/Alti-HW/UserManagementService/new/master#3-roleresponsedto) details               |

#### Sample Response:
```json
{
  "success": true,
  "message": "Role retrieved successfully",
  "data": {
    "Id": "12345",
    "Name": "Admin",
    "Description": "Administrator role",
    "CompositeRoles": []
  }
}
```

### 4. Delete Role

**Endpoint:** `DELETE /api/roles/delete?roleId={roleId}`  
**Description:** Deletes a client role by its ID.

#### Request:
```bash
curl -X DELETE "http://localhost:5000/api/roles/delete?roleId=12345" \
     -H "Authorization: Bearer <token>"
```

#### Response Fields:
| Field    | Type   | Description                |
|----------|--------|----------------------------|
| success  | bool   | Status of the request      |
| message  | string | Response message           |
| data     | bool   | true if deleted, false otherwise |

#### Sample Response:
```json
{
  "success": true,
  "message": "Role deleted successfully",
  "data": true
}
```

### 5. Update Role Permissions

**Endpoint:** `POST /api/roles/update-rolepermissions`  
**Description:** Updates role permissions for a given role.

#### Request:
```bash
curl -X POST "http://localhost:5000/api/roles/update-rolepermissions" \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer <token>" \
     -d '{
         "Name": "Admin",
         "CompositeRoles": [
             { "Id": "123", "Name": "User", "ClientRole": true, "ContainerId": "456" }
         ]
     }'
```
##### Related Models
1. [Role Request Dto](#rolerequestdto)
2. [Client Role Dto](#clientroledto)



#### Response Fields:
| Field    | Type   | Description                |
|----------|--------|----------------------------|
| success  | bool   | Status of the request      |
| message  | string | Response message           |
| data     | bool   | true if updated, false otherwise |

#### Sample Response:
```json
{
  "success": true,
  "message": "Role permissions updated successfully",
  "data": true
}
```

## Models

### 1. RoleRequestDto
| Field          | Type   | Required | Description                    |
|---------------|--------|----------|--------------------------------|
| Name          | string | Yes      | Name of the role               |
| Description   | string | No       | Description of the role        |
| CompositeRoles | array | No       | List of child roles for composite roles |

### 2. ClientRoleDto
| Field        | Type   | Required | Description                          |
|-------------|--------|----------|--------------------------------------|
| Id          | string | Yes      | Unique ID of the client role         |
| Name        | string | Yes      | Name of the client role              |
| ClientRole  | bool   | Yes      | Specifies if this is a client role   |
| ContainerId | string | Yes      | ID of the client that owns this role |

### 3. RoleResponseDto
| Field          | Type   | Description                    |
|---------------|--------|--------------------------------|
| Id            | string | Unique ID of the role         |
| Name          | string | Name of the role              |
| Description   | string | Description of the role       |
| ContainerId   | string | ID of the container          |
| CompositeRoles | array  | List of child roles for composite roles |

### 4. ApiResponse
| Field   | Type   | Description                  |
|---------|--------|------------------------------|
| Success | bool   | Indicates success or failure |
| Message | string | Response message             |
| Data    | T      | Response data                |

## Authentication
All endpoints require authentication using a Bearer Token. Include the token in the Authorization header as follows:

```sh
-H "Authorization: Bearer <your_token>"
```

## Notes
- All API requests require an Authorization header with a valid bearer token.
- Composite roles allow roles to inherit permissions from other roles.
- Ensure that the provided role IDs are valid before making API calls.
