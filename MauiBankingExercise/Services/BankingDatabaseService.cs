using MauiBankingExercise.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MauiBankingExercise.Services
{
    public class BankingDatabaseService
    {
        private readonly SQLiteConnection _dbConnection;
        private static string DbFileName = "BankingApp.db";

        public BankingDatabaseService()
        {
            var dbPath = GetDatabasePath();
            bool dbExists = File.Exists(dbPath);

            if (!dbExists)
            {
                File.Create(dbPath).Dispose();
            }

            _dbConnection = new SQLiteConnection(dbPath);

            if (!dbExists)
            {
                BankingSeeder.Seed(_dbConnection);
            }
        }

        private string GetDatabasePath()
        {
            var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(basePath, DbFileName);
        }

        public List<Customer> GetAllCustomers() => _dbConnection.Table<Customer>().ToList();
        public Customer GetCustomer(int customerId) => _dbConnection.Table<Customer>().FirstOrDefault(c => c.CustomerId == customerId);

        public List<Account> GetCustomerAccounts(int customerId) =>
            _dbConnection.Table<Account>().Where(a => a.CustomerId == customerId).ToList();

        public List<Transaction> GetAccountTransactions(int accountId) =>
            GetAccountTransactions(accountId, 10);

        public List<Transaction> GetAccountTransactions(int accountId, int limit) =>
            _dbConnection.Table<Transaction>()
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.TransactionDate)
                .Take(limit)
                .ToList();

        public AccountType GetAccountType(int accountTypeId) =>
            _dbConnection.Table<AccountType>().FirstOrDefault(t => t.AccountTypeId == accountTypeId);

        public TransactionType GetTransactionType(int transactionTypeId) =>
            _dbConnection.Table<TransactionType>().FirstOrDefault(t => t.TransactionTypeId == transactionTypeId);

        public string GetTransactionTypeName(int transactionTypeId) =>
            GetTransactionType(transactionTypeId)?.Name ?? "Unknown";

        public void AddTransaction(int accountId, decimal amount, int transactionTypeId)
        {
            var account = _dbConnection.Table<Account>().FirstOrDefault(a => a.AccountId == accountId);
            if (account == null) throw new Exception("Account not found");
            if (transactionTypeId == 2 && account.AccountBalance < amount)
                throw new Exception("Insufficient balance");

            if (transactionTypeId == 1) account.AccountBalance += amount;
            if (transactionTypeId == 2) account.AccountBalance -= amount;

            _dbConnection.Update(account);

            var transaction = new Transaction
            {
                AccountId = accountId,
                Amount = amount,
                TransactionTypeId = transactionTypeId,
                TransactionDate = DateTime.Now
            };

            _dbConnection.Insert(transaction);
        }
    }
}
