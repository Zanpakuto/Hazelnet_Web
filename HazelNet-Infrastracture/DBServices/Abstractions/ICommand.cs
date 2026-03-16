namespace HazelNet_Infrastracture.DBServices.Abstractions;

public interface ICommand : IBaseCommand
{
}

public interface ICommand<TResult> : IBaseCommand
{
}

public interface IBaseCommand
{
}