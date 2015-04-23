using System;

namespace Bank.Exceptions
{
    class InvalidBalanceException : Exception
    {
        /*
        public InvalidBalanceException(string invalidDepositAmount) : base(invalidDepositAmount)
        {
        }
        */

        public override string Message
        {
            get
            {
                return "This transaction has created an invalid balance";
            }
        }
    }
}
