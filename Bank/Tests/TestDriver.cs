using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Bank.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bank.Tests
{
    internal class TestDriver
    {
        public void RunAllTests()
        {
            TransactionTests();
            LedgerTests();
            Debug.WriteLine("All account tests passed");
        }

        private void LedgerTests()
        {
            //Test load
            var ledger = Ledger.Load("TestTransactions.json");
            Assert.IsTrue(ledger.Transactions.Count > 0);

            TestGetEntryFromLedger(ledger);
            TestLedgerSaves(ledger);
            TestDeposit(ledger);
            TestWithdrawl(ledger);
            TestCurrentAccountTotals(ledger);
        }

        private void TestDeposit(Ledger ledger)
        {
            var id = ledger.AddTransaction(100);
            ledger.Deposit(id, 200);
            Assert.AreEqual(300.0, ledger.GetBalance(id));

            try
            {
                ledger.Deposit(id, -1);
                Assert.Fail("Cannot deposit negative ammount");
            }
            catch (InvalidAmountException)
            {
            }

            try
            {
                ledger.Deposit("fakeaccount", 1);
                Assert.Fail("Cannot deposit into unknown account");
            }
            catch (InvalidFilterCriteriaException)
            {
            }
        }

        private void TestWithdrawl(Ledger ledger)
        {
            var id = ledger.AddTransaction(500);
            ledger.Withdraw(id, 100);
            Assert.AreEqual(400.0, ledger.GetBalance(id));

            try
            {
                ledger.Withdraw(id, -1);
                Assert.Fail("Cannot deposit negative ammount");
            }
            catch (InvalidAmountException)
            {
            }
        }

        private void TestCurrentAccountTotals(Ledger ledger)
        {
            var accounts = ledger.GetCurrentAccounts();
            foreach (var account in accounts)
            {
                Assert.IsTrue(accounts.Count(a => account.AccountId == a.AccountId) == 1);
            }
        }

        private void TestGetEntryFromLedger(Ledger ledger)
        {
            var id = ledger.AddTransaction(2000);

            //Get by id
            Assert.AreEqual(2000, ledger.GetBalance(id));
            //Partial match, unique
            Assert.AreEqual(2000, ledger.GetBalance(id.Substring(0, 6)));
            //Partial match, not unique
            try
            {
                ledger.GetBalance("1");
                Assert.Fail("Failed to throw exception on non-unique ID search");
            }
            catch (InvalidFilterCriteriaException)
            {
            }

            //Test get as foreign currency
            const double euroConv = 1.22;
            Assert.AreEqual(2000*euroConv, ledger.GetBalance(id, euroConv));
        }

        private void TestLedgerSaves(Ledger ledger)
        {
            var tempName = Guid.NewGuid() + ".json";
            ledger.Save(tempName);
            var tempFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", tempName);
            Assert.IsTrue(File.Exists(tempFile));
            File.Delete(tempFile);
        }

        private void TransactionTests()
        {
            //Add new account with $1000
            CreateValidTransaction(1000);

            //Add new account specific date
            CreateValidTransaction(1, new DateTime(2010, 5, 1).Ticks);

            //Try to create an account with an invalid balance
            CreateInvalidTransaction(-1);
        }

        private void CreateValidTransaction(double balance, long time = 0)
        {
            var acc = new Transaction(balance, time);
            Assert.AreEqual(balance, acc.Balance);
            Assert.IsTrue(!string.IsNullOrEmpty(acc.AccountId));
            Assert.IsTrue(acc.Time > 0);
        }

        private void CreateInvalidTransaction(double balance = 0, long time = 0, string id = "")
        {
            try
            {
                var acc = new Transaction(balance, time, id);
                Assert.Fail("Created an invalid account");
            }
            catch (InvalidBalanceException)
            {
            }
        }
    }
}