using MauiBankingExercise.Models;
using MauiBankingExercise.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;


namespace MauiBankingExercise.ViewModels
{
    public class CustomerViewModel : BindableObject
    {
        private readonly BankingDataService _service;

        public Customer Customer { get; set; }
        public ObservableCollection<Account> Accounts { get; set; }
        public ObservableCollection<string> TransactionTypes { get; set; }
        public Account SelectedAccount { get; set; }
        public string SelectedTransactionType { get; set; }
        public string TransactionAmount { get; set; }

        public ICommand SubmitTransactionCommand { get; set; }

        public CustomerViewModel(int customerId)
        {
            _service = new BankingDataService();
            Customer = _service.GetCustomer(customerId);
            Accounts = new ObservableCollection<Account>(_service.GetCustomerAccounts(customerId));

            TransactionTypes = new ObservableCollection<string> { "Deposit", "Withdrawal" };
            SubmitTransactionCommand = new Command(SubmitTransaction);
        }

        private void SubmitTransaction()
        {
            if (SelectedAccount == null || string.IsNullOrWhiteSpace(SelectedTransactionType) || string.IsNullOrWhiteSpace(TransactionAmount))
                return;

            if (!decimal.TryParse(TransactionAmount, out decimal amount))
                return;

            int typeId = SelectedTransactionType == "Deposit" ? 1 : 2;

            try
            {
                _service.AddTransaction(SelectedAccount.AccountId, amount, typeId);
                Accounts = new ObservableCollection<Account>(_service.GetCustomerAccounts(Customer.CustomerId));
                OnPropertyChanged(nameof(Accounts));
                TransactionAmount = "";
                OnPropertyChanged(nameof(TransactionAmount));
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}