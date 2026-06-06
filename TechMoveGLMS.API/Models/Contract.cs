namespace TechMoveGLMS.API.Models
{
    public enum ContractStatus
    {
        Draft = 0,
        Active = 1,
        Expired = 2,
        OnHold = 3
    }

    public class Contract
    {
        public int ContractId { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ContractStatus Status { get; set; } = ContractStatus.Draft;
        public string ServiceLevel { get; set; } = string.Empty;
        public string SignedAgreementPath { get; set; } = string.Empty;
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    }
}