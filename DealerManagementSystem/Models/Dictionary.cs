namespace DealerManagementSystem.Models
{
    public class Dictionary
    {
        public enum DMSSecondarySalesOrderStatus
        {
            Submit = 0,
            Send_To_HO = 1,
            Approved_By_HO = 2,
            Reject_By_HO = 3,
            Partial_Invoiced = 4,
            Invoiced = 5
        }
    }
}