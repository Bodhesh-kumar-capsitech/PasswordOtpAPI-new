using Microsoft.AspNetCore.Http;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using PasswordOtpAPI.Models;
using PasswordOtpAPI.Settings;



namespace PasswordOtpAPI.Services
{
    public class Messageservice
    {
        private readonly IMongoCollection<Session> _session;

        private readonly IMongoCollection<Message> _message;
        public Messageservice(IConfiguration config)
        {
            var settings = config.GetSection("DatabaseSettings").Get<DatabaseSettings>();
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _message = database.GetCollection<Message>(settings.MessageCollectionName);
            _session = database.GetCollection<Session>(settings.SessionCollectionName);
        }

        public async Task<List<Message>> GetAll()
        {
            return await _message.Find(_ => true).ToListAsync();
        }
        
        public async Task<List<Message>> Getbyid(string id)
        {
            return await _message.Find(u => u.Id == id).ToListAsync();
        }

       
        public async Task AddOrUpdate(string userId, Messagecontainer chatdata)
        {
            var filter = Builders<Message>.Filter.Eq(u => u.Id, userId);
            var update = Builders<Message>.Update.Push(m => m.Messagecontainers, chatdata);

            var options = new UpdateOptions { IsUpsert = true };
            await _message.UpdateOneAsync(filter, update, options);
        }

        public async Task Addsession(string id,List<Message> msgs)
        {
            var session = new Session
            {
                Id = id,
                Messages = msgs
            };
            await _session.InsertOneAsync(session);
        }

        public async Task AddMessageToSessionAsync(string sessionId, Message message)
        {
            var filter = Builders<Session>.Filter.Eq(s => s.Id, sessionId);
            var update = Builders<Session>.Update.Push(s => s.Messages, message);
            await _session.UpdateOneAsync(filter, update);
        }


    }
}
