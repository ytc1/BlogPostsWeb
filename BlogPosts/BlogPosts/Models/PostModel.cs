using PostDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogPosts.Models
{
    public class PostModel : BaseViewModel
    {
        public Post post { get; set; }
        public IEnumerable<Replies> replies { get; set; }
    }
}