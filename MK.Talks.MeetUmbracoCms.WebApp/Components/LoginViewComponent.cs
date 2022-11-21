namespace MK.Talks.MeetUmbracoCms.WebApp.Components;

using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Models;

public class LoginViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View(new LoginModel());
    }
}