#nullable disable

namespace UserManagement.Application.Dtos;

public class ApiResponse<T>
{
    public bool Success { get; set; }

    public string Message { get; set; }

    public List<ValidationError> Errors { get; set; }

    public T Data { get; set; }


}

public class ValidationError
{
    public string PropertyName { get; set; }
    public string ErrorMessage { get; set; }
}