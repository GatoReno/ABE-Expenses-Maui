using System;
using AbeXP.Models;

namespace AbeXP.Views.Templates
{
    public class TransactionTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ExpenseTemplate { get; set; }
        public DataTemplate IncomeTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is ExpenseTransactionModel)
                return ExpenseTemplate;
            if (item is IncomeTransactionModel)
                return IncomeTemplate;

            return null;
        }
    }
}

