﻿#nullable disable

using UserManagement.Application.Common;

namespace UserManagement.Application.Configuration;

public class KeyCloakConfiguration
{
    public const string Section = ConfigurationConstants.KeycloakService;

    public string Realm { get; set; }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string TokenUrl { get; set; }

    public string ServerUrl { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }
}
