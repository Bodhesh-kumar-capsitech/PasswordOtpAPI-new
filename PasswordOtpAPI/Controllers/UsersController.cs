using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PasswordOtpAPI.Models;
using PasswordOtpAPI.Services;
using PasswordOtpAPI.Settings;
using PasswordOtpAPI.DTOs;

namespace PasswordOtpAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UserServices _user;

    public UsersController(UserServices user)
    {
        _user = user;
    }

    [HttpGet]
    public async Task<Apiresponse<List<Data>>> GetAll()
    {
        var res = new Apiresponse<List<Data>>();

        try
        {
            var users = await _user.GetAll();
            res.Message = "Users fetched sucessfully:";
            res.Status = true;
            res.Result = users;
        }
        catch(Exception e)
        {
            res.Message = "Error:" + e.Message;
            res.Status = false;
        }
        return res;
        
    }

    [HttpGet("{id}")]
    public async Task<Apiresponse<object>> GetById(string id)
    {

        var res = new Apiresponse<object>();
        try
        {
            var user = await _user.GetById(id);
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
        try
        {
            var existing = await _user.GetbyTaskname(task.Taskname);
            if (existing!=null)
            {
                res.Message = "Task already exists:";
                return res;
            }
            await _user.Add(task);
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
        try
        {
            var existingname = await _user.GetbyTaskname(data.newname);
            //var existingstatus = await _user.Getbystatus(data.newstatus);
            if (existingname!= null)
            {
                res.Message = "Here data is already same no need to update:";
                return res;
            }
            var update = await _user.Updatetask(id, new Data { Id = id, Taskname = data.newname , Description = data.newdescription , Status = data.newstatus });
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
        var res = new Apiresponse<Data>();
        try
        {
            await _user.Deletetask(id);
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
        var res = new Apiresponse<List<Data>>();

        try
        {
            var data = await _user.Queryparameter(query);
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
 