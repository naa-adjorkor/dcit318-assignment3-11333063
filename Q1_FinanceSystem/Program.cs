namespace Q1_FinanceSystem
{   
    // a) Define core models using records
    public record Transaction (int Id, DateTime Date, decimal Amount, string Category);
    
    // b) Interface for transactions processing
    public interface ITransactionProcessor
    {
        void Process (Transaction transaction);
    }

    // c) Concrete processors
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process (Transaction transaction)
        {
            Console.WriteLine($"[BankTransfer] Debited {transaction.Amount} for {transaction.Category} on {transaction.Date} (Id: {transaction.Id})");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[MobileMoney] Sent {transaction.Amount} for {transaction.Category}. Ref #{transaction.Id}");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Crypto] Debited {transaction.Amount} USDT equivalent for {transaction.Category} on {transaction.Date} (Tx: {transaction.Id})");
        }
    }

    // d) Base Account
    public class Account
    {
        public string AccountNumber { get; init; }
        public decimal Balance { get; protected set; }

        public Account (string accountNumber, decimal initialBalance)
        {
            this.AccountNumber = accountNumber;
            this.Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"[Account] New Balance after {transaction.Category} : {Balance}");
        }
    }

    // e) Sealed SavingsAccount
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance) : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
                return;
            }
            Balance -= transaction.Amount;
            Console.WriteLine($"[Savings] Deducted {transaction.Amount:C}. Updated balance: {Balance:C}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var app = new FinanceApp();
            app.Run();
        }
    }
}
