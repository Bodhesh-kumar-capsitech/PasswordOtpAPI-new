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
        public async Task<Data> GetbyTaskname(string name)
        {
            return await _data.Find(u => u.Taskname == name).FirstOrDefaultAsync();
        }
        //public async Task<Data> Getbystatus(bool status)
        //{
        //    return await _data.Find(u => u.Status == status).FirstOrDefaultAsync();
        //}

        public async Task Add(Data task)
        {
           await _data.InsertOneAsync(task);
           
        }

        public async Task<Data> Updatetask(string id, Data newdata)
        {
            // Make sure to assign the correct Id to the new data object
            newdata.Id = id;

            var result = await _data.ReplaceOneAsync(d => d.Id == id, newdata);

            if (result.MatchedCount == 0)
            {
                throw new Exception("No document found with the given ID.");
            }

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
