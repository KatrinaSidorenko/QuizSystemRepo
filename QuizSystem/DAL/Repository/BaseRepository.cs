using Core.Settings;
using Microsoft.Extensions.Options;

namespace DAL.Repository
{
    public class BaseRepository
    {
        protected readonly string _connectionString;
        public BaseRepository(IOptions<ConnectionSettings> options)
        {
            _connectionString = options.Value.ConnectionString;
        }
    }
}
