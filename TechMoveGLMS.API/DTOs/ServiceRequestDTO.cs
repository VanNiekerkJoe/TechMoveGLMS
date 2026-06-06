namespace TechMoveGLMS.API.DTOs
{
    public class ServiceRequestDTO
    {
        public int ServiceRequestId { get; set; }
        public int ContractId { get; set; }
        public string ContractName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal CostUSD { get; set; }
        public decimal CostZAR { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    public class CreateServiceRequestDTO
    {
        public int ContractId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal CostUSD { get; set; }
        public decimal CostZAR { get; set; }
    }
}