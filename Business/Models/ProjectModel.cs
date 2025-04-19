using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class ProjectModel
    {
        [Required]
        public string Id { get; set; } = null!;
        public string? ImageFileName { get; set; }
        [Required]
        public string ProjectName { get; set; } = null!;
        [Required]
        public ClientModel Client { get; set; } = null!;
        public string? Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? Budget { get; set; }
        [Required]
        public DateTime Created { get; set; }
        [Required]
        public UserModel User { get; set; } = null!;
        [Required]
        public StatusModel Status { get; set; } = null!;
    }
}
