# CLAUDE.md — Backend

**Runtime:** .NET 10 | **ORM:** EF Core (SQL Server) | **Auth:** JWT | **Testing:** xUnit + Moq + FluentAssertions

> Before writing any code, read [`CODE_QUALITY.md`](./CODE_QUALITY.md) — it contains mandatory rules derived from SonarQube findings in this project.

---

## Architecture: Clean Architecture + Custom CQRS (no MediatR)

### Layer Dependency Rule

```
Domain ← Application ← Infrastructure
                ↑
          Presentation (API)
```

| Layer | Project suffix | Allowed dependencies |
|---|---|---|
| Domain | `.Domain` | None |
| Application | `.Application` | Domain only |
| Infrastructure | `.Infrastructure` | Domain + Application |
| Presentation | `.API` | Application only |
| Tests | `.Tests` | All src projects |

### Solution Folder Structure

```
Back/
├── src/
│   ├── Domain/           # Entities, Value Objects, Domain Events, no external deps
│   ├── Application/      # CQRS handlers, DTOs, Contracts, Result pattern
│   ├── Infrastructure/   # EF Core, JWT, Audit, external services
│   └── Presentation/     # Controllers, Middleware, DI wiring
├── tests/
└── docs/
    ├── ARCHITECTURE.md
    └── DOMAIN.md
```

---

## CQRS — Custom Implementation

### Interfaces (never use MediatR)

```csharp
// Markers
public interface IQuery<TResponse> { }
public interface ICommand { }
public interface ICommand<TResponse> { }

// Handlers
public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken ct);
}

public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<Result> Handle(TCommand command, CancellationToken ct);
}

public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<Result<TResponse>> Handle(TCommand command, CancellationToken ct);
}
```

### Handler injection in Controllers

```csharp
// Inject typed handlers directly — no ISender/IMediator
public class UsersController(
    IQueryHandler<GetAllUsersQuery, PagedResult<UserDto>> getAllHandler,
    ICommandHandler<CreateUserCommand, Guid> createHandler) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllUsersQuery query, CancellationToken ct)
    {
        Result<PagedResult<UserDto>> result = await getAllHandler.Handle(query, ct);
        return HandleResult(result);
    }
}
```

---

## ApiController Base

All controllers inherit from `ApiController`:
- Base route: `api/[controller]`
- `[Authorize]` by default
- `HandleResult(result)` maps `ErrorTypeResult` → HTTP status:

| ErrorTypeResult | HTTP |
|---|---|
| `Validation` | 400 |
| `NotFound` | 404 |
| `Conflict` | 409 |
| `Unauthorized` | 401 |
| `Failure` / `Problem` | 400 / 500 |

---

## Result Pattern

```csharp
// Handlers always return Result — never throw
return Result.Success(userDto);
return Result.Failure<UserDto>(UserErrors.NotFound);

// Prohibited inside handlers:
throw new UserNotFoundException();   // ❌
throw new Exception("...");          // ❌
```

### Result types

```csharp
Result                    // void success/failure
Result<TValue>            // success with value
ErrorResult               // record: code, description, ErrorTypeResult
ErrorTypeResult           // enum: Failure, Validation, NotFound, Conflict, Unauthorized, Problem
ValidationErrorResult     // collection of field errors
```

---

## Application Layer Structure

```
src/Application/
├── Abstractions/
│   ├── Audit/Enums/          # AuditAction, AuditEntity, AuditEventType, AuditModule
│   ├── Behaviors/            # ValidationDecoratorBehavior (Scrutor decorator)
│   ├── Messaging/            # ICommand, ICommandHandler, IQuery, IQueryHandler
│   ├── Pagination/           # PagedQuery, PagedResult, PaginationFilter, SortOrder
│   └── Results/              # Result, ErrorResult, ErrorTypeResult, ValidationErrorResult
├── Common/
│   ├── Constants/            # PermissionsConstant (nested static classes per module)
│   ├── DTOs/                 # Shared DTOs (e.g. AuditInfoResponseDto)
│   ├── Errors/               # ApplicationErrors (factory), ErrorCodes (string constants)
│   └── Messages/             # ApplicationMessageKeys, ApplicationMessages (ResourceManager)
├── Contracts/
│   ├── IAuditService.cs
│   ├── ICurrentUser.cs       # Guid? UserId
│   ├── ITokenService.cs      # GenerateToken
│   ├── IUnitOfWork.cs        # CompleteAsync, BeginTransactionAsync, ...
│   └── ReadRepositories/     # IEventLogReadRepository, IUserReadRepository, ...
├── Mappings/                 # AutoMapper profiles per entity
├── UseCases/                 # One folder per aggregate: Commands/, Queries/
└── DependencyInjection.cs
```

### UseCase folder convention

```
UseCases/Users/
├── Commands/
│   ├── CreateUser/
│   │   ├── CreateUserCommand.cs
│   │   ├── CreateUserCommandHandler.cs
│   │   └── CreateUserCommandValidator.cs
│   └── DeleteUser/
│       ├── DeleteUserCommand.cs
│       └── DeleteUserCommandHandler.cs
└── Queries/
    └── GetAllUsers/
        ├── GetAllUsersQuery.cs
        └── GetAllUsersQueryHandler.cs
```

---

## Hard Rules

| Never | Always |
|---|---|
| Use MediatR | Use own `IQueryHandler` / `ICommandHandler` |
| Throw exceptions in handlers | Return `Result.Failure()` |
| Reference Infrastructure from Presentation | Use Application interfaces |
| Use domain entities in controllers | Return DTOs |
| Use write repository in query handlers | Use `IReadRepository` |
| Put business logic in controllers | Keep it in entities or handlers |
| `var` for non-obvious types | Use explicit type |
| Names in Spanish | All code in English |
