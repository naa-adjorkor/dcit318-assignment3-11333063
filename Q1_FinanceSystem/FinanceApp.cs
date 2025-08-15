namespace Q1_FinanceSystem
{
    using System;

    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new();

        public void Run()
        {
            var acct = new SavingsAccount("1141010048377", 10000m);

            var t1 = new Transaction(1, DateTime.Today, 125.50m, "Groceries");
            var t2 = new Transaction(2, DateTime.Today.AddDays(-1), 300m, "Utilities");
            var t3 = new Transaction(3, DateTime.Today.AddDays(-2), 90m, "Entertainment");

            ITransactionProcessor p1 = new MobileMoneyProcessor();
            ITransactionProcessor p2 = new BankTransferProcessor();
            ITransactionProcessor p3 = new CryptoWalletProcessor();

            p1.Process(t1);
            p2.Process(t2);
            p3.Process(t3);

            acct.ApplyTransaction(t1);
            acct.ApplyTransaction(t2);
            acct.ApplyTransaction(t3);

            _transactions.AddRange(new[] { t1, t2, t3 });
        }

    }
}