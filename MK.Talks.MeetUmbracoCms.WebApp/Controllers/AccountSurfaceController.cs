namespace MK.Talks.MeetUmbracoCms.WebApp.Controllers;

using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.Models;
using Umbraco.Cms.Web.Common.Security;
using Umbraco.Cms.Web.Website.Controllers;

public class AccountSurfaceController : SurfaceController
{
    private readonly IMemberManager _memberManager;
    private readonly IMemberSignInManager _memberSignInManager;
    private readonly IPublishedContentQuery _publishedContentQuery;

    public AccountSurfaceController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider, IMemberManager memberManager, IMemberSignInManager memberSignInManager, IPublishedContentQuery publishedContentQuery) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
    {
        _memberManager = memberManager;
        _memberSignInManager = memberSignInManager;
        _publishedContentQuery = publishedContentQuery;
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(string.Empty, "Please provide username and password");
            return CurrentUmbracoPage();
        }

        //validate credentials without logging in
        var validCredentials = await _memberManager.ValidateCredentialsAsync(model.Username, model.Password);

        if (validCredentials)
        {
            //sign in
            var loginResult = await _memberSignInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, true);
            if (loginResult.Succeeded)
            {
                return Redirect("/");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Unable to log in.");
            }

        }
        else
        {
            ModelState.AddModelError(string.Empty, "Wrong credentials");
        }

        return CurrentUmbracoPage();
    }

    public async Task<IActionResult> Logout()
    {
        await _memberSignInManager.SignOutAsync();
        return Redirect(_publishedContentQuery.ContentAtRoot().FirstOrDefault().Url());
    }
}