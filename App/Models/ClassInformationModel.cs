using System.ComponentModel.DataAnnotations;

// done manually
namespace App.Models
{
    public class ClassInformationModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Class Name is required.")]
        public string ClassName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Student count must be greater than 0.")]
        public int StudentCount { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }
    }
}
