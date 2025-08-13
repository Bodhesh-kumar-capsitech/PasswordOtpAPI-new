using Microsoft.AspNetCore.Mvc;
using PasswordOtpAPI.DTOs;
using PasswordOtpAPI.Helpers;
using PasswordOtpAPI.Models;
using PasswordOtpAPI.Services;

namespace PasswordOtpAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly JwtService _jwtService;

    public AuthController(AuthService authService, JwtService jwtService)
    {
        _authService = authService;
        _jwtService = jwtService;
    }

    [HttpGet]
    public async Task<Apiresponse<List<User>>> GetAll()
    {
        var res = new Apiresponse<List<User>>();
        try
        {
            var result = await _authService.GetAll();
            if(result == null)
            {
                res.Message = "Document is empty:";
                return res;
            }

            res.Message = "User found successfully";
            res.Status = true;
            res.Result = result;
        }
        catch (Exception e)
        {
            res.Message = "Error:" + e.Message;
            res.Status = false;
        }
        return res;
    }

    [HttpPost("register")]
    public async Task<Apiresponse<object>> Register([FromBody] RegisterDto request)
    {
        var res = new Apiresponse<object>();
        try
        {
            var existing = await _authService.GetByEmailAsync(request.Email);
            if (existing != null)
            {
                res.Message = "User already exists";
                res.Status = true;
                return res;
            }
            var user = new User
            {
                Email = request.Email,
                ContactNumber = request.ContactNumber,
                PasswordHash = PasswordHasher.Hash(request.Password),
            };

            await _authService.RegisterAsync(user);
            res.Message = "User registered successfully";
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

    [HttpPost("login")]
    public async Task<Apiresponse<User>> Login([FromBody] LoginDto request)
    {
        var res = new Apiresponse<User>();
        
        try
        {
            var user = await _authService.GetByEmailAsync(request.Email);
            if (user == null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
            {
                res.Message = "Invalid credenntials:";
                return res;
            }
            var token = _jwtService.GenerateToken(user.Id, user.Email);
            var refreshtoken = _jwtService.Generaterefreshtoken();

            var user1 = new User
            {

                Id = user.Id,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                ContactNumber = user.ContactNumber,
                OtpCode = user.OtpCode,
                OtpExpiryTime = DateTime.UtcNow.AddDays(7),
                Token = token,
                Refreshtoken = refreshtoken,
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),


            };
            var newuser = await _authService.Updateuserdata(user.Id, user1);
            
            res.Message = "Login successful";

            res.Status = true;
            res.Result = user1;
        }
        catch(Exception e)
        {
            res.Message = "Error:" + e.Message;
            res.Status = false;
        }
        return res;

       
    }

    [HttpPost("refresh-token")]
    public async Task<Apiresponse<User>> RefreshToken([FromBody] TokenRequest tokenRequest)
    {
        var res = new Apiresponse<User>();

        try
        {
            if (tokenRequest == null || string.IsNullOrEmpty(tokenRequest.RefreshToken))
            {
                res.Message = "Invalid request";
                res.Status = false;
                return res;
            }

            var user = await _authService.Getbyrefreshtoken(tokenRequest.RefreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                res.Message = "Invalid or expired refresh token";
                res.Status = false;
                return res;
            }

            var newAccessToken = _jwtService.GenerateToken(user.Id, user.Email);
            var newRefreshToken = _jwtService.Generaterefreshtoken();

            await _authService.Updaterefreshtoken(user.Id, user.Refreshtoken, DateTime.UtcNow.AddDays(7));

            var user1 = new User
            {

                Id = user.Id,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                ContactNumber = user.ContactNumber,
                OtpCode = user.OtpCode,
                OtpExpiryTime = DateTime.UtcNow.AddDays(7),
                Token = newAccessToken,
                Refreshtoken = newRefreshToken,
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
            };
            var newuser = await _authService.Updateuserdata(user.Id, user1);
            res.Result = user1;
            res.Message = "Token refreshed successfully";
            res.Status = true;
            return res;
        }
        catch(Exception e)
        {
            res.Message = "Error:" + e.Message;
            res.Status = false;
        }
        return res;

       
    }


    [HttpPost("generate-otp")]
    public async Task<Apiresponse<string>> GenerateOtp([FromBody] OtpRequestDto request)
    {
        var res = new Apiresponse<string>();
        try
        {
            var otp = await _authService.GenerateOtpAsync(request.Email);
            res.Message = "Otp generated sucessfully:";
            res.Status = true;
            res.Result = otp;
        }
        catch(Exception e)
        {
            res.Message = "Error:" + e.Message;
            res.Status = false;
        }
        return res;
       
    }

    [HttpPost("reset-password")]
    public async Task<Apiresponse<User>> ResetPassword([FromBody] VerifyOtpDto request)
    {
        var res = new Apiresponse<User>();
        try
        {
            var success = await _authService.VerifyOtpAsync(request.Email, request.Otp, request.NewPassword);
            if (!success)
            {
                res.Message = "Invalid or expired otp";
                res.Status = false;
                return res;
            }
            if(request.Email == null || request.Otp == null || request.NewPassword == null)
            {
                res.Message = "Enter data in the provided field";
                return res;
            }
            
            
            var user = new User
            {
                Email = request.Email,
                OtpCode = request.Otp,
                PasswordHash = request.NewPassword

            };

            res.Message = "Password reset successfully:";
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
    [HttpGet("Filter")]
    public async Task<Apiresponse<List<User>>> Getfilter([FromQuery] Queryparameter param)
    {
        var res = new Apiresponse<List<User>>();
        try
        {
            var user = await _authService.Getfilterbyquery(param);
            res.Message = "Data filtered successfully:";
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
}
