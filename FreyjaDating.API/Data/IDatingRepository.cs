using System.Collections.Generic;
using System.Threading.Tasks;
using FreyjaDating.API.Models;

namespace FreyjaDating.API.Data
{
    public interface IDatingRepository
    {
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUser(int id);
        Task<Photo> GetPhoto(int id);
        Task<Photo> GetMainPhotoForUser(int userId);
    }
}