using System;
using AbeXP.Models;

namespace AbeXP.Views.Templates
{
    public class TransactionTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ExpenseTemplate { get; set; }
        public DataTemplate LoanTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is Expense)
                return ExpenseTemplate;
            if (item is Loan)
                return LoanTemplate;

            return null;
        }
    }
}

