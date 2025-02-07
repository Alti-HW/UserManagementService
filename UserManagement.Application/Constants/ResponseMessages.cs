namespace UserManagement.Application.Constants;

public static class ResponseMessages
{
    public const string InvalidRequest = "Invalid request data.";
    public const string NoDataFound = "No data found for the provided criteria.";
    public const string DataRetrieved = "Data retrieved successfully.";
    public const string RecordCreated = "New {Record} created successfully.";
    public const string RecordCreationFailed = "{Record} creation failed.";
    public const string RecordUpdated = "{Record} updated successfully.";
    public const string RecordDeleted = "{Record} deleted successfully.";
    public const string RecordNotFound = "{Record} not found.";
    public const string RoleAssignmentSuccess = "Role(s) assigned to the user successfully.";
    public const string RoleAssignmentFailed = "Role(s) assignment to the user failed.";
    public const string RoleUnassignmentSuccess = "Role(s) un-assigned to the user successfully.";
    public const string RoleUnassignmentFailed = "Role(s) un-assignment to the user failed.";
}
