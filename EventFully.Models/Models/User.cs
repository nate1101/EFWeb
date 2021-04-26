using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventFully.Models
{
    public class LoginModel
    {
        [Required]
        public String Username { get; set; }

        [Required]
        public String Password { get; set; }

        public bool IsEmailConfirmed { get; set; }
    }

    public class ForgotPasswordModel
    {
        [Required]
        public String Username { get; set; }
    }

    public class ResetPasswordModel
    {
        [Required]
        [EmailAddress]
        public String Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }

    }
    public class RegisterModel // : LoginModel
    {
        [Required]
        [StringLength(200)]
        [Display(Name = "First Name")]
        public String FirstName { get; set; }

        [Required]
        [StringLength(250)]
        [Display(Name ="Last Name")]
        public String LastName { get; set; }

        [Required]
        [EmailAddress]
        public String UserName { get; set; }

        [Required]
        public String Password { get; set; }

        [Required]
        [Compare("Password")]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        public String PasswordConfirmation { get; set; }
        public bool IsEmailConfirmed { get; set; }
        [EmailAddress]
        public String Email { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ProfileModel
    {
        [Required]
        [StringLength(200)]
        [Display(Name = "First Name")]
        public String FirstName { get; set; }

        [Required]
        [StringLength(250)]
        [Display(Name = "Last Name")]
        public String LastName { get; set; }

        [Required]
        [EmailAddress]
        public String UserName { get; set; }

        public bool IsEmailConfirmed { get; set; }
        [EmailAddress]
        public String Email { get; set; }
    }

    public class DeletePersonalData
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(200)]
        public String FirstName { get; set; }

        [Required]
        [MaxLength(250)]
        public String LastName { get; set; }

    }

    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RoleDescription { get; set; }
        public bool InternalRole { get; set; }
        public bool OrganizerRole { get; set; }
    }

    public class UserEventRole
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int EventId { get; set; }
        public int RoleId { get; set; }
    }

    public class UserEventRoleView
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int EventId { get; set; }
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public bool InviteSent { get; set; }
        public bool InviteExpired { get; set; }
    }

    public class RequirementType
    {
        public int Type { get; set; }
        public int Id { get; set; }
    }

    public class UserEventInvitation
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string EmailAddress { get; set; }
        public int RoleId { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpiration { get; set; }
        public string AcceptedByUserId { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedByUserId { get; set; }
        public virtual Event Event { get; set; }

    }
}
