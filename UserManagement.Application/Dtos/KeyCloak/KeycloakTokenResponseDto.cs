namespace UserManagement.Application.Dtos.KeyCloak
{
    public class KeycloakTokenResponseDto
    {
        public string Access_Token { get; set; }
        public string Refresh_Token { get; set; }
        public int Expires_In { get; set; }
        public int Refresh_Expires_In { get; set; }
        public string Token_Type { get; set; }
    }
}
