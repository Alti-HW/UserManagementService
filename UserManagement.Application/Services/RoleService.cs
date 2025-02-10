using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UserManagement.Application.Dtos.Role;

public class RoleService : IRoleService
{
    private readonly KeyCloakConfiguration _keycloakOptions;

    public RoleService(IOptions<KeyCloakConfiguration> keycloakOptions)
    {
        _keycloakOptions = keycloakOptions.Value;
    }

    private async Task<string> GetAccessTokenAsync()
    {
        var tokenUrl = $"{_keycloakOptions.ServerUrl}/realms/{_keycloakOptions.Realm}/protocol/openid-connect/token";

        var client = new RestClient(tokenUrl);
        var req = new RestRequest()
            .AddHeader("Content-Type", "application/x-www-form-urlencoded")
            .AddParameter("client_id", _keycloakOptions.ClientId)
            .AddParameter("client_secret", _keycloakOptions.ClientSecret)
            .AddParameter("grant_type", "client_credentials");

        var response = await client.ExecutePostAsync(req);
        if (!response.IsSuccessful) return null;

        var tokenResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
        return tokenResponse?.access_token;
    }

    private async Task<string> GetClientIdAsync()
    {
        string accessToken = await GetAccessTokenAsync();
        var clientsUrl = $"{_keycloakOptions.ServerUrl}/admin/realms/{_keycloakOptions.Realm}/clients";

        var client = new RestClient(clientsUrl);
        var req = new RestRequest().AddHeader("Authorization", $"Bearer {accessToken}");

        var response = await client.ExecuteGetAsync(req);
        if (!response.IsSuccessful) return null;

        var clients = JsonConvert.DeserializeObject<List<dynamic>>(response.Content);
        foreach (var clientObj in clients)
        {
            if (clientObj.clientId == _keycloakOptions.ClientId)
                return clientObj.id;
        }
        return null;
    }

    public async Task<bool> CreateClientRoleAsync(RoleRequestDto roleRequest)
    {
        string accessToken = await GetAccessTokenAsync();
        string clientId = await GetClientIdAsync();

        var createRoleUrl = $"{_keycloakOptions.ServerUrl}/admin/realms/{_keycloakOptions.Realm}/clients/{clientId}/roles";

        var client = new RestClient(createRoleUrl);
        var req = new RestRequest()
            .AddHeader("Authorization", $"Bearer {accessToken}")
            .AddHeader("Content-Type", "application/json")
            .AddJsonBody(roleRequest);

        var response = await client.ExecutePostAsync(req);
        return response.StatusCode == HttpStatusCode.Created;
    }


    public async Task<List<RoleResponse>> GetClientRolesAsync()
    {
        string accessToken = await GetAccessTokenAsync();
        string clientId = await GetClientIdAsync();

        var rolesUrl = $"{_keycloakOptions.ServerUrl}/admin/realms/{_keycloakOptions.Realm}/clients/{clientId}/roles";

        var client = new RestClient(rolesUrl);
        var req = new RestRequest().AddHeader("Authorization", $"Bearer {accessToken}");

        var response = await client.ExecuteGetAsync(req);
        if (!response.IsSuccessful) return null;

        var roles = JsonConvert.DeserializeObject<List<RoleResponse>>(response.Content);
        return roles;
    }
    public async Task<RoleResponse> GetClientRoleByIdAsync(string roleId)
    {
        if (string.IsNullOrEmpty(roleId))
            throw new Exception("Role ID is required.");

        string accessToken = await GetAccessTokenAsync();
        string clientId = await GetClientIdAsync();

        var rolesUrl = $"{_keycloakOptions.ServerUrl}/admin/realms/{_keycloakOptions.Realm}/clients/{clientId}/roles";

        var client = new RestClient(rolesUrl);
        var req = new RestRequest().AddHeader("Authorization", $"Bearer {accessToken}");

        var response = await client.ExecuteGetAsync(req);
        if (!response.IsSuccessful) throw new Exception("Failed to fetch roles");

        var roles = JsonConvert.DeserializeObject<List<RoleResponse>>(response.Content);

        var role = roles.FirstOrDefault(r => r.Id == roleId);
        if (role == null) throw new Exception("Role not found");

        return role;
    }


