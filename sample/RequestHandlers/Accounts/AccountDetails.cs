namespace mediatr_test.RequestHandlers.Accounts
{
    public class AccountDetails
    {
        public string Id { get; set; }
        public string SortCode { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public bool Blocked { get; set; }
    }
}