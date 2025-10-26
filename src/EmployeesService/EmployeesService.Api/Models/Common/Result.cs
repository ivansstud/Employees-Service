namespace EmployeesService.Api.Models.Common;

public readonly struct Result
{
	private Result(string error)
	{
		IsFailure = true;
		Error = error;
	}

	public string Error { get; private init; }
	public bool IsFailure { get; private init; }
	public bool IsSuccess => !IsFailure;

	public static Result Success()
	{
		return new Result();
	}

	public static Result Failure(string error)
	{
		return new Result(error);
	}

	public static Result<T> Success<T>(T value)
	{
		return Result<T>.Success(value);
	}

	public static Result<T> Failure<T>(string error)
	{
		return Result<T>.Failure(error);
	}
}

public readonly struct Result<T>
{
	private Result(T value)
	{
		Value = value;
		Error = string.Empty;
		IsFailure = false;
	}

	private Result(string error)
	{
		Error = error;
		IsFailure = true;
		Value = default!;
	}

	public T Value { get; private init; }
	public string Error { get; private init; }
	public bool IsFailure { get; private init; }
	public bool IsSuccess => !IsFailure;

	public static Result<T> Success(T value) => new(value);
	public static Result<T> Failure(string error) => new(error);

	public static implicit operator Result<T>(T value) => Success(value);
}

