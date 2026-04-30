using System.Reflection;
using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.Abstractions.Pagination;
using Intap.FirstProject.Application.UseCases.Auth.Commands.Login;
using Intap.FirstProject.Application.UseCases.Auth.Commands.Logout;
using Intap.FirstProject.Application.UseCases.Auth.DTOs;
using Intap.FirstProject.Application.UseCases.Auth.Queries.GetCurrentUser;
using Intap.FirstProject.Application.UseCases.Roles.Commands.AssignRolePermissions;
using Intap.FirstProject.Application.UseCases.Roles.Commands.CreateRole;
using Intap.FirstProject.Application.UseCases.Roles.Commands.DeleteRole;
using Intap.FirstProject.Application.UseCases.Roles.Commands.UpdateRole;
using Intap.FirstProject.Application.UseCases.Roles.DTOs;
using Intap.FirstProject.Application.UseCases.Roles.Queries.GetAllRoles;
using Intap.FirstProject.Application.UseCases.Roles.Queries.GetRoleById;
using Intap.FirstProject.Application.UseCases.Projects.Commands.AddProjectMember;
using Intap.FirstProject.Application.UseCases.Projects.Commands.ArchiveProject;
using Intap.FirstProject.Application.UseCases.Projects.Commands.CreateProject;
using Intap.FirstProject.Application.UseCases.Projects.Commands.DeleteProject;
using Intap.FirstProject.Application.UseCases.Projects.Commands.RemoveProjectMember;
using Intap.FirstProject.Application.UseCases.Projects.Commands.UpdateProject;
using Intap.FirstProject.Application.UseCases.Projects.DTOs;
using Intap.FirstProject.Application.UseCases.Projects.Queries.GetProjectById;
using Intap.FirstProject.Application.UseCases.Projects.Queries.GetProjectMembers;
using Intap.FirstProject.Application.UseCases.Projects.Queries.GetProjectsOverview;
using Intap.FirstProject.Application.UseCases.Permissions.DTOs;
using Intap.FirstProject.Application.UseCases.Permissions.Queries.GetAllPermissions;
using Intap.FirstProject.Application.UseCases.Categorias.Commands.CreateCategoria;
using Intap.FirstProject.Application.UseCases.Categorias.Commands.DeactivateCategoria;
using Intap.FirstProject.Application.UseCases.Categorias.Commands.UpdateCategoria;
using Intap.FirstProject.Application.UseCases.Categorias.DTOs;
using Intap.FirstProject.Application.UseCases.Categorias.Queries.GetCategoriaById;
using Intap.FirstProject.Application.UseCases.Categorias.Queries.GetCategorias;
using Intap.FirstProject.Application.UseCases.SubCategorias.Commands.CreateSubCategoria;
using Intap.FirstProject.Application.UseCases.SubCategorias.Commands.DeactivateSubCategoria;
using Intap.FirstProject.Application.UseCases.SubCategorias.Commands.UpdateSubCategoria;
using Intap.FirstProject.Application.UseCases.SubCategorias.DTOs;
using Intap.FirstProject.Application.UseCases.SubCategorias.Queries.GetSubCategoriaById;
using Intap.FirstProject.Application.UseCases.SubCategorias.Queries.GetSubCategorias;
using Intap.FirstProject.Application.UseCases.TiposProyectos.Commands.ChangeTipoProyectoEstado;
using Intap.FirstProject.Application.UseCases.TiposProyectos.Commands.CreateTipoProyecto;
using Intap.FirstProject.Application.UseCases.TiposProyectos.Commands.UpdateTipoProyecto;
using Intap.FirstProject.Application.UseCases.TiposProyectos.DTOs;
using Intap.FirstProject.Application.UseCases.TiposProyectos.Queries.GetTipoProyectoById;
using Intap.FirstProject.Application.UseCases.TiposProyectos.Queries.GetTiposProyectos;
using Intap.FirstProject.Application.UseCases.TiposTareas.Commands.ChangeTipoTareaEstado;
using Intap.FirstProject.Application.UseCases.TiposTareas.Commands.CreateTipoTarea;
using Intap.FirstProject.Application.UseCases.TiposTareas.Commands.UpdateTipoTarea;
using Intap.FirstProject.Application.UseCases.TiposTareas.DTOs;
using Intap.FirstProject.Application.UseCases.TiposTareas.Queries.GetTipoTareaById;
using Intap.FirstProject.Application.UseCases.TiposTareas.Queries.GetTiposTareas;
using Intap.FirstProject.Application.UseCases.Tasks.Commands.CreateTask;
using Intap.FirstProject.Application.UseCases.Tasks.Commands.DeleteTask;
using Intap.FirstProject.Application.UseCases.Tasks.Commands.UpdateTask;
using Intap.FirstProject.Application.UseCases.Tasks.DTOs;
using Intap.FirstProject.Application.UseCases.Tasks.Queries.GetAllTasks;
using Intap.FirstProject.Application.UseCases.Tasks.Queries.GetTaskById;
using Intap.FirstProject.Application.UseCases.Users.Commands.CreateUser;
using Intap.FirstProject.Application.UseCases.Users.Commands.DeleteUser;
using Intap.FirstProject.Application.UseCases.Users.Commands.ToggleUserStatus;
using Intap.FirstProject.Application.UseCases.Users.Commands.UpdateUser;
using Intap.FirstProject.Application.UseCases.Users.DTOs;
using Intap.FirstProject.Application.UseCases.Users.Queries.GetAllUsers;
using Intap.FirstProject.Application.UseCases.Users.Queries.GetUserById;
using Microsoft.Extensions.DependencyInjection;

