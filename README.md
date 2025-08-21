# MauiBankingExercise

## Overview

**MauiBankingExercise** is a cross-platform .NET MAUI application that demonstrates the Model-View-ViewModel (MVVM) pattern in a simple banking scenario. The app allows users to select a customer, view their accounts, and perform transactions (deposits and withdrawals) with real-time updates.

This project is designed as an assignment for students to practice MVVM, data binding, navigation, and basic database operations in .NET MAUI.

---

## Assignment Requirements

You are to complete the following features:

### 1. Customer Selection (No Login Required)
- **Screen:** "Select Customer" or simple login.
- **Functionality:**
  - Display a list of customers from the database using a `CollectionView` or `ListView`.
  - When a customer is selected, navigate to their dashboard.

### 2. Customer Dashboard
- **Screen:** Customer Dashboard.
- **Functionality:**
  - Show the selected customer’s name.
  - List all their accounts (with balances) using a `CollectionView`.
  - Allow drill-down into a specific account.
  - Add the ability to make a transaction in an account.

### 3. Transaction Functionality

#### Viewing Transactions
- When an account is selected, display a list of all past transactions for that account.

#### Making Transactions
- UI for entering a transaction amount and selecting the type (Deposit or Withdrawal).
- Validates transaction (e.g., cannot withdraw more than the current balance).
- On submit:
  - Create a new transaction in the database.
  - Update the account balance accordingly.
  - Refresh the list of transactions and account balance.

---

## Project Structure

- **Models:**  
  - `Bank.cs`, `Customer.cs`, `CustomerType.cs`, `Account.cs`, `AccountType.cs`, `Transaction.cs`, `TransactionType.cs`, `Auth.cs`, `AuthType.cs`, `Asset.cs`, `AssetType.cs`  
  Define the data structures for the banking domain.

- **Services:**  
  - `DatabaseSeederService.cs`
  Handles database seeding and initial data population.
  - `BankingDatabaseService.cs`
  Handles interactions with the sqlite3 database

- **Views:**  
 - `CustomersListView.cs`
 -  `CustomerView.cs`

- **ViewModels:**  
 - `CustomersListViewModel.cs`
 - `CustomerViewModel.cs`

---

## Technologies Used

- **.NET 9**
- **.NET MAUI** (Multi-platform App UI)
- **MVVM Pattern**
- **SQLite** (via `sqlite-net-pcl` and `SQLiteNetExtensions`)

---

## Getting Started

1. **Clone the repository.**
2. **Open the solution in Visual Studio 2022 (or later) with .NET 9 and MAUI workloads installed.**
3. **Restore NuGet packages.**
4. **Build and run the project.**
5. **Implement the required features as described above.**

---

## Tips for Completing the Assignment

- Use `CollectionView` for displaying lists (customers, accounts, transactions).
- Use data binding and commands in your ViewModels for MVVM compliance.
- Use navigation patterns provided by MAUI (e.g., `Shell` or `NavigationPage`).
- Validate user input before processing transactions.
- Update the UI after database changes (e.g., after a transaction).
- Refer to the provided models and services for database structure and seeding.

---

## Useful Resources

- [Official .NET MAUI Documentation](https://learn.microsoft.com/dotnet/maui/)
- [MVVM in .NET MAUI](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)
- [sqlite-net-pcl Documentation](https://github.com/praeclarum/sqlite-net)
- [SQLiteNetExtensions Documentation](https://bitbucket.org/twincoders/sqlite-net-extensions)

---

## License

This project is for educational purposes.
