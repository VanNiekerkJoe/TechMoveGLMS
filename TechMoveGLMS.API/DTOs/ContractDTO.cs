namespace TechMoveGLMS.API.DTOs
{
    public class ContractDTO
    {
        public int ContractId { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ServiceLevel { get; set; } = string.Empty;
        public string SignedAgreementPath { get; set; } = string.Empty;
    }

    public class CreateContractDTO
    {
        public int ClientId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Status { get; set; }
        public string ServiceLevel { get; set; } = string.Empty;
    }

    public class UpdateContractStatusDTO
    {
        public int Status { get; set; }
    }
}