namespace UserManagement.Application.Params;

public class ClientsFilterParams
{
    public string? ClientId { get; set; }

    public int? First { get; set; }

    public int? Max { get; set; }

    public bool? Search { get; set; }

    public bool? ViewableOnly { get; set; }
}
