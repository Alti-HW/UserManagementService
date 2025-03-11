# User API

## Overview
The **UsersController** manages user-related operations within the User Management API. It provides endpoints for creating, retrieving, updating, and deleting user records, as well as inviting new users. All endpoints require authentication.

### Key Features:
- Retrieve a list of users with filtering options
- Create a new user
- Update an existing user
- Get user details by ID
- Delete a user by ID
- Invite a new user

## Base URL
```
/api/Users
```

## Authentication
All endpoints require authentication using a valid Bearer token.

---

## 1. Get Users
**Endpoint:** `GET /Users`

**Description:** Fetches a list of users based on provided filters.

### Request (CURL)
```sh
curl -X GET "http://localhost:5000/api/Users?First=0&Max=10" \
     -H "Authorization: Bearer YOUR_TOKEN" \
     -H "Content-Type: application/json"
```

### Response
| Status Code | Description |
|------------|-------------|
| 200 | Successful response |
| 400 | Invalid pagination values |
| 404 | No users found |

#### Sample Response (200)
```json
{
  "Success": true,
  "Message": "Data retrieved successfully",
  "Data": [
    {
      "Id": "c8b3d5ff-9fbc-4a6a-bd8b-d731a675fc2f",
      "Username": "john_doe",
      "FirstName": "John",
      "LastName": "Doe",
      "Email": "john.doe@example.com",
      "EmailVerified": true,
      "Enabled": true,
      "Created": "2024-01-01T12:00:00Z"
    }
  ]
}
```

---

## 2. Create User
**Endpoint:** `POST /`

**Description:** Creates a new user.

### Request (CURL)
```sh
curl -X POST "http://localhost:5000/api/Users" \
     -H "Authorization: Bearer YOUR_TOKEN" \
     -H "Content-Type: application/json" \
     -d '{
       "Username": "jane_doe",
       "FirstName": "Jane",
       "LastName": "Doe",
       "Email": "jane.doe@example.com",
       "EmailVerified": true,
       "Enabled": true
     }'
```

### Response
| Status Code | Description |
|------------|-------------|
| 201 | User created successfully |
| 400 | User data is required |
| 500 | Internal server error |

#### Sample Response (201)
```json
{
  "Success": true,
  "Message": "User created successfully",
  "Data": {
    "Id": "d2a1f5ff-9abc-4b6b-ad2c-d123b456fc2f",
    "Username": "jane_doe",
    "FirstName": "Jane",
    "LastName": "Doe",
    "Email": "jane.doe@example.com",
    "EmailVerified": true,
    "Enabled": true,
    "Created": "2024-01-02T10:00:00Z"
  }
}
```

---

## 3. Update User
**Endpoint:** `PUT /`

**Description:** Updates an existing user.

### Request (CURL)
```sh
curl -X PUT "http://localhost:5000/api/Users" \
     -H "Authorization: Bearer YOUR_TOKEN" \
     -H "Content-Type: application/json" \
     -d '{
       "Id": "d2a1f5ff-9abc-4b6b-ad2c-d123b456fc2f",
       "Username": "jane_doe",
       "FirstName": "Jane",
       "LastName": "Doe",
       "Email": "jane.doe@example.com",
       "EmailVerified": true,
       "Enabled": false
     }'
```

### Response
| Status Code | Description |
|------------|-------------|
| 200 | User updated successfully |
| 400 | Invalid user data |
| 404 | User not found |

#### Sample Response (200)
```json
{
  "Success": true,
  "Message": "User updated successfully"
}
```

---

## 4. Get User by ID
**Endpoint:** `GET /{userId}`

**Description:** Fetches user details by user ID.

### Request (CURL)
```sh
curl -X GET "http://localhost:5000/api/Users/d2a1f5ff-9abc-4b6b-ad2c-d123b456fc2f" \
     -H "Authorization: Bearer YOUR_TOKEN" \
     -H "Content-Type: application/json"
```

### Response
| Status Code | Description |
|------------|-------------|
| 200 | User found |
| 400 | Invalid user ID |
| 404 | User not found |

#### Sample Response (200)
```json
{
  "Success": true,
  "Message": "Data retrieved successfully",
  "Data": {
    "Id": "d2a1f5ff-9abc-4b6b-ad2c-d123b456fc2f",
    "Username": "jane_doe",
    "FirstName": "Jane",
    "LastName": "Doe",
    "Email": "jane.doe@example.com",
    "EmailVerified": true,
    "Enabled": false,
    "Created": "2024-01-02T10:00:00Z"
  }
}
```

---

## 5. Delete User
**Endpoint:** `DELETE /{userId}`

**Description:** Deletes a user by their ID.

### Request (CURL)
```sh
curl -X DELETE "http://localhost:5000/api/Users/d2a1f5ff-9abc-4b6b-ad2c-d123b456fc2f" \
     -H "Authorization: Bearer YOUR_TOKEN" \
     -H "Content-Type: application/json"
```

### Response
| Status Code | Description |
|------------|-------------|
| 200 | User deleted successfully |
| 400 | Invalid user ID |
| 404 | User not found |

#### Sample Response (200)
```json
{
  "Success": true,
  "Message": "User deleted successfully"
}
```

---

## 6. Invite User
**Endpoint:** `POST /invite`

**Description:** Sends an invite to a new user.

### Request (CURL)
```sh
curl -X POST "http://localhost:5000/api/Users/invite" \
     -H "Authorization: Bearer YOUR_TOKEN" \
     -H "Content-Type: application/json" \
     -d '{
       "Email": "new.user@example.com",
       "FirstName": "New",
       "LastName": "User"
     }'
```
#### Sample Response (200)
```json
{
  "Success": true,
  "Message": "Invitation sent successfully"
}
```

### Response
| Status Code | Description |
|------------|-------------|
| 200 | Invitation sent successfully |
| 400 | Invalid invite data |

## Model Definitions

### 1. **UserDto**
**Description:** Represents a user entity with relevant details.

| Field          | Type    | Description              | Required |
|---------------|--------|--------------------------|----------|
| Id            | UUID   | Unique identifier        | No       |
| Username      | String | User's unique username  | Yes      |
| FirstName     | String | User's first name       | Yes      |
| LastName      | String | User's last name        | Yes      |
| Email         | String | User's email address    | Yes      |
| EmailVerified | Bool   | Email verification flag | No       |
| Enabled       | Bool   | Account status          | No       |
| Created       | DateTime | Creation timestamp    | No       |

---

### 2. **UserFilterParams**
**Description:** Parameters for filtering user queries.

| Field        | Type    | Description                          | Required |
|-------------|--------|--------------------------------------|----------|
| Email       | String | User's email for filtering         | No       |
| EmailVerified | Bool | Filter based on email verification | No       |
| Enabled     | Bool   | Filter based on account status     | No       |
| Exact       | Bool   | Exact match search flag           | No       |
| First       | Int    | Offset for pagination (default 0)  | No       |
| FirstName   | String | Filter by first name               | No       |
| LastName    | String | Filter by last name                | No       |
| Max         | Int    | Limit for pagination (default 10)  | No       |
| Username    | String | Filter by username                 | No       |

---

### 3. **InviteUserDto**
**Description:** Represents the model for inviting a user.

| Field      | Type    | Description              | Required |
|-----------|--------|--------------------------|----------|
| Email     | String | Email of the invitee     | Yes      |
| FirstName | String | First name of the invitee | Yes      |
| LastName  | String | Last name of the invitee  | Yes      |
