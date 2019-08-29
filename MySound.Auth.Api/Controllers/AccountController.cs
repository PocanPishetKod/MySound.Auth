using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        private readonly UserManager<Account> _userManager;

        public AccountController(UserManager<Account> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> Account([Required][EmailAddress]string email)
        {
            var account = await _userManager.FindByEmailAsync(email);
            if (account == null)
            {
                return NotFound();
            }

            return Ok(new AccountGetViewModel()
            {
                Id = account.Id,
                Email = account.Email,
                UserName = account.UserName
            });
        }

        [HttpPost]
        public async Task<IActionResult> Account([Required]NewAccountViewModel viewModel)
        {
            var account = await _userManager.FindByEmailAsync(viewModel.Email);
            if (account != null)
            {
                return Conflict();
            }

            account = new Account()
            {
                Email = viewModel.Email,
                UserName = viewModel.UserName
            };

            var result = await _userManager.CreateAsync(account, viewModel.Password);
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
                Id = account.Id,
                Email = account.Email,
                UserName = account.UserName
            });
        }
    }
}