using System;
using Bank.Exceptions;
using Newtonsoft.Json;

namespace Bank
{
    internal class Transaction : Serializable
    {
        public Transaction(double balance, long time = 0, string accountId = "")
        {
            if (balance < 0)
            {
                throw new InvalidBalanceException();
            }
            Balance = balance;
            AccountId = string.IsNullOrEmpty(accountId) ? GenId() : accountId;
            Time = time <= 0 ? DateTime.Now.Ticks : time;
        }

        public Transaction()
        {
        }

        public string AccountId { get; set; }
        public double Balance { get; set; }
        public long Time { get; set; }

        [JsonIgnore]
        public string RoundedBalance
        {
            get { return string.Format("{0:0.00}", Balance); }
        }

        private static string GenId()
        {
            var encoded = Guid.NewGuid().ToString();
            encoded = encoded.Replace("-", "");
            return encoded;
        }
    }
}