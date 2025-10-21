using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PCStore_API.Controllers;


[ApiController]
[Authorize(Roles = "User,Admin")]
[Route("api/[controller]")]
public class UserController
{
    
}