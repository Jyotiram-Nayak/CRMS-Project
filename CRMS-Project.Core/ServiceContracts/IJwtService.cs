namespace CRMS_Project.Core.ServiceContracts
{
    public interface IJwtService
    {
        Task<string> GenerateJWTTokenAsync(string email);
    }
}