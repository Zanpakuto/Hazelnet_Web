using HazelNet_Application.CQRS.Abstractions;

namespace HazelNet_Application.CQRS.Features.User.Command;

public record UpdateUserPasswordCommand(int UserId, string CurrentPassword, string NewPassword) : ICommand;