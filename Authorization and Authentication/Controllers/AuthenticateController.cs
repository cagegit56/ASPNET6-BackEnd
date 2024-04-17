using Authorization_and_Authentication.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using User.Management.Service.Models;
using User.Management.Service.Services;


namespace Authorization_and_Authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ApplicationDbContext _ApplicationDbContext;


        public AuthenticateController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, IEmailService emailService, ApplicationDbContext ApplicationDbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService;
            _ApplicationDbContext = ApplicationDbContext;

        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                //Is Email Confirmed
                var mailConfirmed = await _userManager.FindByEmailAsync(user.Email);
                if (mailConfirmed != null)
                {

                bool emailStatus = await _userManager.IsEmailConfirmedAsync(mailConfirmed);
                if (emailStatus)
                {              
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Name, user.UserName.ToString())
                    
                };

                foreach (var userRole in userRoles)
                {                           
                  authClaims.Add(new Claim("role", userRole) );                 
                 }

                var token = GetToken(authClaims, Get_configuration());
                     return Ok(new
                       {
                          token = new JwtSecurityTokenHandler().WriteToken(token),
                          expiration = token.ValidTo,
                         // role = userRoles
                       });

                }
                    else
                     {
                      return Ok(new Response
                        {
                          Status = "Error",
                          Message = "Email is unconfirmed, please confirm it first "
                      });
                   }
                  }


            }
            return Unauthorized();

        }


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });
            var emailExists = await _userManager.FindByEmailAsync(model.Email);
            if (emailExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User with this email address already exists!" });

            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError,
               new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            //Email Verification using Token

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authenticate", new { token, email = user.Email }, Request.Scheme);
            var expiration = DateTime.Now.AddMinutes(8);

            var message = new Message(new string[] { user.Email! }, "Confirmation email link", $@"<h1>Verify Email</h1>
                 <br><p style='color: green'>Please Click the Link below to Verify Emaill Account.</p><br> <a href='{confirmationLink!}'>Confirm Email </a><br>
                <p style='color: red'> WARNING<br> The Verify Link Token will Expire at: {expiration}</p>");
            _emailService.SendEmail(message);

            return Ok(new Response { Status = "Success",
                Message = $"User created successfully & Email sent to {user.Email} Successfully" });

        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }
            //Email Confirmation
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authenticate", new { token, email = user.Email }, Request.Scheme);
            var expiration = DateTime.Now.AddMinutes(8);

            var message = new Message(new string[] { user.Email! }, "Confirmation email link", $@"<h1>Verify Email</h1>
                 <br><p style='color: green'>Please Click the Link below to Verify Emaill Account.</p><br> <a href='{confirmationLink!}'>Confirm Email </a><br>
                <p style='color: red'> WARNING<br> The Verify Link Token will Expire at: {expiration}</p>");
            _emailService.SendEmail(message);

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        private IConfiguration Get_configuration()
        {
            return _configuration;
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims, IConfiguration _configuration)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(8),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        [HttpGet("ConfirmEmail")]

        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                    var result = await _userManager.ConfirmEmailAsync(user, token);
                    if (result.Succeeded)
                    {
                        return StatusCode(StatusCodes.Status200OK,
                            new Response { Status = "Success", Message = "Email Verified Successfully, Please Login" });
                    }
                    return StatusCode(StatusCodes.Status500InternalServerError,
                            new Response { Status = "Error", Message = "Invalid or Expired Token" });
               
            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "This User Does not Exist" });


        }




        [HttpPost("ForgotPassword")]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel forgotPasswordModel)
        {
            if (ModelState.IsValid) {
                var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
                if (user == null)
                    return Unauthorized();
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                string url = $"{_configuration["AppAngular"]}?token={token}&email={user.Email}";
                var expires = DateTime.Now.AddMinutes(8);

                var message = new Message(new string[] { user.Email! }, "Reset Password link",
                 $@"<body><h1>Password Reset</h1> <br><p style='color: green'>Please Click the Link below to Reset your Password.</p> <br>
                  <a href='{url!}'><b>Reset Password Here</b> </a>  <br>
                   <p style='color: red'> WARNING<br> The Reset Password Link Token will Expire at: {expires}</p> </body>");
                _emailService.SendEmail(message);
                return StatusCode(StatusCodes.Status200OK,
                        new Response { Status = "Success", Message = $"Successfully Sent Reset Password Link{url}" });

            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                 new Response { Status = "Error", Message = "This User Does not Exist" });

        }



    [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model) 
        {
                       
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var result = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", model.Token);
                if (result)
                {
                        var userPass = await _userManager.RemovePasswordAsync(user);
                        if (userPass.Succeeded)
                        {
                            var resultPass = await _userManager.AddPasswordAsync(user, model.Password);
                            if (resultPass.Succeeded)
                            {
                                return StatusCode(StatusCodes.Status200OK,
                            new Response { Status = "Success", Message = "Password Successfully Changed" });
                            }
                        return StatusCode(StatusCodes.Status500InternalServerError,
                new Response { Status = "Error", Message = "Failed to Add Password" });
                    }
                    return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "Failed to Remove Password" });

                }
                return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "Invalid User Token" });
            }


           return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "This User Does not Exist" });
        }


        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAdminData()
        {
            // Return data accessible only by users with Admin role
            return Ok("Admin data");
        }

        [HttpGet("user")]
        [Authorize(Roles = "User")]
        public IActionResult GetUserData()
        {
            // Return data accessible only by users
            return Ok("User data");
        }

       // [HttpGet("myUsers")]
       // public IActionResult Get()
       // {
        //    return Ok(_userManager.AspNetUsers.ToList());
       // }


        [HttpGet("UseDetails")]
        [Authorize]

        public async Task<Object> getUserDetails()
        {
            string userId = User.Claims.First(c => c.Type == "Id").Value;
            var user = await _userManager.FindByIdAsync(userId);
            return new {
                user.UserName
                
            };

        }





        // [HttpPost("resetPassword")]
        // public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel resetPasswordModel)
        // {

        // var resultPass = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
        // if (resultPass != null)
        //{
        //   var userPass = await _userManager.ResetPasswordAsync(resultPass, resetPasswordModel.Token, resetPasswordModel.Password);
        //   if (userPass.Succeeded)
        //   {
        //        return StatusCode(StatusCodes.Status200OK,
        //        new Response { Status = "Success", Message = "Password Successfully Changed" });
        //     }
        //     return Ok("Password Not Changed");
        //  }


        //   return StatusCode(StatusCodes.Status500InternalServerError,
        //                new Response { Status = "Error", Message = "This User Does not Exist" });
        // }




    }
}
