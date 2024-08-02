using System;
using System.Collections.Generic;

namespace test_task.Models
{
    /// <summary>
    /// Модель описывающая сущность Товара
    /// </summary>
    public partial class Product
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public decimal Price { get; set; }
    }

}

