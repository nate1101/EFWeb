using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EventFully.EMS.Models;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using EventFully.Models;
using Microsoft.AspNetCore.Identity;
using EventFully.Services.Interfaces;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http;
using System.Web;
using DataTables.AspNet.Core;
using System.Linq.Expressions;
using DataTables.AspNet.AspNetCore;

namespace EventFully.EMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IEventService _eventService;
        private readonly IGenericSearchService<OrderSummaryView> _orderGenericSearchService;
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService,
            IEventService eventService,
            IGenericSearchService<OrderSummaryView> orderGenericSearchService
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _eventService = eventService;
            _orderGenericSearchService = orderGenericSearchService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult OrderHistory()
        {
            return View();
        }

        [HttpGet("/account/invoice/{orderId:int}")]
        public async Task<IActionResult> Invoice(int orderId)
        {
            var order = await _eventService.GetOrderById(orderId);
            return View(order);
        }

        public async Task<IActionResult> InvoicePrint(int orderId)
        {
            var order = await _eventService.GetOrderById(orderId);
            return View(order);
        }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // GET: /Account/SignUp
        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignUp(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                var model = new ResetPasswordModel
                {
                    Code = HttpUtility.HtmlDecode(code)
                };
                return View(model);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Username);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // pre-login check
                var user = _userManager.FindByNameAsync(model.Username).Result;
                
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl == null ? "/" : returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return LocalRedirect("/");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Error confirming email for user with ID '{userId}':");
            }

            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEventInvitation(string email, string token)
        {
            if (token == null)
            {
                return LocalRedirect("/");
            }

            var invite = await _eventService.GetUserEventInvitationByToken(token);
            if (invite == null)
            {
                return NotFound($"Unable to load invitation.");
            }

            if (invite.AcceptedDate != null)
            {
                ViewData["AcceptedMessage"] = "This invitation has previously been accepted";
                return View();
            }

            if (invite.TokenExpiration.Subtract(DateTime.Now).Ticks < 0)
            {
                ViewData["ExpirationMessage"] = "This invitation has expired";
                return View();
            }

            var user = await _userManager.FindByEmailAsync(invite.EmailAddress);
            if (user == null)
            {
                //register
                return RedirectToAction("SignUp", new { returnUrl = "/Account/ConfirmEventInvitation?email=" + email + "&token=" + token });
            }
            else
            {
                invite.AcceptedByUserId = user.Id;
                invite.AcceptedDate = DateTime.Now;

                await _eventService.SaveUserEventInvitation(invite);

                await _eventService.SaveUserEventRole(new UserEventRole()
                {
                    EventId = invite.EventId,
                    RoleId = invite.RoleId,
                    UserId = user.Id
                });
            }
            // Confirm Invitation
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(RegisterModel model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    //TODO: Use Automapper instead of manual binding

                    UserName = model.UserName,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.UserName
                };

                var identityResult = await _userManager.CreateAsync(user, model.Password);
                if (identityResult.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = user.Id, code = code },
                        Url.ActionContext.HttpContext.Request.Scheme

                        );

                    await _emailService.SendEmailConfirmationAsync(model.UserName, "Confirm your email", HtmlEncoder.Default.Encode(callbackUrl));

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Username);
                if (user == null) // || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                var callbackUrl = Url.Action(
                        "ResetPassword",
                        "Account",
                        new { code = code },
                        Url.ActionContext.HttpContext.Request.Scheme
                        );

                await _emailService.SendPasswordResetAsync(
                    model.Username,
                    "Reset Password",
                    HtmlEncoder.Default.Encode(callbackUrl));

                return RedirectToAction("ForgotPasswordConfirmation");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> AppForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Username);
                if (user == null) // || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return BadRequest();
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                var callbackUrl = Url.Action(
                        "ResetPassword",
                        "Account",
                        new { code = code },
                        Url.ActionContext.HttpContext.Request.Scheme
                        );

                await _emailService.SendPasswordResetAsync(
                    model.Username,
                    "Reset Password",
                    HtmlEncoder.Default.Encode(callbackUrl));

                return Ok();
            }

            return BadRequest();
        }
        //
        // POST: /Account/LogOff
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            //return LocalRedirect("/");
            return Json(new { success = true });
        }

        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userName = await _userManager.GetUserNameAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            var model = new ProfileModel();
            model.UserName = userName;
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.Email = user.Email;
            model.IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ProfileModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var email = await _userManager.GetEmailAsync(user);
            if (model.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting email for user with ID '{userId}'.");
                }
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            //StatusMessage = "Your profile has been updated";
            return Json(new { success = true });
        }

        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                var message = "";
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    message += ". " + error.Description;
                }
                return Json(new { success = false, message=message });
            }

            await _signInManager.RefreshSignInAsync(user);

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DownloadPersonalData()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Only include personal data for download
            var personalData = new Dictionary<string, string>();
            var personalDataProps = typeof(IdentityUser).GetProperties().Where(
                            prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (var p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }

            Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
            return new FileContentResult(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(personalData)), "text/json");
        }

        public async Task<IActionResult> PersonalData()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return View();
        }

        public async Task<IActionResult> DeletePersonalData()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeletePersonalData(DeletePersonalData model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

                if (!await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    ModelState.AddModelError(string.Empty, "Password not correct.");
                    return View();
                }

            var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleteing user with ID '{userId}'.");
            }

            await _signInManager.SignOutAsync();

            return Redirect("~/");
        }

        [HttpPost("account/GetOrderSummary")]
        public IActionResult GetOrderSummary(IDataTablesRequest request)
        {
            var userId = _userManager.GetUserId(User);
            // set initial filter
            Expression<Func<OrderSummaryView, bool>> filterExpression = c => c.UserId == userId;

            // init search expression
            Expression<Func<OrderSummaryView, bool>> searchExpression = null;
            // create default sort
            Expression<Func<OrderSummaryView, object>> orderBy = c => c.Id.ToString();

            // get the sorting information
            string sortDirection = "Ascending";
            var sortColumn = request.Columns.Where(c => c.Sort != null).FirstOrDefault();

            if (sortColumn != null)
            {
                // set the sort direction
                switch (sortColumn.Sort.Direction)
                {
                    case SortDirection.Ascending:
                        sortDirection = "Descending";
                        break;
                    case SortDirection.Descending:
                        sortDirection = "Ascending";
                        break;
                    default:
                        sortDirection = "Ascending";
                        break;
                }

                // set the sort field
                switch (sortColumn.Field.ToUpper())
                {
                    case "ID":
                        orderBy = c => c.Id;
                        break;
                    //case "ORDERDATE":
                    //    orderBy = c => c.OrderDate;
                    //    break;
                    default:
                        orderBy = c => c.Id;
                        break;
                }
            }

            // handle search
            if (!string.IsNullOrWhiteSpace(request.Search.Value))
            {
                string search = request.Search.Value;
                searchExpression = s => s.EventName.Contains(search);
            }
            // get the datatable results
            SearchResponse<OrderSummaryView> resultx = _orderGenericSearchService.Search(filterExpression, searchExpression, orderBy, request.Length + request.Start, request.Start, sortDirection);

            // create the datatable response
            var result = DataTablesResponse.Create(request, resultx.iTotalRecords, resultx.iTotalDisplayRecords, resultx.Results);

            // return the result
            return new DataTablesJsonResult(result, true);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
