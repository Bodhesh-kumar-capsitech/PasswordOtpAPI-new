using MongoDB.Driver;
using PasswordOtpAPI.Helpers;
using PasswordOtpAPI.Models;
using PasswordOtpAPI.Settings;

namespace PasswordOtpAPI.Services
{
    public class AuthService
    {
        private readonly IMongoCollection<User> _users;

        public AuthService(IConfiguration config)
        {
            var settings = config.GetSection("DatabaseSettings").Get<DatabaseSettings>();
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _users = database.GetCollection<User>(settings.UsersCollectionName);
        }

        public async Task<List<User>> GetAll()
        {
            var res = await _users.Find( _=> true).ToListAsync();
            return res;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User> GetByContactNumberAsync(string contact)
        {
            return await _users.Find(u => u.ContactNumber == contact).FirstOrDefaultAsync();
        }

        public async Task RegisterAsync(User user)
        {
            await _users.InsertOneAsync(user);
        }

        public async Task<string> GenerateOtpAsync(string contact)
        {
            var user = await GetByContactNumberAsync(contact);
            if (user == null)
                throw new Exception("User not found");

            var otp = new Random().Next(100000, 999999).ToString();
            user.OtpCode = otp;
            user.OtpExpiryTime = DateTime.UtcNow.AddHours(1);

            var update = Builders<User>.Update
                .Set(u => u.OtpCode, user.OtpCode)
                .Set(u => u.OtpExpiryTime, user.OtpExpiryTime);

            await _users.UpdateOneAsync(u => u.Id == user.Id, update);

            return otp;
        }

        public async Task<bool> VerifyOtpAsync(string email, string otp, string newPassword)
        {
            var user = await GetByEmailAsync(email);
            if (user == null)
                return false;

            if (user.OtpCode != otp || user.OtpExpiryTime < DateTime.UtcNow)
                return false;

            var update = Builders<User>.Update
                .Set(u => u.PasswordHash, PasswordHasher.Hash(newPassword))
                .Set(u => u.OtpCode, null)
                .Set(u => u.OtpExpiryTime, null);

            await _users.UpdateOneAsync(u => u.Id == user.Id, update);
            return true;
        }
         
        public async Task<List<User>> Getfilterbyquery(Queryparameter query)
        {
            //logic for searching
            var filterbuilder = Builders<User>.Filter;
            var filter = filterbuilder.Empty;

            if(!string.IsNullOrEmpty(query.Search))
            {
                var searchfilter = filterbuilder.Or
                    (
                    filterbuilder.Regex(u => u.ContactNumber, new MongoDB.Bson.BsonRegularExpression(query.Search)),
                     filterbuilder.Regex(u => u.Email, new MongoDB.Bson.BsonRegularExpression(query.Search))
                    );
                filter &= searchfilter;
            }
            //logic for sorting
            var sortfilter = Builders<User>.Sort;
            var sort = query.SortDir.ToLower() == "desc" ?
                sortfilter.Descending(query.SortBy) :
            sortfilter.Ascending(query.SortBy);

            var skip = (query.Page - 1) * query.PageSize;

            return await _users.Find(filter)
                .Sort(sort)
                .Skip(skip)
                .Limit(query.PageSize)
                .ToListAsync();

        }
    }
}
