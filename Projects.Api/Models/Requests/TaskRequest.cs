using System;
using System.ComponentModel.DataAnnotations;

namespace Projects.Api.Models.Requests
{
    public class TaskRequest
    {
        [Required]
        [MaxLength(250, ErrorMessage = "The filed {0} must contain less than {1} characteres.")]
        public string Name { get; set; }

        [MaxLength(2500, ErrorMessage = "The filed {0} must contain less than {1} characteres.")]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ExecutionDate { get; set; }
    }
}
