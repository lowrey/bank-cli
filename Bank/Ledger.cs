using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Bank.Exceptions;
using Newtonsoft.Json;

namespace Bank
{
    internal class Ledger : Serializable
    {
        public List<Transaction> Transactions { get; set; }

        public string AddTransaction(double balance, long time = 0, string accountId = "")
        {
            var transaction = new Transaction(balance, time, accountId);
            Transactions.Add(transaction);
            return transaction.AccountId;
        }

        public static Ledger Load(string filename = "Transactions.json")
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", filename);
            return JsonConvert.DeserializeObject<Ledger>(File.ReadAllText(path));
        }

        public void Save(string filename = "Transactions.json")
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", filename);
            File.WriteAllText(path, ToJson());
        }

        private IEnumerable<Transaction> GetAccount(string id)
        {
            var results = Transactions.Where(t => t.AccountId.StartsWith(id));
            if (!results.Any())
            {
                throw new InvalidFilterCriteriaException("Account ID not found");
            }
            //see if id is unique
            var rId = results.First().AccountId;
            if (!results.All(t => t.AccountId == rId))
            {
                throw new InvalidFilterCriteriaException("ID too broad, returned multiple accounts");
            }
            return results;
        }

        //Return the latest balance from a given account id
        public double GetBalance(string id, double conversion = 1.0)
        {
            var accounts = GetAccount(id);
            //Return the latest transaction of the given id
            var latest = accounts.Max(t => t.Time);
            return accounts.Last(t => t.Time == latest).Balance*conversion;
        }

        //Return the latest balance from a given account id and a specific date
        public double GetBalance(string id, DateTime date, double conversion = 1.0)
        {
            var accounts = GetAccount(id).Where(a => new DateTime(a.Time).Date <= date.Date);
            //Return the latest transaction of the given id
            var latest = accounts.Max(t => t.Time);
            return accounts.Last(t => t.Time == latest).Balance*conversion;
        }

        //return all latest state of each account
        public IEnumerable<Transaction> GetCurrentAccounts()
        {
            return Transactions.OrderByDescending(t => t.Time).GroupBy(t => t.AccountId).Select(t => t.First());
        }

        public void Deposit(string accountId, double amount, long time = 0)
        {
            if (amount <= 0)
            {
                throw new InvalidAmountException();
            }
            var fullId = GetAccount(accountId).First().AccountId;
            var balance = GetBalance(fullId);
            AddTransaction(balance + amount, time, fullId);
        }

        public void Withdraw(string accountId, double amount, long time = 0)
        {
            if (amount <= 0)
            {
                throw new InvalidAmountException();
            }
            var fullId = GetAccount(accountId).First().AccountId;
            var balance = GetBalance(fullId);
            balance -= amount;
            if (balance < 0)
            {
                throw new InvalidBalanceException();
            }
            AddTransaction(balance, time, fullId);
        }
    }
}