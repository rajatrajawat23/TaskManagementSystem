namespace TaskManagement.API.Services.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        Guid? CompanyId { get; }
        string UserRole { get; }
        string UserName { get; }
        bool IsAuthenticated { get; }
    }
}