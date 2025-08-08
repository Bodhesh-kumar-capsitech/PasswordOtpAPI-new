using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PasswordOtpAPI.Models;
using PasswordOtpAPI.Services;
using PasswordOtpAPI.Settings;
using PasswordOtpAPI.DTOs;
using PasswordOtpAPI.Helpers;

namespace PasswordOtpAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UserServices _user;

    private string Getuseridfromjwt()
    {
        return User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    }
    public UsersController(UserServices user)
    {
        _user = user;
    }

    [HttpGet]
    public async Task<Apiresponse<List<Data>>> GetAll()
    {
        var res = new Apiresponse<List<Data>>();
        var userid = Getuseridfromjwt();
        try
        {
            var users = await _user.GetAll(userid);
            res.Message = "Users fetched sucessfully:";
            res.Status = true;
            res.Result = users;
        }
        catch (Exception e)
        {
            res.Message = "Error:" + e.Message;
            res.Status = false;
        }
        return res;

    }

    [HttpGet("{id}")]
    public async Task<Apiresponse<object>> GetById(string id)
    {
        var userId = Getuseridfromjwt();
        var res = new Apiresponse<object>();
        try
        {
            var user = await _user.GetById(id,userId);
            res.Message = "User found successfully:";
            res.Status = true;
            res.Result = user;
        }

        catch(Exception e)
        {
            res.Message = "Error:" + e.Message;
            res.Status = false;
        }

        return res;
        
    }
    [HttpPost("Add")]
    public async Task<Apiresponse<Data>> Post([FromBody] Data task)
    {
        var res = new Apiresponse<Data>();
        var userId = Getuseridfromjwt();

        try
        {
            var existing = await _user.GetbyTaskname(task.Taskname,userId);
            if (existing!=null)
            {
                res.Message = "Task already exists:";
                return res;
            }
            await _user.Add(task,userId);
            res.Message = "task added sucessfully:";
            res.Status = true;
            res.Result = task;

        }
        catch(Exception e)
        {
            res.Message = "Error:" + e.Message;
            res.Status = false;
        }
        return res;
       
    }

    [HttpPut("{id}")]
    public async Task<Apiresponse<Data>> Update([FromBody] Updateuser data, string id)
    {
        var res = new Apiresponse<Data>();
        var userId = Getuseridfromjwt();
        try
        {
            var existingname = await _user.GetbyTaskname(data.newname,userId);

            //var existingstatus = await _user.Getbystatus(data.newstatus);
            if (existingname!= null)
            {
                res.Message = "Here data is already same no need to update:";
                return res;
            }
            var update = await _user.Updatetask(id, userId, new Data { Id = id, Taskname = data.newname , Description = data.newdescription , Status = data.newstatus });
            res.Message = "Updated successfully.";
            res.Status = true;
            res.Result = update;
        }
        catch (Exception e)
        {
            res.Message = "Error: " + e.Message;
            res.Status = false;
        }
        return res;
    }



    [HttpDelete("{id}")]
    public async Task<Apiresponse<Data>> Delete(string id)
    {
        var userid = Getuseridfromjwt();
        var res = new Apiresponse<Data>();
        try
        {
            await _user.Deletetask( id ,userid );
            res.Message = "Deleted successfully:";
            res.Status = true;
            
        }
        catch(Exception e)
        {
            res.Message = "Error:" + e.Message;
            res.Status = false;
        }
        return res;
        

    }

    [HttpGet("Filter")]
    public async Task<Apiresponse<List<Data>>> Filter([FromQuery] Queryparameter query)
    {
        var userid = Getuseridfromjwt();
        var res = new Apiresponse<List<Data>>();

        try
        {
            var data = await _user.Queryparameter(query,userid);
            res.Message = "Data filtered successfully";
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
 