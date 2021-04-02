using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Backend6.Models.ForumViewModels
{
    public class ForumMessageAttachmentModel
    {
        public DateTime Created { get; set; }

        [Required]
        public IFormFile File { get; set; }
    }
}
