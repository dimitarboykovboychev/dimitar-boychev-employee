namespace TeamLongestPeriod.Models
{
    public class Employee
    {
        public int ID { get; set; }

        public int EmployeeID { get; set; }

        public int ProjectID { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }
    }
}
