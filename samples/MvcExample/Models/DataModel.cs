using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MvcExample.Models
{
    public class DataModel
    {
        [Key]
        public string Name { get; set; }

        public string Text { get; set; }

        public string Owner { get; set; }
    }
}
