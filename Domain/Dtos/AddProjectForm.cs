using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos
{
    public class AddProjectForm
    {
        public IFormFile? ImageFileName { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "Length be at least 2 characters.")]
        public string ProjectName { get; set; } = null!;
        [Required]
        public string ClientId { get; set; } = null!;
        public string? Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; } 
        [Required]
        public DateTime EndDate { get; set; } 
        public decimal? Budget { get; set; }
        [Required]
        public string UserId { get; set; } = null!;
    }
}
