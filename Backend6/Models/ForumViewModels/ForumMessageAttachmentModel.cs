using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend6.Models.ForumViewModels
{
    public class ForumMessageAttachmentModel
    {
        public DateTime Created { get; set; }

        [Required]
        public String FileName { get; set; }

        [Required]
        public String FilePath { get; set; }
    }
}
