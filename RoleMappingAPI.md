# Role Mapping API Documentation

## Overview
The Role Mapping API allows you to manage user role assignments, including retrieving available roles, assigned roles, assigning new roles, and unassigning existing roles.

## Base URL
```plaintext
http://localhost:5000/api/RoleMapping
```

## Endpoints

### 1. Get Available Client Roles
**Description**: Fetches the available client roles for a given user ID.

**Endpoint**:
```plaintext
GET /user/{userId}/available
```

**Request Example (cURL):**
```sh
curl -X GET "http://localhost:5000/api/RoleMapping/user/{userId}/available" -H "Authorization: Bearer <token>"
```

**Response**:

| Field    | Type                                | Description                      |
|----------|-------------------------------------|----------------------------------|
| Success  | bool                                | Indicates success or failure    |
| Message  | string                              | Response message                |
| Data     | List<RoleRepresentationDto>        | List of available client roles  |

**Sample JSON Response:**
```json
{
    "Success": true,
    "Message": "Data retrieved successfully",
    "Data": [
        {
            "Id": "123",
            "Name": "Admin",
            "Description": "Administrator role",
            "Composite": false,
            "ClientRole": true
        }
    ]
}
```

### 2. Get Roles Assigned to a User
**Description**: Retrieves the roles assigned to a specific user.

**Endpoint**:
```plaintext
GET /user/{userId}
```

**Request Example (cURL):**
```sh
curl -X GET "http://localhost:5000/api/RoleMapping/user/{userId}" -H "Authorization: Bearer <token>"
```

**Response**:

| Field    | Type                              | Description                        |
|----------|-----------------------------------|------------------------------------|
| Success  | bool                              | Indicates success or failure      |
| Message  | string                            | Response message                  |
| Data     | RealmMappingsResponseDto          | Assigned roles information        |

**Sample JSON Response:**
```json
{
    "Success": true,
    "Message": "Data retrieved successfully",
    "Data": {
        "RealmMappings": [
            {
                "Id": "123",
                "Name": "User",
                "Description": "Basic user role",
                "Composite": false,
                "ClientRole": false
            }
        ],
        "ClientMappings": {}
    }
}
```

### 3. Assign Role to User
**Description**: Assigns a role to the user.

**Endpoint**:
```plaintext
POST /user
```

**Request Example (cURL):**
```sh
curl -X POST "http://localhost:5000/api/RoleMapping/user" -H "Content-Type: application/json" -H "Authorization: Bearer <token>" -d '{"UserId": "uuid-value", "RoleRepresentation": [{"Id": "role-id", "Name": "Admin"}]}'
```

**Response**:

| Field    | Type    | Description                 |
|----------|--------|-----------------------------|
| Success  | bool   | Indicates success or failure|
| Message  | string | Response message            |

**Sample JSON Response:**
```json
{
    "Success": true,
    "Message": "Role assigned successfully"
}
```

### 4. Unassign Role from User
**Description**: Removes a role from a user.

**Endpoint**:
```plaintext
DELETE /user
```

**Request Example (cURL):**
```sh
curl -X DELETE "http://localhost:5000/api/RoleMapping/user" -H "Content-Type: application/json" -H "Authorization: Bearer <token>" -d '{"UserId": "uuid-value", "RoleRepresentation": [{"Id": "role-id", "Name": "Admin"}]}'
```

**Response**:

| Field    | Type    | Description                 |
|----------|--------|-----------------------------|
| Success  | bool   | Indicates success or failure|
| Message  | string | Response message            |

**Sample JSON Response:**
```json
{
    "Success": true,
    "Message": "Role unassigned successfully"
}
```

## Models

### 1. RoleRepresentationDto
| Field        | Type    | Description             | Required |
|-------------|--------|-------------------------|----------|
| Id          | string | Unique role identifier  | Yes      |
| Name        | string | Name of the role        | Yes      |
| Description | string | Role description        | No       |
| Composite   | bool?  | Indicates composite role| No       |
| ClientRole  | bool?  | Indicates client role   | No       |

### 2. RealmMappingsResponseDto
| Field            | Type                                      | Description                  | Required |
|----------------|--------------------------------|--------------------------|----------|
| RealmMappings | List<RoleRepresentationDto>    | List of assigned roles   | Yes      |
| ClientMappings | Dictionary<string, ClientMappingsRepresentationDto> | Client roles mapping | No       |

### 3. ClientMappingsRepresentationDto
| Field    | Type                      | Description               | Required |
|----------|--------------------------|---------------------------|----------|
| Id       | string                   | Unique client ID         | Yes      |
| Client   | string                   | Client name              | Yes      |
| Mappings | List<RoleRepresentationDto> | List of role mappings   | No       |

### 4. UserRoleRepresentationDto
| Field             | Type                               | Description            | Required |
|------------------|---------------------------------|------------------------|----------|
| UserId          | Guid                             | ID of the user        | Yes      |
| RoleRepresentation | List<RoleRepresentationDto> | Assigned roles        | Yes      |

### 5. ApiResponse<T>
| Field    | Type         | Description                | Required |
|----------|------------|----------------------------|----------|
| Success  | bool       | Indicates success/failure  | Yes      |
| Message  | string     | Response message           | Yes      |
| Errors   | List<ValidationError> | Validation errors   | No       |
| Data     | T          | Response data              | No       |

### 6. ValidationError
| Field        | Type    | Description                        | Required |
|-------------|--------|--------------------------------|----------|
| PropertyName | string | Name of the property with error | Yes      |
| ErrorMessage | string | Error message                   | Yes      |

## Authentication
All API requests must include an `Authorization` header with a valid Bearer token.

Example:
```sh
-H "Authorization: Bearer <your-token>"
