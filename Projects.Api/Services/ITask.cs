using Projects.Api.Models.Requests;
using Projects.Api.Models.Responses;
using System.Threading.Tasks;

namespace Projects.Api.Services
{
    public interface ITask
    {
        Task<Response> AddTask(TaskRequest request, int projectId);
        Task<Response> UpdateTask(TaskRequest request, int taskId);
        Task<Response> ChangeStateTask(ChangeStateRequest request, int taskId);
        Task<Response> DeleteProject(int taskId);
    }
}
