using EmployeesService.Api.Data.Repositories;
using System.Data;

namespace EmployeesService.Api.Data;

public interface IUnitOfWork : IDisposable
{
	IDbConnection Connection { get; }
	IDbTransaction Transaction { get; }
	IEmployeesRepositoty Employees {  get; }
	ICompaniesRepository Companies {  get; }
	Task CommitAsync(CancellationToken cancellationToken);
	Task RollbackAsync(CancellationToken cancellationToken);
}