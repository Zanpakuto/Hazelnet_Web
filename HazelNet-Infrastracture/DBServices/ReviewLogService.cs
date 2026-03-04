using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HazelNet_Domain.Models;
using HazelNet_Infrastracture.DBContext;

namespace HazelNet_Infrastracture.DBServices;

public class ReviewLogService
{
    private readonly ApplicationDbContext _context;

    public ReviewLogService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ReviewLog>> GetAllReviewLogsAsync()
    {
        return await _context.ReviewLogs.ToListAsync();
    }

    public async Task<ReviewLog> GetReviewLogByIdAsync(int reviewLogId)
    {
        return await _context.ReviewLogs
            .FirstOrDefaultAsync(r => r.Id == reviewLogId);
    }

    public async Task AddReviewLogAsync(ReviewLog reviewLog)
    {
        _context.ReviewLogs.Add(reviewLog);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateReviewLogAsync(ReviewLog reviewLog)
    {
        _context.ReviewLogs.Update(reviewLog);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteReviewLogAsync(int reviewLogId)
    {
        var reviewLog = await _context.ReviewLogs.FindAsync(reviewLogId);
        if (reviewLog != null)
        {
            _context.ReviewLogs.Remove(reviewLog);
            await _context.SaveChangesAsync();
        }
    }
}