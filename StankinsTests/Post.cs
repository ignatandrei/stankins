using System;
using System.Collections.Generic;
using System.Text;

namespace StankinsTests
{
    public class Post
    {
        public Post()
        {
            LastModifiedOrCreatedTime = DateTime.Now;
        }
        public int Id { get; set; }

        public int UserId { get; set; }      

        public string Content { get; set; }

        public DateTime LastModifiedOrCreatedTime { get; set; }
    }
}
