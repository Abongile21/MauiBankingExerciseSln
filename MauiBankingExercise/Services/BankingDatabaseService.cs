using MauiBankingExercise.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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
            string pathToDb = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(pathToDb, DbFileName);
        }

        private bool ExtractDbEmbeddedResource(string dbPath)
        {
            var assembly = typeof(BankingDatabaseService).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("MauiBankingExercise.EmbeddedDb.BankingApp.db");

            if (stream != null && !File.Exists(dbPath))
            {
                using (BinaryReader br = new BinaryReader(stream))
                using (FileStream fs = new FileStream(dbPath, FileMode.Create))
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    bw.Write(bytes);
                }
                return true;
            }
            return false;
        }

        public List<Customer> GetAllCustomers() => _dbConnection.Table<Customer>().ToList();

        public Customer GetCustomer(int customerId) => _dbConnection.Find<Customer>(customerId);

        public List<Account> GetCustomerAccounts(int customerId) =>
            _dbConnection.Table<Account>().Where(a => a.CustomerId == customerId).ToList();

        public List<Transaction> GetAccountTransactions(int accountId) =>
            _dbConnection.Table<Transaction>()
                        .Where(t => t.AccountId == accountId)
                        .OrderByDescending(t => t.TransactionDate)
                        .ToList();

        public void AddTransaction(int accountId, decimal amount, int transactionTypeId)
        {
            var account = _dbConnection.Find<Account>(accountId);
            if (account == null) throw new Exception("Account not found");

            if (transactionTypeId == 2 && amount > account.AccountBalance)
                throw new Exception("Insufficient balance for withdrawal");

            if (transactionTypeId == 1)
                account.AccountBalance += amount;
            else
                account.AccountBalance -= amount;

            _dbConnection.Update(account);

            var transaction = new Transaction
            {
                AccountId = accountId,
                Amount = amount,
                TransactionTypeId = transactionTypeId,
                TransactionDate = DateTime.Now,
                Description = transactionTypeId == 1 ? "Deposit" : "Withdrawal"
            };

            _dbConnection.Insert(transaction);
        }
    }
}