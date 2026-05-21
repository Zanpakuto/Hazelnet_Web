using HazelNet_Domain.Models;
using HazelNet_Domain.IRepository;
using HazelNet_Infrastracture.DBContext;
using Microsoft.EntityFrameworkCore;

namespace HazelNet_Infrastracture.DBServices.Repository;

//implementation of ireviewlogrepository
public class ReviewLogRepository : IReviewLogRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    public ReviewLogRepository(IDbContextFactory<ApplicationDbContext> context)
    {
        _contextFactory = context;
    }

    public async Task<ReviewLog?> GetReviewLogByIdAsync(int reviewLogId)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync();
        return await _context.ReviewLogs.FindAsync(reviewLogId);
    }

    public async Task<List<ReviewLog>> GetReviewLogsByReviewHistoryIdAsync(int reviewHistoryId)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync();
        return await _context.ReviewLogs
            .Where(rl => rl.ReviewHistoryId == reviewHistoryId)
            .ToListAsync();
    } 
    
    public async Task<int> GetTotalReviewsByUserIdAsync(int userId)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync();
        return await _context.ReviewLogs
            .Where(rl => rl.ReviewHistory.Card.Deck.UserId == userId)
            .CountAsync();
    }
    
    public async Task<IReadOnlyDictionary<int, List<ReviewLog>>> GetReviewLogsByHistoryIdsAsync(IEnumerable<int> historyIds)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync();
        var idList = historyIds.ToList();
        var logs = await _context.ReviewLogs
            .Where(r => idList.Contains(r.ReviewHistoryId))
            .ToListAsync();

        return logs
            .GroupBy(r => r.ReviewHistoryId)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public async Task UpdateAsync(ReviewLog reviewLog)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync();
        _context.ReviewLogs.Update(reviewLog);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int reviewLogId)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync();
        var reviewLog = await GetReviewLogByIdAsync(reviewLogId);
        if (reviewLog != null)
        {
            _context.ReviewLogs.Remove(reviewLog);
            await _context.SaveChangesAsync();
        }
    }

    public async Task CreateAsync(ReviewLog reviewLog)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync();
        await _context.ReviewLogs.AddAsync(reviewLog);
        await _context.SaveChangesAsync();
    }
}