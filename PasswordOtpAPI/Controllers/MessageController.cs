using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using PasswordOtpAPI.DTOs;
using PasswordOtpAPI.Hubs;
using PasswordOtpAPI.Models;
using PasswordOtpAPI.Services;
using PasswordOtpAPI.Settings;
using System.Security.Claims;

namespace PasswordOtpAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {

        private readonly Messageservice _messageservice;

        private readonly IHubContext<ChatHub> _hubcontext;

        private string Getuseridfromjwt()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public MessageController(IHubContext<ChatHub> hubcontext,Messageservice messageservice)
        {
            _hubcontext = hubcontext;
            _messageservice = messageservice;
        }

        [HttpGet]
        public async Task<Apiresponse<List<Message>>> GetAllMessage()
        {
            var res = new Apiresponse<List<Message>>();
            try
            {
                var data = await _messageservice.GetAll();
                res.Message = "All message found sucessfully:";

                res.Status = true;
                res.Result = data;
                
            }
            catch(Exception e)
            {
                res.Message = "Error:" + e.Message;
                res.Status = false;
            }
            return res;
        }

        [HttpGet("{id}")]
        public async Task<Apiresponse<object>> Getbyid(string id)
        {
            var res = new Apiresponse<object>();

            try
            {
                if(id! == Getuseridfromjwt())
                {
                    res.Message = "You can not saw others chat:";
                    res.Status = false;
                    return res;
                }
                var data = await _messageservice.Getbyid(id);
                res.Message = "Message found sucessfully";
                res.Status = true;
                res.Result = data;
            }
            catch(Exception e)
            {
                res.Message = "Error:" + e.Message;
                res.Status = false;
            }
            return res;
        }

        [HttpPost]
        public async Task<Apiresponse<object>> SendMessage([FromBody] Messagecontainer input)
        {
            var res = new Apiresponse<object>();
            try
            {
                var userId = Getuseridfromjwt();
                var chatdata = new Messagecontainer
                {
                    User = input.User,
                    Messages = input.Messages,
                };

                await _messageservice.AddOrUpdate(userId, chatdata);

                res.Message = "Message sent";
                res.Status = true;
                res.Result = chatdata;
            }
            catch (Exception e)
            {
                res.Message = "Error:" + e.Message;
                res.Status = false;
            }
            return res;
        }


        [HttpPost("session")]
        public async Task<Apiresponse<List<Message>>> Added()
        {
            var res = new Apiresponse<List<Message>>();
            try
            {
                var data = await _messageservice.Getbyid(Getuseridfromjwt());
                await _messageservice.Addsession(Getuseridfromjwt(),data);
                res.Message = "session fetched sucessfully";
                res.Status = true;
                res.Result = data;
            }
            catch(Exception e)
            {
                res.Message = "Error:" + e.Message;
                res.Status = false;
            }
            return res;
        }


    }
}