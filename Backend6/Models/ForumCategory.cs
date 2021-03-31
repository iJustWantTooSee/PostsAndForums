using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend6.Models
{
    public class ForumCategory
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public ICollection<Forum> Forums { get; set; }

        [Required]
        [MaxLength(200, ErrorMessage = "Максимальная длина названия формума 200 символов")]
        public String Name { get; set; }
    }
}
