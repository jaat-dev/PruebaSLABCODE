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
    public class ProjectsController : ControllerBase
    {
        private readonly IProject _project;

        public ProjectsController(IProject project)
        {
            _project = project;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("AddProject")]
        public async Task<IActionResult> PostProject(ProjectRequest request)
        {
            Response result = await _project.AddProject(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("UpdateProject/{id}")]
        public async Task<IActionResult> PutProject(ProjectRequest request, int id)
        {
            Response result = await _project.GetProjectById(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            result = await _project.UpdateProject(request, id);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("DeleteProject/{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            Response result = await _project.GetProjectById(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            result = await _project.DeleteProject(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("CompleteProject/{id}")]
        public async Task<IActionResult> CompleteProject(ChangeStateRequest request, int id)
        {
            Response result = await _project.GetProjectById(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            result = await _project.ChangeStateProject(request, id);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }
    }
}
