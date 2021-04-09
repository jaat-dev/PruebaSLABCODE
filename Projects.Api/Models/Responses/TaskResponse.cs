using System;

namespace Projects.Api.Models.Responses
{
    public class TaskResponse
    {
        public int TaskId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime ExecutionDate { get; set; }
        public int ProjectId { get; set; }
    }
}
