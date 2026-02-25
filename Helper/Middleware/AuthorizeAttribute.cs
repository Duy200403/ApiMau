using System;
using System.Collections.Generic;
using System.Linq;
using ApiWebsite.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiWebsite.Helper.Middleware
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
  public class AuthorizeAttribute : Attribute, IAuthorizationFilter
  {
    private readonly IList<Role> _roles;

    public AuthorizeAttribute(params Role[] roles)
    {
      _roles = roles ?? new Role[] { };
    }
    public void OnAuthorization(AuthorizationFilterContext context)
    {
      var account = (Account)context.HttpContext.Items[ConstantsInternal.Account];
      var enumList = new List<Role>();
      if (account != null)
      {
        List<string> listStringRoles = new List<string>(account.Roles.Split(','));
        enumList = listStringRoles.Select(x => Enum.Parse(typeof(Role), x))
                              .Cast<Role>()
                              .ToList();
      }

      if (account == null || !_roles.Any(r => enumList.Contains(r)))
      {
        context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
      }
    }
  }
}