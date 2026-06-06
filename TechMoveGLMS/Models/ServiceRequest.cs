using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechMoveGLMS.Models
{
    public enum ServiceRequestStatus
    {
        Pending,
        Approved,
        Completed,
        Cancelled
    }

    public class ServiceRequest
    {
        public int ServiceRequestId { get; set; }

        [Required]
        public int ContractId { get; set; }
        public Contract Contract { get; set; } = null!;

        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal CostUSD { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CostZAR { get; set; }

        public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Pending;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}