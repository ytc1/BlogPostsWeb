using PostDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogPosts.Models
{
    public class MainModel : BaseViewModel
    {
        public IEnumerable<Post> posts { get; set; }
    }
}