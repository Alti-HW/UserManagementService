# Single Sign-On (SSO) API Documentation

## Overview
The **SSO API** provides endpoints to handle Single Sign-On authentication. It allows users to authenticate using external SSO providers and obtain access tokens for secure access to applications.

## Endpoints

### 1. SSO Login
**Endpoint:** `GET /api/sso/login`

**Description:**
Initiates the SSO (Single Sign-On) login process by redirecting the user to the authentication provider.

#### Request
```bash
curl -X GET "https://your-api.com/api/sso/login?provider=google" -H "Accept: application/json"
```

#### Query Parameters
| Parameter  | Type   | Required | Description                                      |
|------------|--------|----------|--------------------------------------------------|
| provider   | string | No       | The name of the SSO provider. Defaults if empty. |

#### Response
| Field   | Type    | Description                                         |
|---------|---------|-----------------------------------------------------|
| Success | boolean | Indicates if the request was successful.           |
| Message | string  | Error message if applicable.                        |

**Example Response:**
| Success | Message                  |
|---------|--------------------------|
| false   | Invalid redirect URL.    |

### 2. SSO Callback
**Endpoint:** `GET /api/sso/callback`

**Description:**
Handles the SSO callback by exchanging the authorization code for an access token.

#### Request
```bash
curl -X GET "https://your-api.com/api/sso/callback?code=123456&session_state=xyz&iss=google" -H "Accept: application/json"
```

#### Query Parameters
| Parameter      | Type   | Required | Description                                             |
|---------------|--------|----------|---------------------------------------------------------|
| session_state | string | No       | Session state returned by the SSO provider.            |
| iss           | string | No       | Issuer identifier indicating the SSO provider.         |
| code          | string | Yes      | Authorization code received from the SSO provider.     |

#### Response
| Field        | Type    | Description                                         |
|-------------|---------|-----------------------------------------------------|
| Access_Token  | string  | The access token to authenticate further requests. |
| Refresh_Token | string  | Token to obtain a new access token when expired.  |

**Example Response:**
Redirects the user to the client application with tokens:
```
https://your-client.com?token=abc123&refreshToken=xyz789
```

## Models

### 1. `ApiResponse<T>`
| Field   | Type    | Required | Description                              |
|---------|--------|----------|------------------------------------------|
| Success | bool   | Yes      | Indicates success or failure.           |
| Message | string | No       | Message providing additional information. |
| Data    | object | No       | Generic data object.                     |

### 2. `TokenResponse`
| Field         | Type   | Required | Description                         |
|--------------|--------|----------|-------------------------------------|
| Access_Token  | string | Yes      | The access token for authentication. |
| Refresh_Token | string | Yes      | The refresh token for new access tokens. |

## Notes
- The `login` endpoint redirects to the authentication provider, and the user must complete authentication before returning.
- The `callback` endpoint exchanges an authorization code for tokens and redirects the user accordingly.
- Ensure to handle errors and invalid authorization codes gracefully.
