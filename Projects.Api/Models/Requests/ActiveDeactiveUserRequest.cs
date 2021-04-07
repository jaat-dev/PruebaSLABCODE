namespace Projects.Api.Models.Requests
{
    public class ActiveDeactiveUserRequest
    {
        public string Email { get; set; }
        public bool IsActive { get; set; }
    }
}
