using System.Diagnostics.CodeAnalysis;

namespace Intap.FirstProject.Application.Abstractions.Messaging;

// TResponse is not used in the interface body by design — it acts as a phantom type
// that flows into IQueryHandler<TQuery, TResponse>, allowing the handler to
// enforce a return type without the marker interface needing to declare it.
[SuppressMessage("Major Code Smell", "S2326:Unused type parameters should be removed",
    Justification = "TResponse is a phantom type parameter used by IQueryHandler<TQuery,TResponse> to enforce return type contracts.")]
public interface IQuery<TResponse> { }
