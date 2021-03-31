using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Backend6.Models.ForumViewModels
{
    public class ForumModel
    {
        [Required]
        public String Name { get; set; }

        [Required]
        public String Description { get; set; }
    }
}
