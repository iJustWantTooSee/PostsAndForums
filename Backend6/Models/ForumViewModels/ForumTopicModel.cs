﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend6.Models.ForumViewModels
{
    public class ForumTopicModel
    {
        [Required]
        public String Name { get; set; }
    }
}
