namespace Mango.Services.AuthApi.Models.DTO
{
    // When new user register
    public class RegistrationRequestDto
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }

    }
}
