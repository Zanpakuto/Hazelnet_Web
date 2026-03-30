namespace HazelNet_Application.CQRS.Abstractions;

public interface ICommand : IBaseCommand
{
}

public interface ICommand<TResult> : IBaseCommand
{
}

public interface IBaseCommand
{
}