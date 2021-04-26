using EventFully.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventFully.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace EventFully
{
    public class HasEventPermissionHandler : AuthorizationHandler<HasEventPermissionRequirement, RequirementType>//, IAuthorizationRequirement
    {
        private readonly UserManager<ApplicationUser> _userManager;
        IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HasEventPermissionHandler(IUserService userService, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasEventPermissionRequirement requirement, RequirementType requirementType)
        {
            // get the user's role
            var roleIds = _userService.GetUserEventRoles(requirementType.Id,_userManager.GetUserId(context.User)).Result;
            // if the user is an AD or Content Administrator, pass them through
            if (roleIds.Contains(Constant.SecurityRole.Administrator))
            {
                context.Succeed(requirement);
                return Task.FromResult(0);
            }

            // check the appropriate permission
            //switch (requirementType.Type)
            //{
            //    case Constant.RequirementTypes.Team:
            //        // check that the user has permission for this team
            //        if (_userService.RoleHasPermission(roleIds, requirement.PermissionId) && _userService.UserHasTeamPermission(_userManager.GetUserId(context.User), requirementType.Id))
            //        {
            //            context.Succeed(requirement);
            //            return Task.FromResult(0);
            //        }
            //        break;
            //    case Constant.RequirementTypes.RosterAthlete:
            //        // check that the user has permission for this athlete
            //        if (_userService.RoleHasPermission(roleIds, requirement.PermissionId) && _userService.UserHasRosterAthletePermission(_userManager.GetUserId(context.User), requirementType.Id))
            //        {
            //            context.Succeed(requirement);
            //            return Task.FromResult(0);
            //        }
            //        break;
            //    case Constant.RequirementTypes.RosterStaff:
            //        // check that the user has permission for this athlete
            //        if (_userService.RoleHasPermission(roleIds, requirement.PermissionId) && _userService.UserHasRosterStaffPermission(_userManager.GetUserId(context.User), requirementType.Id))
            //        {
            //            context.Succeed(requirement);
            //            return Task.FromResult(0);
            //        }
            //        break;
            //}


            context.Fail();
            return Task.FromResult(0);
        }
    }

    public class HasEventPermissionRequirement : IAuthorizationRequirement
    {
        public HasEventPermissionRequirement(int permissionId)
        {
            PermissionId = permissionId;
        }

        public int PermissionId { get; set; }
    }
}
