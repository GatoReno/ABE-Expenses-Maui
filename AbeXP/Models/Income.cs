using AbeXP.Interfaces;
using AbeXP.Common.Enum;
using System;
using System.Collections.Generic;

namespace AbeXP.Models
{
    public class Income : TransactionModel, IAmTransaction
    {
        public Income()
        {
            Type = TransactionType.Income;
        }
    }
}
