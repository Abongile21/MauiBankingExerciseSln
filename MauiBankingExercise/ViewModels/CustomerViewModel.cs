using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using MauiBankingExercise.Models;
using MauiBankingExercise.Services;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

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

        public ObservableCollection<Account> Accounts { get; set; }
        public ObservableCollection<string> TransactionTypes { get; set; }
        public ICommand SubmitTransactionCommand { get; set; }
        public ICommand RefreshCommand { get; set; }

        private int _customerId;
        public int CustomerId
        {
            get => _customerId;
            set
            {
                _customerId = value;
                OnPropertyChanged();
                if (_customerId != 0)
                {
                    _ = LoadCustomerData();
                }
            }
        }

        public Customer Customer
        {
            get => _customer;
            set
            {
                _customer = value;
                OnPropertyChanged();
            }
        }

        public Account SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                _selectedAccount = value;
                OnPropertyChanged();
            }
        }

        public string SelectedTransactionType
        {
            get => _selectedTransactionType;
            set
            {
                _selectedTransactionType = value;
                OnPropertyChanged();
            }
        }

        public string TransactionAmount
        {
            get => _transactionAmount;
            set
            {
                _transactionAmount = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public CustomerViewModel()
        {
            _service = new BankingDatabaseService();
            Accounts = new ObservableCollection<Account>();
            TransactionTypes = new ObservableCollection<string> { "Deposit", "Withdrawal" };

            SubmitTransactionCommand = new Command(async () => await SubmitTransaction(), CanSubmitTransaction);
            RefreshCommand = new Command(async () => await LoadCustomerData());
        }

        public async Task LoadCustomerData()
        {
            if (CustomerId == 0) return;

            IsLoading = true;
            try
            {
                Customer = _service.GetCustomer(CustomerId);
                var accounts = _service.GetCustomerAccounts(CustomerId);

                Accounts.Clear();
                if (accounts != null)
                {
                    foreach (var account in accounts)
                        Accounts.Add(account);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load customer data: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SubmitTransaction()
        {
            if (!CanSubmitTransaction()) return;

            if (!decimal.TryParse(TransactionAmount, out decimal amount) || amount <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please enter a valid amount", "OK");
                return;
            }

            int typeId = SelectedTransactionType == "Deposit" ? 1 : 2;

            IsLoading = true;
            try
            {
                _service.AddTransaction(SelectedAccount.AccountId, amount, typeId);

                await LoadCustomerData();

                TransactionAmount = "";
                SelectedAccount = null;
                SelectedTransactionType = null;

                await Application.Current.MainPage.DisplayAlert("Success", "Transaction completed successfully", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanSubmitTransaction()
        {
            return SelectedAccount != null &&
                   !string.IsNullOrWhiteSpace(SelectedTransactionType) &&
                   !string.IsNullOrWhiteSpace(TransactionAmount) &&
                   !IsLoading;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            if (propertyName == nameof(SelectedAccount) ||
                propertyName == nameof(SelectedTransactionType) ||
                propertyName == nameof(TransactionAmount) ||
                propertyName == nameof(IsLoading))
            {
                ((Command)SubmitTransactionCommand).ChangeCanExecute();
            }
        }
    }
}
