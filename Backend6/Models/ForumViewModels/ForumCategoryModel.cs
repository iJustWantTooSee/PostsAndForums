using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Backend6.Models.ForumViewModels
{
    public class ForumCategoryModel
    {
        [Required]
        [MaxLength(200, ErrorMessage = "Максимальная длина названия формума 200 символов")]
        public String Name { get; set; }
    }
}
