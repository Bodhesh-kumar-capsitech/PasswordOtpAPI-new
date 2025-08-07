using MongoDB.Driver;
using PasswordOtpAPI.Helpers;
using PasswordOtpAPI.Models;
using PasswordOtpAPI.Settings;
using System.Text.RegularExpressions;

namespace PasswordOtpAPI.Services
{


    public class UserServices
    {
        private readonly IMongoCollection<Data> _data;

        public UserServices(IConfiguration config)
        {
            var settings = config.GetSection("DatabaseSettings").Get<DatabaseSettings>();
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _data = database.GetCollection<Data>(settings.DataCollectionName);
        }

        public async Task<List<Data>> GetAll()
        {
            return await _data.Find(_ => true).ToListAsync();
        }

        public async Task<Data> GetById(string id)
        {
            return await _data.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Data> Add(Data task)
        {
            await _data.InsertOneAsync(task);
            return task;
        }

        public async Task<Data?> Updatetask(string id, Data newdata)
        {
            var task = await _data.ReplaceOneAsync(u => u.Id == id, newdata);
            return newdata;
        }

        public async Task Deletetask(string id)
        {
            await _data.DeleteOneAsync(u => u.Id == id);
            //return "Item deleted";
        }
        public async Task<List<Data>> Queryparameter(Queryparameter query)
        {
            //For searching
            var filterbuilder = Builders<Data>.Filter;
            var filter = filterbuilder.Empty;

            if(!string.IsNullOrEmpty(query.Search))
                {
                var searchfilter = filterbuilder.Or
                    (
                    filterbuilder.Regex(u => u.Taskname, new MongoDB.Bson.BsonRegularExpression(query.Search))
                    );
                filter &= searchfilter;
                    

            }
            
            //for filter
            var sortbuilder = Builders<Data>.Sort;
            var sort = query.SortDir.ToLower() == "desc" ?

                sortbuilder.Descending(query.SortBy) : sortbuilder.Ascending(query.SortBy);

            var skip = (query.Page - 1) * query.PageSize;

            return await _data.Find(filter)
                .Sort(sort)
                .Skip(skip)
                .Limit(query.PageSize)
                .ToListAsync();

        }
    }
}
