using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projects.Api.Models.Requests;
using Projects.Api.Models.Responses;
using Projects.Api.Services;
using System.Threading.Tasks;

namespace Projects.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITask _task;

        public TasksController(ITask task)
        {
            _task = task;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("AddTask/{id}")]
        public async Task<IActionResult> PostTask(TaskRequest request, int id)
        {
            Response result = await _task.AddTask(request, id);
            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }

            return Ok(result);
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("UpdateTask/{id}")]
        public async Task<IActionResult> PutTask(TaskRequest request, int id)
        {
            Response result = await _task.UpdateTask(request, id);            
            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }

            return Ok(result);
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("CompleteTask/{id}")]
        public async Task<IActionResult> CompleteTask(ChangeStateRequest request, int id)
        {
            Response result = await _task.ChangeStateTask(request, id);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("DeleteTask/{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            Response result = await _task.DeleteProject(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }
    }
}
