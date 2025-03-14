﻿using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UserManagement.Application.Dtos;
using UserManagement.Application.Dtos.Role;
using UserManagement.Application.Models;

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

    public async Task<ApiResponse1<bool>> CreateClientRoleAsync(RoleRequestDto roleRequest, bool isCompositeRole = false)
    {
        try
        {
            string accessToken = await GetAccessTokenAsync();
            string clientId = await GetClientIdAsync();
            string createRoleUrl = $"{_keycloakOptions.ServerUrl}/admin/realms/{_keycloakOptions.Realm}/clients/{clientId}/roles";

            var client = new RestClient(createRoleUrl);
            var req = new RestRequest()
                .AddHeader("Authorization", $"Bearer {accessToken}")
                .AddHeader("Content-Type", "application/json")
                .AddJsonBody(new
                {
                    name = roleRequest.Name,
                    description = roleRequest.Description,
                    composite = isCompositeRole,
                    clientRole = true
                });

            // Step 1: Create the Role in Keycloak
            var response = await client.ExecutePostAsync(req);

            var dynamicPermissionOrRole = "Permission";

            if(isCompositeRole)
            {
                dynamicPermissionOrRole = "Role";
            }

            if (response.StatusCode != HttpStatusCode.Created)
            {
                return new ApiResponse1<bool>(
                    false,
                    $"Failed to create {dynamicPermissionOrRole}: {response.Content}",
                    false
                );
            }

            // If the role is not composite, return success
            if (!isCompositeRole)
            {
                return new ApiResponse1<bool>(true, "created successfully", true);
            }

            // Step 2: Assign Child Roles to the Composite Role (if any)
            if (roleRequest.CompositeRoles != null && roleRequest.CompositeRoles.Count > 0)
            {
                string assignCompositeUrl = $"{createRoleUrl}/{roleRequest.Name}/composites";
                var compositeRolesPayload = new List<object>();

                foreach (var childRole in roleRequest.CompositeRoles)
                {
                    compositeRolesPayload.Add(new
                    {
                        id = childRole.Id,
                        name = childRole.Name,
                        clientRole = isCompositeRole,
                        containerId = clientId
                    });
                }

                var compositeClient = new RestClient(assignCompositeUrl);
                var compositeReq = new RestRequest()
                    .AddHeader("Authorization", $"Bearer {accessToken}")
                    .AddHeader("Content-Type", "application/json")
                    .AddJsonBody(JsonConvert.SerializeObject(compositeRolesPayload));

                var compositeResponse = await compositeClient.ExecutePostAsync(compositeReq);
                if (compositeResponse.StatusCode != HttpStatusCode.NoContent)
                {
                    return new ApiResponse1<bool>(
                        false,
                        $"Failed to assign permissions: {compositeResponse.Content}",
                        false
                    );
                }
            }

            return new ApiResponse1<bool>(true, "Role created successfully", true);
        }
        catch (Exception ex)
        {
            return new ApiResponse1<bool>(
                false,
                $"An error occurred: {ex.Message}",
                false
            );
        }
    }
    public async Task<bool> UpdateCompositeRolesAsync(RoleRequestDto updateRequest)
    {
        try
        {
            string accessToken = await GetAccessTokenAsync();
            string clientId = await GetClientIdAsync();
            string baseUrl = $"{_keycloakOptions.ServerUrl}/admin/realms/{_keycloakOptions.Realm}/clients/{clientId}/roles/{updateRequest.Name}/composites";

            var client = new RestClient();

            // Step 1: Fetch currently assigned composite roles
            var getReq = new RestRequest(baseUrl, Method.Get)
                .AddHeader("Authorization", $"Bearer {accessToken}");

            var getResponse = await client.ExecuteAsync(getReq);
            if (getResponse.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine($"Error fetching current permissions: {getResponse.Content}");
                return false;
            }

            var currentCompositeRoles = JsonConvert.DeserializeObject<List<ClientRoleDto>>(getResponse.Content);
            if (currentCompositeRoles == null)
            {
                Console.WriteLine("Failed to parse current permissions.");
                return false;
            }

            // Step 2: Determine roles to add and remove
            var rolesToAdd = updateRequest.CompositeRoles
                .Where(r => !currentCompositeRoles.Any(c => c.Id == r.Id))
                .Select(r => new { id = r.Id, name = r.Name }) // Ensure correct JSON format
                .ToList();

            var rolesToRemove = currentCompositeRoles
                .Where(c => !updateRequest.CompositeRoles.Any(r => r.Id == c.Id))
                .Select(c => new { id = c.Id, name = c.Name }) // Ensure correct JSON format
                .ToList();

            Console.WriteLine($"Roles to Add: {JsonConvert.SerializeObject(rolesToAdd, Formatting.Indented)}");
            Console.WriteLine($"Roles to Remove: {JsonConvert.SerializeObject(rolesToRemove, Formatting.Indented)}");

            // Step 3: Assign new composite roles
            if (rolesToAdd.Count > 0)
            {
                var addReq = new RestRequest(baseUrl, Method.Post)
                    .AddHeader("Authorization", $"Bearer {accessToken}")
                    .AddHeader("Content-Type", "application/json")
                    .AddJsonBody(JsonConvert.SerializeObject(rolesToAdd));

                var addResponse = await client.ExecuteAsync(addReq);
                if (addResponse.StatusCode != HttpStatusCode.NoContent)
                {
                    Console.WriteLine($"Error adding composite roles: {addResponse.Content}");
                    return false;
                }
            }

            // Step 4: Remove composite roles not in the input list
            if (rolesToRemove.Count > 0)
            {
                var removeReq = new RestRequest(baseUrl, Method.Delete)
                    .AddHeader("Authorization", $"Bearer {accessToken}")
                    .AddHeader("Content-Type", "application/json")
                    .AddJsonBody(JsonConvert.SerializeObject(rolesToRemove));

                var removeResponse = await client.ExecuteAsync(removeReq);
                if (removeResponse.StatusCode != HttpStatusCode.NoContent)
                {
                    Console.WriteLine($"Error removing composite roles: {removeResponse.Content}");
                    return false;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in UpdateCompositeRolesAsync: {ex.Message}");
            return false;
        }
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

        // Fetch composite roles for each role
        foreach (var role in roles)
        {
            role.CompositeRoles = await GetCompositeRolesAsync(clientId, role.Name, accessToken);
        }

        return roles;
    }
    private async Task<List<RoleResponse>> GetCompositeRolesAsync(string clientId, string roleName, string accessToken)
    {
        var compositeRolesUrl = $"{_keycloakOptions.ServerUrl}/admin/realms/{_keycloakOptions.Realm}/clients/{clientId}/roles/{roleName}/composites";

        var client = new RestClient(compositeRolesUrl);
        var req = new RestRequest().AddHeader("Authorization", $"Bearer {accessToken}");

        var response = await client.ExecuteGetAsync(req);
        if (!response.IsSuccessful) return new List<RoleResponse>();

        return JsonConvert.DeserializeObject<List<RoleResponse>>(response.Content);
    }


    public async Task<RoleResponseDto> GetClientRoleByIdAsync(string roleId)
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

        var roles = JsonConvert.DeserializeObject<List<RoleResponseDto>>(response.Content);

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


    //public async Task<bool> UpdateClientRoleAsync(UpdateRoleRequestDto updateRoleRequest)
    //{
    //    if (string.IsNullOrEmpty(updateRoleRequest.Id))
    //        throw new Exception("Role ID is required.");

    //    if (string.IsNullOrEmpty(updateRoleRequest.NewRoleName))
    //        throw new Exception("New role name is required.");

    //    string accessToken = await GetAccessTokenAsync();
    //    string clientId = await GetClientIdAsync();

    //    // Fetch existing role details (to make sure it exists)
    //    var existingRole = await GetClientRoleByIdAsync(updateRoleRequest.Id);
    //    if (existingRole == null)
    //        throw new Exception("Role not found in Keycloak.");

    //    // Prepare the update request
    //    var updateRoleUrl = $"{_keycloakOptions.ServerUrl}/admin/realms/{_keycloakOptions.Realm}/clients/{clientId}/roles/{existingRole.Name}";

    //    var client = new RestClient(updateRoleUrl);
    //    var req = new RestRequest()
    //        .AddHeader("Authorization", $"Bearer {accessToken}")
    //        .AddHeader("Content-Type", "application/json")
    //        .AddJsonBody(new
    //        {
    //            id=updateRoleRequest.Id,
    //            name = updateRoleRequest.NewRoleName,  // Update only necessary fields
    //            description = updateRoleRequest.Description,
    //            composite = existingRole.Composite,
    //            clientRole = existingRole.ClientRole
    //        });

    //    var response = await client.ExecutePutAsync(req);

    //    // Return true if updated successfully, false otherwise
    //    return response.StatusCode == HttpStatusCode.NoContent;
    //}

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
