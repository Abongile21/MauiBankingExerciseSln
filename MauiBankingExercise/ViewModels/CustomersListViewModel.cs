using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using MauiBankingExercise.Models;
using MauiBankingExercise.Services;
using System.Runtime.CompilerServices;

namespace MauiBankingExercise.ViewModels
{
    public class CustomersListViewModel : INotifyPropertyChanged
    {
        private readonly BankingDatabaseService _service;
        private bool _isLoading;

        public ObservableCollection<Customer> Customers { get; set; }
        public ICommand SelectCustomerCommand { get; set; }


        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public CustomersListViewModel()
        {
            _service = new BankingDatabaseService();
            Customers = new ObservableCollection<Customer>();
            SelectCustomerCommand = new Command<SelectionChangedEventArgs>(OnCustomerSelected);
            LoadCustomers();
        }

        private async void LoadCustomers()
        {
            IsLoading = true;
            try
            {
                var customers = _service.GetAllCustomers();
                Customers.Clear();

                if (customers != null)
                {
                    foreach (var customer in customers)
                        Customers.Add(customer);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load customers: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private Customer? _selectedCustomer;

        public Customer? SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                _selectedCustomer = value;

                OnPropertyChanged(nameof(SelectedCustomer));
            }
        }



        
        private async void OnCustomerSelected(object obj)
        {
            if (SelectedCustomer != null)
            {
                var param = new ShellNavigationQueryParameters()
            {
                { "customerId", SelectedCustomer.CustomerId }
            };

                await AppShell.Current.GoToAsync($"customerview", param);
            }

        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
