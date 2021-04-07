﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Projects.Api.Entities
{
    public class TaskEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ExecutionDate { get; set; }

        public ProjectEntity Project { get; set; }
    }
}
