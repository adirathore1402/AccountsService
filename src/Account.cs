namespace AccountsService
{
    public class Account
    {
        public string Name { get; set; }
        public string Password { get; set; }

        public Guid id { get; set; }
        // are we sure we want to expose the balance with set?
        public double Balance {  get; set; }
        public DateTime OpeningDate {  get; set; }

        public List<Transaction> Transactions { get; set; }
    }
}
