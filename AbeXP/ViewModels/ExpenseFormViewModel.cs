

using AbeXP.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AbeXP.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using System.Collections.ObjectModel;

    namespace AbeXP.ViewModels
    {
        public partial class ExpenseFormViewModel : ObservableObject
        {
            [ObservableProperty]
            private DateTime date = DateTime.Now;

            [ObservableProperty]
            private decimal amount;

            [ObservableProperty]
            private string description;

            [ObservableProperty]
            private string selectedPaymentTypeId;

            public ObservableCollection<TagItemViewModel> Tags { get; } = new();

            public ObservableCollection<PaymentType> PaymentTypes { get; } = new();

            public ExpenseFormViewModel()
            {
                // Simulación de datos iniciales
                PaymentTypes.Add(new PaymentType { Id = "1", Name = "Tarjeta" });
                PaymentTypes.Add(new PaymentType { Id = "2", Name = "Efectivo" });
                PaymentTypes.Add(new PaymentType { Id = "3", Name = "Transferencia" });

                Tags.Add(new TagItemViewModel { Id = "1", Name = "Comida", ColorHex = "#FF5733" });
                Tags.Add(new TagItemViewModel { Id = "2", Name = "Ropa", ColorHex = "#33FF57" });
                Tags.Add(new TagItemViewModel { Id = "3", Name = "Transporte", ColorHex = "#3357FF" });
            }

            [RelayCommand]
            private void SaveExpense()
            {
                var expense = new Expense
                {
                    Date = Date,
                    Amount = Amount,
                    Description = Description,
                    PaymentTypeId = SelectedPaymentTypeId,
                    TagIds = Tags.Where(t => t.IsSelected).Select(t => t.Id).ToList()
                };

                // 🚧 Aquí después lo mandamos a Firebase
                Console.WriteLine($"Guardado: {expense.Amount} - {expense.Description}");
            }
        }
    }

    public class TagItemViewModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        public string ColorHex { get; set; } = "#FFFFFF";
        public bool IsSelected { get; set; } = false; // Para la selección en la UI
    }
}
