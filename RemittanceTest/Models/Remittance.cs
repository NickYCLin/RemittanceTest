namespace RemittanceTest.Models
{
    public class Remittance
    {
        public int Id { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
        // 0:待覆核, 1:交易中, 2:已完成, 9:已取消
        public int Status { get; set; }
    }
}