    private async Task<string> GetRoleIdByNameAsync(string roleName, string accessToken, string clientId)
    {
        var rolesUrl = $"{_keycloakOptions.ServerUrl}/admin/realms/{_keycloakOptions.Realm}/clients/{clientId}/roles/{roleName}";

        var client = new RestClient(rolesUrl);
        var req = new RestRequest().AddHeader("Authorization", $"Bearer {accessToken}");

        var response = await client.ExecuteGetAsync(req);
        if (!response.IsSuccessful) return null;

        var role = JsonConvert.DeserializeObject<RoleResponse>(response.Content);
        return role?.Id;  // Return the role ID
    }

    private async Task<string> GetRoleNameByIdAsync(string roleId, string clientId, string accessToken)
    {
        var roleUrl = $"{_keycloakOptions.ServerUrl}/admin/realms/{_keycloakOptions.Realm}/clients/{clientId}/roles-by-id/{roleId}";

        var client = new RestClient(roleUrl);
        var req = new RestRequest().AddHeader("Authorization", $"Bearer {accessToken}");

        var response = await client.ExecuteGetAsync(req);
        if (!response.IsSuccessful) return null;

        var role = JsonConvert.DeserializeObject<dynamic>(response.Content);
        return role?.name?.ToString();
    }

    private async Task<dynamic> GetRoleDetailsByNameAsync(string roleName, string clientId, string accessToken)
    {
        var roleUrl = $"{_keycloakOptions.ServerUrl}/admin/realms/{_keycloakOptions.Realm}/clients/{clientId}/roles/{roleName}";

        var client = new RestClient(roleUrl);
        var req = new RestRequest().AddHeader("Authorization", $"Bearer {accessToken}");

        var response = await client.ExecuteGetAsync(req);
        if (!response.IsSuccessful) return null;

        return JsonConvert.DeserializeObject<dynamic>(response.Content);
    }


    public async Task<bool> UpdateClientRoleAsync(UpdateRoleRequestDto updateRoleRequest)
    {
        if (string.IsNullOrEmpty(updateRoleRequest.Id))
            throw new Exception("Role ID is required.");

        if (string.IsNullOrEmpty(updateRoleRequest.NewRoleName))
            throw new Exception("New role name is required.");

        string accessToken = await GetAccessTokenAsync();
        string clientId = await GetClientIdAsync();

        // Fetch existing role details (to make sure it exists)
        var existingRole = await GetClientRoleByIdAsync(updateRoleRequest.Id);
        if (existingRole == null)
            throw new Exception("Role not found in Keycloak.");

        // Prepare the update request
        var updateRoleUrl = $"{_keycloakOptions.ServerUrl}/admin/realms/{_keycloakOptions.Realm}/clients/{clientId}/roles";

        var client = new RestClient(updateRoleUrl);
        var req = new RestRequest()
            .AddHeader("Authorization", $"Bearer {accessToken}")
            .AddHeader("Content-Type", "application/json")
            .AddJsonBody(new
            {
                id=updateRoleRequest.Id,
                name = updateRoleRequest.NewRoleName,  // Update only necessary fields
                description = updateRoleRequest.Description
            });

        var response = await client.ExecutePutAsync(req);

        // Return true if updated successfully, false otherwise
        return response.StatusCode == HttpStatusCode.NoContent;
    }

    public async Task<bool> DeleteClientRoleAsync(string roleId)
    {
        if (string.IsNullOrEmpty(roleId))
            throw new Exception("Role ID is required for deletion.");

        string accessToken = await GetAccessTokenAsync();
        string clientId = await GetClientIdAsync();

        // Get all roles and find the role name using the role ID
        var rolesUrl = $"{_keycloakOptions.ServerUrl}/admin/realms/{_keycloakOptions.Realm}/clients/{clientId}/roles";

        var client = new RestClient(rolesUrl);
        var req = new RestRequest().AddHeader("Authorization", $"Bearer {accessToken}");

        var response = await client.ExecuteGetAsync(req);
        if (!response.IsSuccessful) throw new Exception("Failed to fetch roles from Keycloak.");

        var roles = JsonConvert.DeserializeObject<List<RoleResponse>>(response.Content);
        var role = roles.FirstOrDefault(r => r.Id == roleId);

        if (role == null) throw new Exception("Role not found.");

        // Delete role using the retrieved role name
        var deleteRoleUrl = $"{_keycloakOptions.ServerUrl}/admin/realms/{_keycloakOptions.Realm}/clients/{clientId}/roles/{role.Name}";

        var deleteClient = new RestClient(deleteRoleUrl);
        var deleteReq = new RestRequest().AddHeader("Authorization", $"Bearer {accessToken}");

        var deleteResponse = await deleteClient.ExecuteDeleteAsync(deleteReq);
        return deleteResponse.StatusCode == HttpStatusCode.NoContent;
    }

}
