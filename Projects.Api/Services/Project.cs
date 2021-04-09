using Microsoft.EntityFrameworkCore;
using Projects.Api.Data;
using Projects.Api.Entities;
using Projects.Api.Enums;
using Projects.Api.Models.Requests;
using Projects.Api.Models.Responses;
using System;
using System.Threading.Tasks;

namespace Projects.Api.Services
{
    public class Project : IProject
    {
        private readonly DataContext _context;

        public Project(DataContext context)
        {
            _context = context;
        }

        public async Task<Response> AddProject(ProjectRequest request)
        {
            try
            {
                ProjectEntity project = new()
                {
                    Name = request.Name,
                    Description = request.Description,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    State = State.Proceso
                };

                await _context.AddAsync(project);
                await _context.SaveChangesAsync();

                return new Response
                {
                    IsSuccess = true,
                    Message = "Project created successfully",
                    Result = project
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

        public async Task<Response> UpdateProject(ProjectRequest request, int id)
        {
            try
            {
                ProjectEntity project = await _context.Projects.FindAsync(id);
                if (project == null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "The project does not exist"
                    };
                }

                int numTask = await _context.Tasks
                    .CountAsync(t => t.Project.Id == id && t.ExecutionDate > request.EndDate);
                if (numTask > 0)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "There are tasks with dates of execution higher than the new date."
                    };
                }

                project.Name = request.Name;
                project.Description = request.Description;
                project.EndDate = request.EndDate;

                _context.Update(project);            
                await _context.SaveChangesAsync();

                return new Response
                {
                    IsSuccess = true,
                    Message = "Project updated successfully",
                    Result = project
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

        public async Task<Response> GetProjectById(int id)
        {
            try
            {
                ProjectEntity project = await _context.Projects.FindAsync(id);
                if (project == null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "The project does not exist"
                    };
                }

                return new Response
                {
                    IsSuccess = true,
                    Result = project
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

        public async Task<Response> DeleteProject(int id)
        {
            try
            {
                ProjectEntity project = await _context.Projects
                    .Include(p => p.Tasks)
                    .FirstOrDefaultAsync(p => p.Id == id);
                if (project == null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "The project does not exist"
                    };
                }

                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();

                return new Response
                {
                    IsSuccess = true,
                    Message = "Project removed successfully."
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

        public async Task<Response> ChangeStateProject(ChangeStateRequest request, int id)
        {
            try
            {
                if (!request.Completed)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "Illegal value, must be true"
                    };
                }

                ProjectEntity project = await _context.Projects
                    .Include(p=>p.Tasks)
                    .FirstOrDefaultAsync(p => p.Id == id);
                int numTask = project.Tasks.Count;
                if (numTask == 0)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "Project cannot be completed because it has no associated tasks."
                    };
                }

                int taskCompleted = await _context.Tasks
                    .CountAsync(t => t.State == State.Finalizado && t.Project.Id == id);
                if (taskCompleted != numTask)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "Project can't be completed because it has pending tasks."
                    };
                }

                project.State = State.Finalizado;
                _context.Update(project);
                await _context.SaveChangesAsync();

                return new Response
                {
                    IsSuccess = true,
                    Message = "Project Completed successfully"
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
