using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend6.Models.ForumViewModels
{
    public class ForumMessageModel
    {
        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        [Required]
        public String Text { get; set; }
    }
}
