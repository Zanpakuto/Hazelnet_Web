namespace HazelNet_Application.DBServices.Abstractions;

public interface ICommand : IBaseCommand
{
}

public interface ICommand<TResult> : IBaseCommand
{
}

public interface IBaseCommand
{
}