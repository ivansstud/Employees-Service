using EmployeesService.Api.Data.Repositories;
using System.Data;
using System.Data.Common;

namespace EmployeesService.Api.Data;

public class UnitOfWork : IUnitOfWork
{
	private readonly IDbConnection _connection;
	private readonly IDbTransaction _transaction;
	private readonly IEmployeesRepositoty _employees;
	private readonly ICompaniesRepository _companies;
	private bool _disposed;
	private bool _completed;

	public UnitOfWork(IDbConnection connection)
	{
		_connection = connection ?? throw new ArgumentNullException(nameof(connection));

		if (_connection.State != ConnectionState.Open)
		{
			_connection.Open();
		}

		_transaction = _connection.BeginTransaction();
		_employees = new EmployeesRepositoty(this);
		_companies = new CompaniesRepository(this);
	}

	public IDbConnection Connection => _connection;
	public IDbTransaction Transaction => _transaction;

	public IEmployeesRepositoty Employees => _employees;
	public ICompaniesRepository Companies => _companies;

	public async Task CommitAsync(CancellationToken cancellationToken = default)
	{
		ThrowIfDisposed();
		ThrowIfCompleted();

		try
		{
			if (_transaction is DbTransaction dbTransaction)
			{
				await dbTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);
			}
			else
			{
				_transaction?.Commit();
			}

			_completed = true;
		}
		catch
		{
			await RollbackAsync(cancellationToken).ConfigureAwait(false);
			throw;
		}
	}

	public async Task RollbackAsync(CancellationToken cancellationToken = default)
	{
		ThrowIfDisposed();

		if (_completed)
		{
			return;
		}

		try
		{
			if (_transaction is DbTransaction dbTransaction)
			{
				await dbTransaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
			}
			else
			{
				_transaction?.Rollback();
			}
		}
		finally
		{
			_completed = true;
		}
	}

	private void ThrowIfDisposed()
	{
		ObjectDisposedException.ThrowIf(_disposed, nameof(UnitOfWork));
	}

	private void ThrowIfCompleted()
	{
		if (_completed)
		{
			throw new InvalidOperationException("Транзакция уже была завершена");
		}
	}

	public void Dispose()
	{
		if (_disposed)
		{
			return;
		}

		if (!_completed)
		{
			try
			{
				_transaction?.Rollback();
			}
			catch
			{
			}
		}

		_transaction?.Dispose();
		_connection?.Dispose();

		_disposed = true;
		GC.SuppressFinalize(this);
	}
}