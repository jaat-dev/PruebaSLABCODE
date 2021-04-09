using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Projects.Api.Models.Requests
{
    public class ProjectRequest
    {
        [Required]
        [MaxLength(250, ErrorMessage = "The filed {0} must contain less than {1} characteres.")]
        public string Name { get; set; }

        [Required]
        [MaxLength(250, ErrorMessage = "The filed {0} must contain less than {1} characteres.")]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime EndDate { get; set; }
    }
}
