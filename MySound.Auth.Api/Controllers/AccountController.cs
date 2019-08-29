using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySound.Auth.Api.ViewModels.Account;
using MySound.Auth.Models.Domain;

namespace MySound.Auth.Api.Controllers 
{
    [Route("account")]
    [ApiController]
    public class AccountController : ControllerBase 
    {
        private readonly UserManager<User> _userManager;

        public AccountController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> Account([Required][EmailAddress]string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(new AccountGetViewModel()
            {
                Id = user.Id,
                Email = user.Email
            });
        }

        [HttpPost]
        public async Task<IActionResult> Account([FromBody]NewAccountViewModel viewModel)
        {
            var user = await _userManager.FindByEmailAsync(viewModel.Email);
            if (user != null)
            {
                return Conflict();
            }

            user = new User()
            {
                Email = viewModel.Email,
                UserName = viewModel.UserName
            };

            var result = await _userManager.CreateAsync(user, viewModel.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(x => new 
                { 
                    Code = x.Code,
                    Description = x.Description
                }));
            }

            return Ok(new AccountGetViewModel()
            {
                Id = user.Id,
                Email = user.Email
            });
        }
    }
}