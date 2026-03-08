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


    //retrieves all review logs
    public async Task<List<ReviewLog>> GetAllReviewLogsAsync()
    {
        return await _context.ReviewLogs.ToListAsync();
    }

    //retrieves a review log by ID
    public async Task<ReviewLog?> GetReviewLogByIdAsync(int reviewLogId)
    {
        return await _context.ReviewLogs
            .FirstOrDefaultAsync(r => r.Id == reviewLogId);
    }


    public async Task UpdateReviewLogAsync(ReviewLog reviewLog)
    {
        _context.ReviewLogs.Update(reviewLog);
        await _context.SaveChangesAsync();
    }


}