namespace Intap.FirstProject.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        Assembly assembly = typeof(DependencyInjection).Assembly;

        services.AddAutoMapper(cfg => cfg.AddMaps(assembly));

        // Auth handlers
        services.AddScoped<ICommandHandler<LoginCommand, LoginResponse>, LoginCommandHandler>();
        services.AddScoped<ICommandHandler<LogoutCommand>, LogoutCommandHandler>();
        services.AddScoped<IQueryHandler<GetCurrentUserQuery, LoginResponse>, GetCurrentUserQueryHandler>();

        // Users handlers
        services.AddScoped<IQueryHandler<GetAllUsersQuery, PagedResult<UserDto>>, GetAllUsersQueryHandler>();
        services.AddScoped<IQueryHandler<GetUserByIdQuery, UserDto>, GetUserByIdQueryHandler>();
        services.AddScoped<ICommandHandler<CreateUserCommand, Guid>, CreateUserCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateUserCommand>, UpdateUserCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteUserCommand>, DeleteUserCommandHandler>();
        services.AddScoped<ICommandHandler<ToggleUserStatusCommand>, ToggleUserStatusCommandHandler>();

        // Permissions handlers
        services.AddScoped<IQueryHandler<GetAllPermissionsQuery, List<PermissionDto>>, GetAllPermissionsQueryHandler>();

        // Roles handlers
        services.AddScoped<IQueryHandler<GetAllRolesQuery, List<RoleDto>>, GetAllRolesQueryHandler>();
        services.AddScoped<IQueryHandler<GetRoleByIdQuery, RoleDetailDto>, GetRoleByIdQueryHandler>();
        services.AddScoped<ICommandHandler<CreateRoleCommand, Guid>, CreateRoleCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateRoleCommand>, UpdateRoleCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteRoleCommand>, DeleteRoleCommandHandler>();
        services.AddScoped<ICommandHandler<AssignRolePermissionsCommand>, AssignRolePermissionsCommandHandler>();

        // Projects handlers
        services.AddScoped<IQueryHandler<GetProjectsOverviewQuery, List<ProjectSummaryDto>>, GetProjectsOverviewQueryHandler>();
        services.AddScoped<IQueryHandler<GetProjectByIdQuery, ProjectDto>, GetProjectByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetProjectMembersQuery, List<TeamMemberDto>>, GetProjectMembersQueryHandler>();
        services.AddScoped<ICommandHandler<CreateProjectCommand, Guid>, CreateProjectCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateProjectCommand>, UpdateProjectCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteProjectCommand>, DeleteProjectCommandHandler>();
        services.AddScoped<ICommandHandler<ArchiveProjectCommand>, ArchiveProjectCommandHandler>();
        services.AddScoped<ICommandHandler<AddProjectMemberCommand>, AddProjectMemberCommandHandler>();
        services.AddScoped<ICommandHandler<RemoveProjectMemberCommand>, RemoveProjectMemberCommandHandler>();

        // Tasks handlers
        services.AddScoped<IQueryHandler<GetAllTasksQuery, PagedResult<TaskDto>>, GetAllTasksQueryHandler>();
        services.AddScoped<IQueryHandler<GetTaskByIdQuery, TaskDto>, GetTaskByIdQueryHandler>();
        services.AddScoped<ICommandHandler<CreateTaskCommand, Guid>, CreateTaskCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateTaskCommand>, UpdateTaskCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteTaskCommand>, DeleteTaskCommandHandler>();

        // Categorias handlers
        services.AddScoped<IQueryHandler<GetCategoriasQuery, List<CategoriaDto>>, GetCategoriasQueryHandler>();
        services.AddScoped<IQueryHandler<GetCategoriaByIdQuery, CategoriaDto>, GetCategoriaByIdQueryHandler>();
        services.AddScoped<ICommandHandler<CreateCategoriaCommand, Guid>, CreateCategoriaCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateCategoriaCommand>, UpdateCategoriaCommandHandler>();
        services.AddScoped<ICommandHandler<DeactivateCategoriaCommand>, DeactivateCategoriaCommandHandler>();

        // SubCategorias handlers
        services.AddScoped<IQueryHandler<GetSubCategoriasQuery, List<SubCategoriaDto>>, GetSubCategoriasQueryHandler>();
        services.AddScoped<IQueryHandler<GetSubCategoriaByIdQuery, SubCategoriaDto>, GetSubCategoriaByIdQueryHandler>();
        services.AddScoped<ICommandHandler<CreateSubCategoriaCommand, Guid>, CreateSubCategoriaCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateSubCategoriaCommand>, UpdateSubCategoriaCommandHandler>();
        services.AddScoped<ICommandHandler<DeactivateSubCategoriaCommand>, DeactivateSubCategoriaCommandHandler>();

        // TiposProyectos handlers
        services.AddScoped<IQueryHandler<GetTiposProyectosQuery, PagedResult<TipoProyectoDto>>, GetTiposProyectosQueryHandler>();
        services.AddScoped<IQueryHandler<GetTipoProyectoByIdQuery, TipoProyectoDto>, GetTipoProyectoByIdQueryHandler>();
        services.AddScoped<ICommandHandler<CreateTipoProyectoCommand, Guid>, CreateTipoProyectoCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateTipoProyectoCommand, Guid>, UpdateTipoProyectoCommandHandler>();
        services.AddScoped<ICommandHandler<ChangeTipoProyectoEstadoCommand, TipoProyectoEstadoDto>, ChangeTipoProyectoEstadoCommandHandler>();

        // TiposTareas handlers
        services.AddScoped<IQueryHandler<GetTiposTareasQuery, PagedResult<TipoTareaDto>>, GetTiposTareasQueryHandler>();
        services.AddScoped<IQueryHandler<GetTipoTareaByIdQuery, TipoTareaDto>, GetTipoTareaByIdQueryHandler>();
        services.AddScoped<ICommandHandler<CreateTipoTareaCommand, Guid>, CreateTipoTareaCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateTipoTareaCommand, Guid>, UpdateTipoTareaCommandHandler>();
        services.AddScoped<ICommandHandler<ChangeTipoTareaEstadoCommand, TipoTareaEstadoDto>, ChangeTipoTareaEstadoCommandHandler>();

        return services;
    }
}
