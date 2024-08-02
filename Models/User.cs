using System;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;

namespace test_task.Models
{
    /// <summary>
    /// Модель описывающая сущность Пользователя
    /// </summary>
    public partial class User
    {
        public int UserId { get; set; }

        public string UserName { get; set; } = null!;

        public string UserLogin { get; set; } = null!;

        public string UserPassword { get; set; } = null!;
        
        public UserRole UserRole { get; set; }
        
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }

    public enum UserRole
    {
        Admin,
        User
    }
}
