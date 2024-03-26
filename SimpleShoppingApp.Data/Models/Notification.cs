﻿using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Data.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Text { get; set; } = null!;

        [Required]
        public string SenderUserId { get; set; } = null!;

        [Required]
        public string ReceiverUserId { get; set; } = null!;

        [Required]
        public ApplicationUser SenderUser { get; set; } = null!;

        [Required]
        public ApplicationUser ReceiverUser { get; set; } = null!;

        public string? Url { get; set; }

        public bool IsRead { get; set; }

        public bool IsDeleted { get; set; }
    }
}
