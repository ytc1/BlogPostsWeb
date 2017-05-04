using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data.Linq;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Amazon;
using Amazon.Runtime;
using Amazon.S3.Model;
using Amazon.S3;
using System.Web;

namespace PostDatabase
{
    public class UserManager
    {
        private string _connectionString;
        public UserManager(string connectionString)
        {
            _connectionString = connectionString;
        }
        public User AddUser(string name, string password)
        {
            User user = new User();
            string salt = PasswordHelper.GenerateRandomSalt();
            user.PasswordHash = PasswordHelper.HashPassword(password, salt);
            user.Salt = salt;
            user.Name = name;
            using (var context = new DataConnectionDataContext(_connectionString))
            {
                context.Users.InsertOnSubmit(user);
                context.SubmitChanges();
            }
            return user;

        }
        public User GetUser(string username, string password)
        {
            using (var context = new DataConnectionDataContext(_connectionString))
            {
                User user = context.Users.Where(u => u.Name == username).First();
                bool success = PasswordHelper.PasswordMatch(password, user.PasswordHash, user.Salt);
                return success ? user : null;
            }
        }
        public bool CheckUser(string username)
        {
            using (var context = new DataConnectionDataContext(_connectionString))
            {
                return !context.Users.Any(u => u.Name == username);
            }
        }
        public IEnumerable<Post> GetPosts()
        {
            using (var context = new DataConnectionDataContext(_connectionString))
            {


                var loadOptions = new DataLoadOptions();

                loadOptions.LoadWith<Post>(p => p.User);

                loadOptions.LoadWith<Post>(p => p.Replies);



                context.LoadOptions = loadOptions;

                return context.Posts.ToList();
            }
        }
        public void GetPicture(Post post)
        {
            
             IAmazonS3 client = new AmazonS3Client();
            using (client = new AmazonS3Client(Amazon.RegionEndpoint.USEast2))
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = "bucketonetest1",
                    Key = post.PictureLink
                };

                using (GetObjectResponse response = client.GetObject(request))
                {
                    string dest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), post.Title);
                    if (!File.Exists(dest))
                    {
                       
                        response.WriteResponseStreamToFile(dest);
                    }
                 
                }
            }

        }
        public void AddPost(string title, string content, int userId,string pictureLink)
        {
            Post post = new Post();
            post.Content = content;
            post.Date = DateTime.Now;
            post.Title = title;
            post.User_Id = userId;
            post.PictureLink = pictureLink;
            
             IAmazonS3 client;
            client = new AmazonS3Client(Amazon.RegionEndpoint.USEast2);
            using (client = new AmazonS3Client(Amazon.RegionEndpoint.USEast2))
            {

                PutObjectResponse response;
                IAsyncResult asyncResult;
                PutObjectRequest putRequest = new PutObjectRequest
                {
                    BucketName = "bucketonetest1",
                    Key = pictureLink,
                    FilePath = pictureLink,
                    ContentType = "text/plain"
                };
                putRequest.Metadata.Add(post.Title, post.Content);
                 response = client.PutObject(putRequest);
                try
                {
                    response = client.PutObject(putRequest);
                }
                catch (AmazonS3Exception s3Exception)
                {
                    //
                    // Code to process exception
                    //
                }

            }
            using (var context = new DataConnectionDataContext(_connectionString))
            {
                context.Posts.InsertOnSubmit(post);
                context.SubmitChanges();
            }


        }
        public void AddReply(string reply, int postId, int userId)
        {
            Replies r = new Replies();
            r.Reply = reply;
            r.User_Id = userId;

            r.Date = DateTime.Now;

            r.Post_Id = postId;

            using (var context = new DataConnectionDataContext(_connectionString))
            {
                context.Replies.InsertOnSubmit(r);
                context.SubmitChanges();
            }



        }
        public Post GetPost(int postId)
        {
            using (var context = new DataConnectionDataContext(_connectionString))
            {


                var loadOptions = new DataLoadOptions();
                loadOptions.LoadWith<Post>(p => p.User);
                loadOptions.LoadWith<Post>(p => p.Replies);
                loadOptions.LoadWith<Replies>(r => r.Reply);
                context.LoadOptions = loadOptions;
                Post post = context.Posts.Where(p => p.Id == postId).First();

                return post;
            }
        }
        public IEnumerable<Replies> GetReplies(int postId)
        {
            using (var context = new DataConnectionDataContext(_connectionString))
            {


                var loadOptions = new DataLoadOptions();
                loadOptions.LoadWith<Replies>(r => r.User);
             
                context.LoadOptions = loadOptions;
                IEnumerable<Replies> replies = context.Replies.Where(r => r.Post_Id == postId).ToList();

                return replies;
            }
        }
        public User GetUserId(string name)
        {
            using (var context = new DataConnectionDataContext(_connectionString))
            {
                return context.Users.Where(u => u.Name == name).FirstOrDefault();
            }
        }
    }
}