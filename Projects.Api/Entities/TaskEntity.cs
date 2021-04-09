using Projects.Api.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projects.Api.Entities
{
    public class TaskEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        [StringLength(2500)]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ExecutionDate { get; set; }

        public State State { get; set; }

        [NotMapped]
        public int IdProject { get; set; }

        public ProjectEntity Project { get; set; }
    }
}
