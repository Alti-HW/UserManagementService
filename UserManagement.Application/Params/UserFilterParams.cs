namespace UserManagement.Application.Params;

public class UserFilterParams
{
    public string? Email { get; set; }
    public bool? EmailVerified { get; set; }
    public bool? Enabled { get; set; } = true;
    public bool? Exact { get; set; }
    public int? First { get; set; } = 0;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int? Max { get; set; } = 10;
    public string? Username { get; set; }
}
