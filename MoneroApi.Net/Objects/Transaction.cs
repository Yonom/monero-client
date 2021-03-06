﻿namespace Jojatekok.MoneroAPI
{
    public class Transaction
    {
        public TransactionType Type { get; private set; }
        public bool IsAmountSpendable { get; private set; }
        public double Amount { get; internal set; }
        public string TransactionId { get; private set; }
        public int Number { get; private set; }

        public Transaction(TransactionType type, bool isAmountSpendable, double amount, string transactionId, int number)
        {
            Type = type;
            IsAmountSpendable = isAmountSpendable;
            Amount = amount;
            TransactionId = transactionId;
            Number = number;
        }

        public Transaction(TransactionType type, bool isAmountSpendable, double amount, string transactionId) : this(type, isAmountSpendable, amount, transactionId, -1)
        {

        }
    }
}
