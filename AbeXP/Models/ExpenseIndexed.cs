using AbeXP.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Models
{
    internal class ExpenseIndexed : Expense
    {

        public ExpenseIndexed(Expense expense) 
        {
            Id = expense.Id;
            Date = expense.Date;
            Amount = expense.Amount;
            Description = expense.Description;
            PaymentTypeId = expense.PaymentTypeId;
            TagIds = expense.TagIds;
            UserId = expense.UserId;
        }

        public string UserId_Date => $"{UserId}_{Date.ToString(DateConstants.IndexDateFormat)}";
    }
}
