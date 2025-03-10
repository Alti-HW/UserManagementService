#nullable disable

namespace UserManagement.Application.Configuration;

public class SsoProvidersHintPath
{
    public const string Section = nameof(SsoProvidersHintPath);

    public string Google { get; set; }
}