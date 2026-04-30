using Intap.FirstProject.Domain.Categorias;
using Intap.FirstProject.Domain.Common;

namespace Intap.FirstProject.Domain.Projects;

public sealed class Project : BaseEntity
{
    private readonly List<ProjectMember> _members = [];

    private Project() { }

    public string              Name        { get; private set; } = string.Empty;
    public string              Client      { get; private set; } = string.Empty;
    public ProjectStatus       Status      { get; private set; } = ProjectStatus.Active;
    public string?             Description { get; private set; }
    public List<string>        Scope       { get; private set; } = [];
    public DateOnly            StartDate   { get; private set; }
    public DateOnly?           EndDate     { get; private set; }
    public bool                IsDeleted   { get; private set; }
    public Guid?               CategoriaId { get; private set; }
    public Categoria?          Categoria   { get; private set; }

    public ICollection<ProjectMember> Members => _members.AsReadOnly();

    public static Project Create(
        string       name,
        string       client,
        string?      description,
        List<string> scope,
        DateOnly     startDate,
        DateOnly?    endDate)
    {
        return new Project
        {
            Name        = name,
            Client      = client,
            Description = description,
            Scope       = scope,
            StartDate   = startDate,
            EndDate     = endDate,
            Status      = ProjectStatus.Active,
        };
    }

    public void Update(
        string       name,
        string       client,
        string?      description,
        List<string> scope,
        DateOnly     startDate,
        DateOnly?    endDate)
    {
        Name        = name;
        Client      = client;
        Description = description;
        Scope       = scope;
        StartDate   = startDate;
        EndDate     = endDate;
        SetUpdatedAt();
    }

    public void Archive()
    {
        Status = ProjectStatus.Archived;
        SetUpdatedAt();
    }

    public void Delete()
    {
        IsDeleted = true;
        SetUpdatedAt();
    }

    public void AddMember(Guid userId, AccessLevel accessLevel)
    {
        ProjectMember member = ProjectMember.Create(Id, userId, accessLevel);
        _members.Add(member);
        SetUpdatedAt();
    }

    public void RemoveMember(Guid memberId)
    {
        ProjectMember? member = _members.FirstOrDefault(m => m.Id == memberId);
        if (member is not null)
        {
            _members.Remove(member);
            SetUpdatedAt();
        }
    }

    public void AssignCategoria(Guid? categoriaId)
    {
        CategoriaId = categoriaId;
        SetUpdatedAt();
    }
}
