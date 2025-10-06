using System;
using System.Collections.Generic;

namespace AbeXP.Models
{
    public class Income : BaseUserOwnerEntity
    {
        public DateTime Date { get; set; } = DateTime.Now;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? PaymentTypeId { get; set; }
        public List<string> TagIds { get; set; } = new();
    }
}
