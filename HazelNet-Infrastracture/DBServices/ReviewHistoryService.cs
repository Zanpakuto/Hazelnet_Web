using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HazelNet_Domain.Models;
using HazelNet_Infrastracture.DBContext;

namespace HazelNet_Infrastracture.DBServices;

public class ReviewHistoryService
{
    private readonly ApplicationDbContext _context;

    public ReviewHistoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    //retrieves all review histories
    public async Task<List<ReviewHistory>> GetAllReviewHistoriesAsync()
    {
        return await _context.ReviewHistory.ToListAsync();
    }

    //retrieves a review history by ID
    public async Task<ReviewHistory?> GetReviewHistoryByIdAsync(int reviewHistoryId)
    {
        return await _context.ReviewHistory
            .FirstOrDefaultAsync(r => r.Id == reviewHistoryId);
    }


    //update the review history whenever contents are modified
    public async Task UpdateReviewHistoryAsync(ReviewHistory reviewHistory)
    {
        _context.ReviewHistory.Update(reviewHistory);
        await _context.SaveChangesAsync();
    }


    //retrieves reviewlogs for a review history
    public async Task<List<ReviewLog>> GetReviewLogsByReviewHistoryIdAsync(int reviewHistoryId)
    {
        var reviewHistory = await _context.ReviewHistory
            .Include(rh => rh.ReviewLogs) // Include related review logs
            .FirstOrDefaultAsync(rh => rh.Id == reviewHistoryId);

        var logs = reviewHistory?.ReviewLogs ?? new List<ReviewLog>();
        return logs.ToList().OrderByDescending(log => log.Review).ToList(); // Sort by review date descending
    }

    //adds a review log to a review history
    public async Task AddReviewLogToReviewHistoryAsync(int reviewHistoryId, ReviewLog reviewLog)
    {
        var reviewHistory = await _context.ReviewHistory
            .Include(rh => rh.ReviewLogs) // Include related review logs
            .FirstOrDefaultAsync(rh => rh.Id == reviewHistoryId);

        if (reviewHistory != null)
        {
            reviewLog.ReviewHistoryId = reviewHistoryId; // Set the foreign key
            reviewLog.ReviewHistory = reviewHistory; // Set the navigation property
            reviewHistory.ReviewLogs.Add(reviewLog);
            await _context.SaveChangesAsync();
        }
    }

    //deletes a review log from a review history.
    public async Task DeleteReviewLogsByReviewHistoryIdAsync(int reviewHistoryId)
    {
        var reviewHistory = await _context.ReviewHistory
            .Include(rh => rh.ReviewLogs) // Include related review logs
            .FirstOrDefaultAsync(rh => rh.Id == reviewHistoryId);

        if (reviewHistory != null)
        {
            _context.ReviewLogs.RemoveRange(reviewHistory.ReviewLogs);
            await _context.SaveChangesAsync();
        }
    }

}