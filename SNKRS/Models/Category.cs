using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SNKRS.Models
{
    public class Category
    {
        public Category()
        {
            Products = new HashSet<Portfolio>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Portfolio> Products { get; set; }
    }
}