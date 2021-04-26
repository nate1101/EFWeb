using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using EventFully.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using EventFully.Models;

namespace EventFully.EMS
{
    public static class HtmlHelpers
    {

        public static string IsSelected(this IHtmlHelper html, string controller = null, string action = null, string cssClass = null)
        {
            if (String.IsNullOrEmpty(cssClass))
                cssClass = "active";

            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (String.IsNullOrEmpty(controller))
                controller = currentController;

            if (String.IsNullOrEmpty(action))
                action = currentAction;

            return controller == currentController && action == currentAction ?
                cssClass : String.Empty;
        }

        public static string PageClass(this IHtmlHelper htmlHelper)
        {
            string currentAction = (string)htmlHelper.ViewContext.RouteData.Values["action"];
            return currentAction;
        }

    }

    [HtmlTargetElement("UserFullName")]
    public class UserFullNameTagHelper : TagHelper
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserFullNameTagHelper(IUserService userService, UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        public string username { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "strong";
            output.TagMode = TagMode.StartTagAndEndTag;
            var user = _userManager.FindByNameAsync(username).Result;
            var content = $"{user.FirstName} {user.LastName}";

            output.Attributes.SetAttribute("class", "font-bold");
            output.Content.SetHtmlContent(content?.ToString());

            base.Process(context, output);
        }
    }

    [HtmlTargetElement("UserInitials")]
    public class UserInitialsTagHelper : TagHelper
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserInitialsTagHelper(IUserService userService, UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        public string username { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;
            var user = _userManager.FindByNameAsync(username).Result;
            var content = $"{user.FirstName.Substring(0,1)}{user.LastName.Substring(0,1)}";

            output.Attributes.SetAttribute("class", "initial-circle");
            output.Content.SetHtmlContent(content?.ToString());

            base.Process(context, output);
        }
    }
}
