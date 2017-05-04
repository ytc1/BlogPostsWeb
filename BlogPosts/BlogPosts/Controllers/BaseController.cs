using PostDatabase;
using BlogPosts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BlogPosts.Controllers
{
    public class BaseController : Controller
    {
        protected override ViewResult View(IView view, object model)
        {
            if (model == null)
            {
                var bvm = new BaseViewModel();
                SetUserDetails(bvm);
                model = bvm;
            }
            else
            {
                var bvm = (BaseViewModel)model;
                SetUserDetails(bvm);
            }
            return base.View(view, model);
        }

        private void SetUserDetails(BaseViewModel viewModel)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return;
            }
            viewModel.User = new User
            {
                Name = User.Identity.Name
            };
        }

        protected override ViewResult View(string viewName, string masterName, object model)
        {
            if (model == null)
            {
                var bvm = new BaseViewModel();
                SetUserDetails(bvm);
                model = bvm;
            }
            else
            {
                var bvm = (BaseViewModel)model;
                SetUserDetails(bvm);
            }
            return base.View(viewName, masterName, model);
        }
    }
}