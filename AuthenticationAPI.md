# Authentication API Documentation

## Overview
The Authentication API provides endpoints for user login and logout operations. These APIs interact with the authentication service to manage user sessions using access and refresh tokens.

## Base URL
```
http://localhost:5000/api/auth
```

## Endpoints

### 1. Login
**Description:** Authenticates a user and returns an access and refresh token.

**Endpoint:**
```
POST /api/auth/login
```

**Request:**
```bash
curl -X POST "http://localhost:5000/api/auth/login" \
     -H "Content-Type: application/json" \
     -d '{
           "Email": "user@example.com",
           "Password": "password123"
         }'
```

**Request Body:**
| Parameter         | Type   | Required | Description |
|------------------|--------|----------|-------------|
| Email           | string | Yes      | The user's email address. |
| Password        | string | Yes      | The user's password. |
| TwoFactorCode   | string | No       | Optional two-factor authentication code. |
| TwoFactorRecoveryCode | string | No | Optional recovery code for two-factor authentication. |

**Response:**
| Field              | Type   | Description |
|-------------------|--------|-------------|
| Success          | bool   | Indicates whether the request was successful. |
| Message         | string | Response message. |
| Data.Access_Token | string | The generated access token. |
| Data.Refresh_Token | string | The generated refresh token. |
| Data.Expires_In | int | Token expiration time (in seconds). |
| Data.Refresh_Expires_In | int | Refresh token expiration time (in seconds). |
| Data.Token_Type | string | Type of token issued. |

**Sample Response:**
```json
{
  "Success": true,
  "Message": "Login Successful.",
  "Data": {
    "Access_Token": "eyJhbGciOiJIUzI1...",
    "Refresh_Token": "eyJhbGciOiJIUzI1...",
    "Expires_In": 3600,
    "Refresh_Expires_In": 86400,
    "Token_Type": "Bearer"
  }
}
```

---

### 2. Logout
**Description:** Invalidates the user's refresh token to log them out.

**Endpoint:**
```
POST /api/auth/logout
```

**Request:**
```bash
curl -X POST "http://localhost:5000/api/auth/logout" \
     -H "Content-Type: application/json" \
     -d '{
           "RefreshToken": "eyJhbGciOiJIUzI1..."
         }'
```

**Request Body:**
| Parameter     | Type   | Required | Description |
|--------------|--------|----------|-------------|
| RefreshToken | string | Yes      | The refresh token to invalidate. |

**Response:**
| Field    | Type   | Description |
|---------|--------|-------------|
| Success | bool   | Indicates whether the request was successful. |
| Message | string | Response message. |
| Data    | string | Null in case of success. |

**Sample Response:**
```json
{
  "Success": true,
  "Message": "Logout successful.",
  "Data": null
}
```

---

## Models

### 1. LoginRequest
| Field                  | Type   | Required | Description |
|------------------------|--------|----------|-------------|
| Email                 | string | Yes      | The user's email address. |
| Password              | string | Yes      | The user's password. |
| TwoFactorCode         | string | No       | Optional two-factor authentication code. |
| TwoFactorRecoveryCode | string | No       | Optional recovery code for two-factor authentication. |

### 2. LogoutRequest
| Field        | Type   | Required | Description |
|-------------|--------|----------|-------------|
| RefreshToken | string | Yes      | The refresh token to invalidate. |

### 3. KeycloakTokenResponseDto
| Field              | Type   | Description |
|-------------------|--------|-------------|
| Access_Token     | string | The generated access token. |
| Refresh_Token    | string | The generated refresh token. |
| Expires_In       | int    | Token expiration time (in seconds). |
| Refresh_Expires_In | int  | Refresh token expiration time (in seconds). |
| Token_Type       | string | Type of token issued. |

### 4. ApiResponse1<T>
| Field    | Type   | Description |
|---------|--------|-------------|
| Success | bool   | Indicates if the operation was successful. |
| Message | string | Description of the response. |
| Data    | T      | The response data (varies by request). |
