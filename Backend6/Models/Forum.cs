﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Backend6.Models
{
    public class Forum
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ForumCategoryId { get; set; }

        public ForumCategory ForumCategory { get; set; }

        public ICollection<ForumTopic> ForumTopics { get; set; }

        [Required]
        public String Name { get; set; }

        [Required]
        public String Description { get; set; }

    }
}
