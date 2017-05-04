using PostDatabase;
using BlogPosts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Amazon;
using Amazon.Runtime;
using Amazon.S3.Model;
using Amazon.S3;

namespace BlogPosts.Controllers
{
    public class HomeController : BaseController
    {
        private string connectionString = @"Data Source=LAPTOP-6JDKR5MU\SQLEXPRESS;Initial Catalog=PostsWebsite;Integrated Security=True";
        public ActionResult Main()
        {
            var data = new PostDatabase.UserManager(connectionString);
            MainModel mm = new MainModel();
            mm.posts = data.GetPosts();

            return View(mm);
        }
        public ActionResult Post(int postId)
        {
            var data = new PostDatabase.UserManager(connectionString);
            PostModel pm = new PostModel();
            pm.post = data.GetPost(postId);
            pm.replies = data.GetReplies(postId);

            return View(pm);
        }
        public ActionResult SignIn()
        {
            return View();
        }
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult newuser(string name, string password)
        {
            var data = new PostDatabase.UserManager(connectionString);
            User user = data.AddUser(name, password);
            FormsAuthentication.SetAuthCookie(user.Name, true);
            return RedirectToAction("Main");
        }
        [HttpPost]
        public ActionResult userSignin(string name, string password)
        {
            var data = new PostDatabase.UserManager(connectionString);
            User user = data.GetUser(name, password);
            FormsAuthentication.SetAuthCookie(user.Name, true);
            return RedirectToAction("Main");
        }
        [HttpPost]
        public ActionResult checkUser(string name)
        {
            var data = new PostDatabase.UserManager(connectionString);
            bool valid = data.CheckUser(name);
            return Json(valid);
        }
        [Authorize]
        public ActionResult AddPost()
        {

            return View();
        }
        [HttpPost]
        public ActionResult AddPost(string content, string title, HttpPostedFileBase pic)
        {
            var data = new PostDatabase.UserManager(connectionString);
            User u = data.GetUserId(User.Identity.Name);
            Post p = new Post();
            //IAmazonS3 client;
            //client = new AmazonS3Client(Amazon.RegionEndpoint.USEast2);
            //PutObjectRequest request = new PutObjectRequest()
            //{
            //    BucketName = bucketName,
            //    Key = keyName,
            //    FilePath = filePath
            //};
            //PutObjectResponse response2 = client.PutObject(request);
            Guid guid = Guid.NewGuid();
            pic.SaveAs(Server.MapPath("uploads/" + guid + ".jpg"));
           // pic.SaveAs("C:/Program Files (x86)/IIS Express/Uploads");
            p.PictureLink = (Server.MapPath("uploads/" + guid + ".jpg"));
            data.AddPost(title, content, u.Id, p.PictureLink);
            return RedirectToAction("Main");
        }
        [HttpPost]
        public ActionResult AddReply(string reply, int postId)
        {
            var data = new PostDatabase.UserManager(connectionString);
            User u = data.GetUserId(User.Identity.Name);
            data.AddReply(reply, postId, u.Id);
            return RedirectToAction("Post", new { postId = postId });
        }

    }
}