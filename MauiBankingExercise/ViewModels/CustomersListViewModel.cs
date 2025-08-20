using MauiBankingExercise.Models;
using MauiBankingExercise.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MauiBankingExercise.ViewModels
{
    public class CustomersListViewModel
    {
        private readonly BankingDatabaseService _service;
        public ObservableCollection<Customer> Customers { get; set; }
        public ICommand SelectCustomerCommand { get; set; }

        public CustomersListViewModel()
        {
            _service = new BankingDatabaseService();
            Customers = new ObservableCollection<Customer>(_service.GetAllCustomers());
        }
    }
}