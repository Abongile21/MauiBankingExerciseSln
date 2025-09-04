using MauiBankingExercise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MauiBankingExercise.Services
{
    public class BankingApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7258/api/";

        public BankingApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Customer>>("customers");
        }

        public async Task<Customer> GetCustomerAsync(int customerId)
        {
            return await _httpClient.GetFromJsonAsync<Customer>($"customers/{customerId}");
        }

        public async Task<List<Account>> GetCustomerAccountsAsync(int customerId)
        {
            var accounts = await _httpClient.GetFromJsonAsync<List<Account>>("accounts");
            return accounts?
                .Where(a => a.CustomerId == customerId)
                .ToList() 
                ?? new List<Account>();
        }

        public async Task<List<Transaction>> GetAccountTransactionsAsync(int accountId, int limit = 10)
        {
            var transactions = await _httpClient.GetFromJsonAsync<List<Transaction>>("transactions");
            return transactions?
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.TransactionDate)
                .Take(limit)
                .ToList() 
                ?? new List<Transaction>();
        }

        public async Task AddTransactionAsync(int accountId, decimal amount, int transactionTypeId)
        {
            var transaction = new Transaction
            {
                AccountId = accountId,
                Amount = amount,
                TransactionTypeId = transactionTypeId,
                TransactionDate = DateTime.Now
            };

            var response = await _httpClient.PostAsJsonAsync("transactions", transaction);
            response.EnsureSuccessStatusCode();
        }

        public async Task<string> GetBankNameAsync(int bankId)
        {
            var bank = await _httpClient.GetFromJsonAsync<Bank>($"banks/{bankId}");
            return bank?.BankName ?? "Unknown Bank";
        }
    }
}
