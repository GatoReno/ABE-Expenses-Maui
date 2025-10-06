using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Models
{
    public class PaymentMethod : BaseEntity
    {
        public string Name { get; set; } = ""; // Ej: "Tarjeta", "Crédito", "Transferencia"
        public string Details { get; set; } = ""; // Ej: número de tarjeta, banco, etc.
    }
}
