using Microsoft.EntityFrameworkCore;
using Projects.Api.Data;
using Projects.Api.Entities;
using Projects.Api.Enums;
using Projects.Api.Models.Requests;
using Projects.Api.Models.Responses;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Projects.Api.Services
{
    public class Task : ITask
    {
        private readonly DataContext _context;

        public Task(DataContext context)
        {
            _context = context;
        }

        public async Task<Response> AddTask(TaskRequest request, int projectId)
        {
            try
            {
                ProjectEntity project = await _context.Projects
                    .Include(p=>p.Tasks)
                    .FirstOrDefaultAsync(p=>p.Id==projectId);
                if (project == null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "Project does not exist."
                    };
                }

                TaskEntity task = new()
                {
                    Name = request.Name,
                    Description = request.Description,
                    ExecutionDate = request.ExecutionDate,
                    State = State.Proceso,
                    IdProject =project.Id,
                    Project = project
                };

                project.Tasks.Add(task);                
                _context.Update(project);
                await _context.SaveChangesAsync();

                return new Response
                {
                    IsSuccess = true,
                    Message = "Task created successfully",
                    Result = new TaskResponse
                    {
                        TaskId = task.Id,
                        Name = task.Name,
                        Description = task.Description,
                        ExecutionDate = task.ExecutionDate,
                        ProjectId = projectId
                    }
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Response> ChangeStateTask(ChangeStateRequest request, int taskId)
        {
            try
            {
                TaskEntity task = await _context.Tasks.FindAsync(taskId);
                if (task == null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "Task does not exist."
                    };
                }

                ProjectEntity project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Tasks.FirstOrDefault(d => d.Id == task.Id) != null);
                task.IdProject = project.Id;
                task.State = request.Completed ? State.Finalizado : State.Proceso;
                _context.Update(task);
                await _context.SaveChangesAsync();

                return new Response
                {
                    IsSuccess = true,
                    Message = request.Completed ?
                        "Project Completed successfully" : "Project Open successfully"
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Response> DeleteProject(int taskId)
        {
            try
            {
                TaskEntity task = await _context.Tasks.FindAsync(taskId);
                if (task == null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "Task does not exist."
                    };
                }

                _context.Remove(task);
                await _context.SaveChangesAsync();

                return new Response
                {
                    IsSuccess = true,
                    Message = "Task Deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Response> UpdateTask(TaskRequest request, int taskId)
        {
            try
            {
                TaskEntity task = await _context.Tasks.FindAsync(taskId);
                if (task == null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "Task does not exist."
                    };
                }

                ProjectEntity project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Tasks.FirstOrDefault(d => d.Id == task.Id) != null);
                task.IdProject = project.Id;
                task.Name = request.Name;
                task.Description = request.Description;
                task.ExecutionDate = request.ExecutionDate;
                _context.Update(task);
                await _context.SaveChangesAsync();

                return new Response
                {
                    IsSuccess = true,
                    Message = "Task Updated successfully",
                    Result = new TaskResponse
                    {
                        TaskId = task.Id,
                        Name = task.Name,
                        Description = task.Description,
                        ExecutionDate = task.ExecutionDate,
                        ProjectId = task.IdProject
                    }
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
