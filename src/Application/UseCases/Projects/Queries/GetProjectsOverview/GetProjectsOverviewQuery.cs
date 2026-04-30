using Intap.FirstProject.Application.Abstractions.Messaging;
using Intap.FirstProject.Application.UseCases.Projects.DTOs;

namespace Intap.FirstProject.Application.UseCases.Projects.Queries.GetProjectsOverview;

public sealed record GetProjectsOverviewQuery : IQuery<List<ProjectSummaryDto>>;
