﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompleetKassa.Models
{
    public class ProductCategory : Category
    {
        public IList<ProductSubCategory> SubCategories { get; set; }
    }
}
