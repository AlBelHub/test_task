using System;
using System.Collections.Generic;

namespace test_task.Models
{
    /// <summary>
    /// Модель описывающая сущность поля заказа
    /// </summary>
    public partial class OrderItem
    {
        public int OrderItemId { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

    }
}


