using HazelNet_Application.CQRS.Abstractions;
using HazelNet_Domain.Models;

namespace HazelNet_Application.CQRS.Features.ReviewLogs.Queries;

public record GetTotalCountOfReviewLogsQuery(int UserId) : IQuery<int>;