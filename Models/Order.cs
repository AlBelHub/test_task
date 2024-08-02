using System;
using System.Collections.Generic;

namespace test_task.Models
{
    /// <summary>
    /// Модель описывающая сущность Заказа
    /// </summary>
    public partial class Order
    {
        public int OrderId { get; set; }

        public int UserId { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal OrderPrice { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }

}

