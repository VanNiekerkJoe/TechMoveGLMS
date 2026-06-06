namespace TechMoveGLMS.API.Models
{
    public enum ServiceRequestStatus
    {
        Pending = 0,
        Approved = 1,
        Completed = 2,
        Cancelled = 3
    }

    public class ServiceRequest
    {
        public int ServiceRequestId { get; set; }
        public int ContractId { get; set; }
        public Contract Contract { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public decimal CostUSD { get; set; }
        public decimal CostZAR { get; set; }
        public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Pending;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}