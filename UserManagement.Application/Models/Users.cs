#nullable disable

namespace UserManagement.Application.Models;

public class Users
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool? EmailVerified { get; set; }
    public Dictionary<string, List<string>> Attributes { get; set; }
    public long? CreatedTimestamp { get; set; }
    public bool? Enabled { get; set; }
    public bool? Totp { get; set; }
    public HashSet<string> DisableableCredentialTypes { get; set; }
    public List<string> RequiredActions { get; set; }
    public int? NotBefore { get; set; } 
    public Dictionary<string, bool> Access { get; set; }
}
