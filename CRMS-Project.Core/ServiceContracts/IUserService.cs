namespace CRMS_Project.Core.ServiceContracts
{
    public interface IUserService
    {
        Guid GetUserId();
        string GetUserRole();
    }
}