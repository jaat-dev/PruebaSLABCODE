using Microsoft.EntityFrameworkCore;
using Projects.Api.Data;
using Projects.Api.Entities;
using Projects.Api.Models.Requests;
using Projects.Api.Models.Responses;
using System;
using System.Threading.Tasks;

namespace Projects.Api.Services
{
    public class ProjectService : IProject
    {
        private readonly DataContext _context;

        public ProjectService(DataContext context)
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
                    EndDate = request.EndDate
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

                project.Name = request.Name;
                project.Description = request.Description;
                project.EndDate = request.EndDate;

                //Todo: Validar que no existan tareas con fechas de ejecución
                //posteriores a la nueva fecha de finalización

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
                ProjectEntity project = await _context.Projects.FindAsync(id);
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

        public async Task<Response> UpdateStateProject(ChangeStateProjectRequest request, int id)
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

                project.Completed = request.Completed;

                //Todo: La petición solo
                //debe ser exitosa si todas las tareas del proyecto ya están en estado
                //Realizada, estom dispara un correo a los administradores indicando la
                //finalización del proyecto.

                _context.Update(project);
                await _context.SaveChangesAsync();

                return new Response
                {
                    IsSuccess = true,
                    Message = request.Completed ? 
                        "Project Completed successfully" : "Project Open successfully",
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
    }
}
