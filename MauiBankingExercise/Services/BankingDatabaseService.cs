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
        private readonly SQLiteConnection _db;
        private static string DbFileName = "BankingApp.db";

        public BankingDatabaseService()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DbFileName);

            bool dbExists = File.Exists(dbPath);
            _db = new SQLiteConnection(dbPath);

            if (!dbExists)
            {
                BankingSeeder.Seed(_db);
            }
        }

        public List<Customer> GetAllCustomers() => _db.Table<Customer>().ToList();

        public Customer GetCustomer(int customerId) => _db.Find<Customer>(customerId);

        public List<Account> GetCustomerAccounts(int customerId) =>
            _db.Table<Account>().Where(a => a.CustomerId == customerId).ToList();

        public List<Transaction> GetAccountTransactions(int accountId) =>
            _db.Table<Transaction>().Where(t => t.AccountId == accountId).OrderByDescending(t => t.TransactionDate).ToList();

        public void AddTransaction(int accountId, decimal amount, int transactionTypeId)
        {
            var account = _db.Find<Account>(accountId);
            if (account == null) throw new Exception("Account not found");

            if (transactionTypeId == 2 && amount > account.AccountBalance)
                throw new Exception("Insufficient balance for withdrawal");

            if (transactionTypeId == 1)
                account.AccountBalance += amount;
            else
                account.AccountBalance -= amount;

            _db.Update(account);

            var transaction = new Transaction
            {
                AccountId = accountId,
                Amount = amount,
                TransactionTypeId = transactionTypeId,
                TransactionDate = DateTime.Now,
                Description = transactionTypeId == 1 ? "Deposit" : "Withdrawal"
            };
            _db.Insert(transaction);
        }
    }
}