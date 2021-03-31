﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Backend6.Models
{
    public class ForumMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public String CreatorId { get; set; }

        public ApplicationUser Creator { get; set; }

        public Guid ForumTopicId { get; set; } 

        public ForumTopic ForumTopic { get; set; }

        public ICollection<ForumMessageAttachment> ForumMessageAttachments { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        [Required]
        public String Text { get; set; }

    }
}
