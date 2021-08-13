﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcExample.Models
{
    public class ViewModel
    {
        public string Name { get; set; }

        public string Text { get; set; }

        public string Owner { get; set; }

        public Permissions Permissions { get; set; }
    }
}
