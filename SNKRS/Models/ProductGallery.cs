using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SNKRS.Models
{
    public class ProductGallery
    {
        public int Id { get; set; }

        public string Src { get; set; }

        public int ProductId { get; set; }

        public virtual Portfolio Product { get; set; }
    }
}