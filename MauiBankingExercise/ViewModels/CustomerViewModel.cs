using MauiBankingExercise.Models;
using MauiBankingExercise.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MauiBankingExercise.ViewModels
{

    [QueryProperty(nameof(CustomerId), "customerId")]
    public class CustomerViewModel : INotifyPropertyChanged
    {
        private readonly BankingDatabaseService _service;
        private Customer _customer;
        private Account _selectedAccount;
        private string _selectedTransactionType;
        private string _transactionAmount;
        private bool _isLoading;
        private int _customerId;


        public ObservableCollection<Account> Accounts { get; } = new();
        public ObservableCollection<Transaction> RecentTransactions { get; } = new();
        public ObservableCollection<string> TransactionTypes { get; } = new() { "Deposit", "Withdrawal" };

        public ICommand SubmitTransactionCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ViewTransactionsCommand { get; }

        public int CustomerId
        {
            get => _customerId;
            set
            {
                if (_customerId != value)
                {
                    _customerId = value;
                    OnPropertyChanged();
                    _ = LoadCustomerData();
                }
            }
        }

        public Customer Customer
        {
            get => _customer;
            set { _customer = value; OnPropertyChanged(); OnPropertyChanged(nameof(CustomerName)); }
        }

        public string CustomerName => Customer == null ? "Unknown Customer" : $"{Customer.FirstName} {Customer.LastName}";
        
        public Account SelectedAccount
        {
            get => _selectedAccount;
            set { _selectedAccount = value; OnPropertyChanged(); _ = LoadAccountTransactions(); }
        }

        public string SelectedTransactionType
        {
            get => _selectedTransactionType;
            set { _selectedTransactionType = value; OnPropertyChanged(); }
        }

        public string TransactionAmount
        {
            get => _transactionAmount;
            set { _transactionAmount = value; OnPropertyChanged(); }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public CustomerViewModel()


        {

            _service = new BankingDatabaseService();
            SubmitTransactionCommand = new Command(async () => await SubmitTransaction(), CanSubmitTransaction);
            RefreshCommand = new Command(async () => await LoadCustomerData());
            ViewTransactionsCommand = new Command<Account>(async acc => await ViewAccountTransactions(acc));
        }



        public async Task LoadCustomerData()
        {
            if (CustomerId == 0) return;
            IsLoading = true;

            try
            {
                Customer =  _service.GetCustomer(CustomerId);

                Accounts.Clear();
                var accounts = _service.GetCustomerAccounts(CustomerId);
                foreach (var acc in accounts)
                {
                    acc.AccountType ??= _service.GetAccountType(acc.AccountTypeId);
                    Accounts.Add(acc);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load: {ex.Message}", "OK");
            }
            finally { IsLoading = false; }
        }


        private async Task LoadAccountTransactions()
        {
            if (SelectedAccount == null) return;

            try
            {
                RecentTransactions.Clear();
                var transactions =_service.GetAccountTransactions(SelectedAccount.AccountId);
                foreach (var tx in transactions)
                {
                    tx.TransactionType ??=  _service.GetTransactionType(tx.TransactionTypeId);
                    RecentTransactions.Add(tx);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load: {ex.Message}", "OK");
            }
        }

        private async Task SubmitTransaction()
        {
            if (!CanSubmitTransaction()) return;

            if (!decimal.TryParse(TransactionAmount, out var amount) || amount <= 0)
            {
                await Shell.Current.DisplayAlert("Error", "Enter a valid amount > 0", "OK");
                return;
            }

            string typeLabel = SelectedTransactionType;
            int typeId = typeLabel == "Deposit" ? 1 : 2;
            IsLoading = true;

            try
            {
                _service.AddTransaction(SelectedAccount.AccountId, amount, typeId);
                await LoadCustomerData();
                await LoadAccountTransactions();

                TransactionAmount = "";
                SelectedTransactionType = null;

                await Shell.Current.DisplayAlert("Success",
                    $"{typeLabel}: {amount:C} completed!", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally { IsLoading = false; }
        }

        private async Task ViewAccountTransactions(Account account)
        {
            if (account == null) return;

            try
            {
                var txs = _service.GetAccountTransactions(account.AccountId);
                var list = string.Join("\n", txs.Select(t =>
                    $"{t.TransactionDate:MM/dd/yyyy} - {_service.GetTransactionTypeName(t.TransactionTypeId)}: {t.Amount:C}"));

                await Shell.Current.DisplayAlert("Transactions",
                    $"Account: {account.AccountNumber}\nBalance: {account.AccountBalance:C}\n\n{list}", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private bool CanSubmitTransaction() =>
            SelectedAccount != null &&
            !string.IsNullOrWhiteSpace(SelectedTransactionType) &&
            !string.IsNullOrWhiteSpace(TransactionAmount) &&
            !IsLoading;


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            if (name is nameof(SelectedAccount) or nameof(SelectedTransactionType) or nameof(TransactionAmount) or nameof(IsLoading))
                ((Command)SubmitTransactionCommand).ChangeCanExecute();
        }
    }
}
