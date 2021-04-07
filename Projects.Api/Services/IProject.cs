using Projects.Api.Entities;
using Projects.Api.Models.Requests;
using Projects.Api.Models.Responses;
using System.Threading.Tasks;

namespace Projects.Api.Services
{
    public interface IProject
    {
        Task<Response> AddProject(ProjectRequest request);
        Task<Response> UpdateProject(ProjectRequest request, int id);
        Task<Response> GetProjectById(int id);
        Task<Response> DeleteProject(int id);
        Task<Response> UpdateStateProject(ChangeStateProjectRequest request, int id);
    }
}
