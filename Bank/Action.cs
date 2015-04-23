using System;
using System.Linq;
using Bank.Tests;

namespace Bank
{
    internal class Action
    {
        private readonly double _convRate;
        private readonly char _currency;
        private readonly Ledger _ledger;
        private readonly Options _options;

        public Action(Options options, Ledger ledger)
        {
            _options = options;
            _ledger = ledger;
            _convRate = _options.Euro ? .83 : 1.0;
            _currency = _options.Euro ? '€' : '$';
        }

        public string Run()
        {
            if (_options.Test)
            {
                new TestDriver().RunAllTests();
                return "Ran all tests successfully";
            }
            if (_options.NewAccount)
            {
                return NewAccount();
            }
            if (!string.IsNullOrEmpty(_options.BalanceId))
            {
                return Balance();
            }
            if (!string.IsNullOrEmpty(_options.Deposit))
            {
                return Deposit();
            }
            if (!string.IsNullOrEmpty(_options.Withdraw))
            {
                return Withdraw();
            }
            if (_options.Top)
            {
                return ListAccounts(limit: 5);
            }
            if (_options.Bottom)
            {
                return ListAccounts(false, 5);
            }
            if (_options.ListAll)
            {
                return ListAccounts();
            }
            return _options.GetUsage();
        }

        private string ListAccounts(bool desc = true, int limit = 0)
        {
            var accounts = _ledger.GetCurrentAccounts();
            accounts = desc ? accounts.OrderByDescending(a => a.Balance) : accounts.OrderBy(a => a.Balance);
            if (limit > 0)
            {
                accounts = accounts.Take(limit);
            }
            var output = string.Format("{0, -35} | {1, -20}\n", "Account ID", "Balance");
            output += new string('-', 55) + "\n";
            foreach (var account in accounts)
            {
                output += string.Format("{0, -35} | {1}{2:0.00}\n", account.AccountId, _currency,
                    account.Balance*_convRate);
            }
            return output;
        }

        private string Deposit()
        {
            _ledger.Deposit(_options.Deposit, _options.Amount);
            return string.Format("Deposited ${0} in account {1}", _options.Amount, _options.Deposit);
        }

        private string Withdraw()
        {
            _ledger.Withdraw(_options.Withdraw, _options.Amount);
            return string.Format("Withdrew ${0} from account {1}", _options.Amount, _options.Withdraw);
        }

        private string NewAccount()
        {
            var accId = _ledger.AddTransaction(0);
            return string.Format("Created new account with ID: " + accId);
        }

        private string Balance()
        {
            var output = string.Format("{0, -35} | {1, -20}\n", "Account ID", "Balance");
            output += new string('-', 55) + "\n";
            if (!string.IsNullOrEmpty(_options.Date))
            {
                try
                {
                    var dt = Convert.ToDateTime(_options.Date);
                    var balance = _ledger.GetBalance(_options.BalanceId, dt, _convRate);
                    output += string.Format("{0, -35} | {1}{2:0.00}\n", _options.BalanceId, _currency, balance);
                }
                catch (Exception)
                {
                    return "No balance found for that ID and date.";
                }
            }
            else
            {
                output += string.Format("{0, -35} | {1}{2:0.00}\n", _options.BalanceId, _currency,
                    _ledger.GetBalance(_options.BalanceId, _convRate));
            }
            return output;
        }
    }
}