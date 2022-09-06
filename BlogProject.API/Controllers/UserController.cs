﻿using BlogProject.Business.Services.AuthenticationService;
using BlogProject.Business.Services.PostService;
using BlogProject.Business.Services.UserService;
using BlogProject.Business.Services.UserService.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogProject.API.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IPostService _postService;
    private readonly IJwtAuthenticationManager _jwtAuthenticationManager;

    public UserController(
        IUserService userService,
        IPostService postService,
        IJwtAuthenticationManager jwtAuthenticationManager) =>
        (_userService, _postService, _jwtAuthenticationManager) =
        (userService, postService, jwtAuthenticationManager);

    // GET
    [HttpGet("{id:int:min(1)}")]
    public async Task<IActionResult> Get(int id)
    {
        if (id == 0) return BadRequest();
        if (!await _userService.IsExistAsync(id)) return NotFound();
    
        var response = await _userService.GetAsync(id);

        return Ok(response);
    }
    [HttpGet("Login")]
    public async Task<IActionResult> Login(string username, string password)
    {
        var response = await _userService.ValidateUserAsync(username, password);

        if (response == null) return NotFound();

        var tokenResponse = await _jwtAuthenticationManager.GetJwtTokenAsync(response.UserName);

        if (tokenResponse == null) throw new Exception("Token is null");

        response.Token = tokenResponse;

        return Ok(response);
    }
    [Authorize]
    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        var responseList = await _userService.GetAllAsync();

        return Ok(responseList);
    }
    [HttpGet("{id:int:min(1)}/Posts")]
    public async Task<IActionResult> GetPosts([FromRoute]int id)
    {
        if (id == 0) return BadRequest();
        
        var responseList = await _postService.GetAllByUserIdAsync(id);

        return Ok(responseList);
    }
    [HttpGet("{id:int:min(1)}/EditedPosts")]
    public async Task<IActionResult> GetEditedPosts([FromRoute]int id)
    {
        var responseList = await _postService.GetAllByEditorIdAsync(id);

        return Ok(responseList);
    }

    [HttpGet("IsExist")]
    public async Task<IActionResult> IsExist(int userId)
    {
        if (userId == 0) return BadRequest();
    
        var response = await _userService.IsExistAsync(userId);

        return Ok(response);
    }
    [HttpGet("Validate")]
    public async Task<IActionResult> Validate(string username, string password)
    {
        var response = await _userService.ValidateUserAsync(username, password);

        return Ok(response);
    }

    [HttpGet("GetUserIdByUsername")]
    public async Task<IActionResult> GetUserIdByUsername(string username)
    {
        var response = await _userService.GetUserIdByUsername(username);

        return Ok(response);
    }
    [HttpGet("IsEmailExist")]
    public async Task<IActionResult> IsEmailExist(string email)
    {
        var response = await _userService.IsEmailExistAsync(email);

        return Ok(response);
    }

    // POST
    [HttpPost("")]
    public async Task<IActionResult> Add(AddUserRequest request)
    {
        var affectedRows = await _userService.AddAsync(request);

        return Ok(affectedRows);
    }

    // PATCH
    [Authorize]
    [HttpPatch("")]
    public async Task<IActionResult> Update(UpdateUserRequest request)
    {
        var affectedRows = await _userService.UpdateAsync(request);

        return Ok(affectedRows);
    }

    // DELETE
    [Authorize]
    [HttpDelete("")]
    public async Task<IActionResult> Delete(int userId)
    {
        var affectedRows = await _userService.DeleteAsync(userId);

        return Ok(affectedRows);
    }
}

