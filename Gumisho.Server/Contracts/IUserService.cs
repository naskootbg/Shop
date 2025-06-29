
namespace Backend.Contracts
{
    public interface IUserService
    { 

        Task DeleteAddress(int addressId, string user_id);
    }
}
