# Permission API Documentation

## Overview
The Permission API allows managing user permissions in the system. It provides endpoints to:

- Create a new permission
- Retrieve a list of all permissions
- Delete a permission by ID

All endpoints require authentication via a Bearer Token.

## Base URL
```
http://localhost:5000/api/permissions
```

## Endpoints

### 1. Create Permission

#### Description
This endpoint allows you to create a new permission in the system. The permission consists of a name and an optional description.

#### Endpoint:
```
POST /api/permissions/create
```

#### Request:
```sh
curl -X POST "http://localhost:5000/api/permissions/create" \
-H "Authorization: Bearer <your_token>" \
-H "Content-Type: application/json" \
-d '{
    "name": "ManageUsers",
    "description": "Permission to manage user accounts"
}'
```

#### Request Body (JSON):
```json
{
    "name": "ManageUsers",
    "description": "Permission to manage user accounts"
}
```

#### Response:
| Field   | Type    | Description                              |
|---------|--------|------------------------------------------|
| success | bool   | Indicates whether the request was successful. |
| message | string | Response message.                        |
| data    | object | The created permission details.         |

#### Sample Response:
```json
{
    "success": true,
    "message": "Permission created successfully",
    "data": {
        "name": "ManageUsers",
        "description": "Permission to manage user accounts"
    }
}
```

---

### 2. Get Permissions

#### Description
This endpoint retrieves a list of all available permissions in the system.

#### Endpoint:
```
GET /api/permissions/list
```

#### Request:
```sh
curl -X GET "http://localhost:5000/api/permissions/list" \
-H "Authorization: Bearer <your_token>"
```

#### Response:
| Field   | Type    | Description                              |
|---------|--------|------------------------------------------|
| success | bool   | Indicates whether the request was successful. |
| message | string | Response message.                        |
| data    | array  | List of permissions.                     |

#### Sample Response:
```json
{
    "success": true,
    "message": "Permissions fetched successfully",
    "data": [
        {
            "name": "ManageUsers",
            "description": "Permission to manage user accounts"
        },
        {
            "name": "ViewReports",
            "description": "Permission to view reports"
        }
    ]
}
```

---

### 3. Delete Permission

#### Description
This endpoint allows you to delete a permission using its unique ID.

#### Endpoint:
```
DELETE /api/permissions/delete
```

#### Request:
```sh
curl -X DELETE "http://localhost:5000/api/permissions/delete?permissionId=123" \
-H "Authorization: Bearer <your_token>"
```

#### Query Parameters:
| Parameter    | Type   | Required | Description                    |
|-------------|--------|----------|--------------------------------|
| permissionId | string | Yes      | ID of the permission to delete. |

#### Response:
| Field   | Type    | Description                              |
|---------|--------|------------------------------------------|
| success | bool   | Indicates whether the request was successful. |
| message | string | Response message.                        |
| data    | bool   | `true` if deleted, `false` otherwise.    |

#### Sample Response (Success):
```json
{
    "success": true,
    "message": "Permission deleted successfully.",
    "data": true
}
```

#### Sample Response (Failure):
```json
{
    "success": false,
    "message": "Permission not found or could not be deleted.",
    "data": false
}
```

---

## Model Definitions

### 1. Permission Model
This model represents a permission entity in the system.

| Field       | Type   | Required | Description                   |
|------------|--------|----------|--------------------------------|
| name       | string | Yes      | Name of the permission.       |
| description | string | No      | Description of the permission. |

### 2. ApiResponse Model
This model represents the standard API response format.

| Field   | Type    | Description                              |
|---------|--------|------------------------------------------|
| success | bool   | Indicates if the request was successful. |
| message | string | Response message.                        |
| data    | object | Response data, varies per request.       |

---

## Authentication
All endpoints require authentication using a Bearer Token. Include the token in the Authorization header as follows:

```sh
-H "Authorization: Bearer <your_token>"
```

---

## Error Handling
- If an invalid or missing `permissionId` is provided in the delete request, the API returns an error.
- If a request body is missing required fields, the API returns an error with a descriptive message.
