using HazelNet_Domain.Models;
using HazelNet_Domain.IRepository;
using HazelNet_Infrastracture.DBContext;
using Microsoft.EntityFrameworkCore;

namespace HazelNet_Infrastracture.DBServices.Repository;

//implementation of ireviewhistoryrepository
public class ReviewHistoryRepository : IReviewHistoryRepository
{
    private readonly ApplicationDbContext _context;

    public ReviewHistoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReviewHistory?> Get(int reviewHistoryId)
    {
        return await _context.ReviewHistory.FindAsync(reviewHistoryId);
    }

    public async Task<ReviewHistory?> GetReviewHistoryByCardId(int cardId)
    {
        return await _context.ReviewHistory
            .FirstOrDefaultAsync(rh => rh.CardId == cardId);
    }

    public async Task Update(ReviewHistory reviewHistory)
    {
        _context.ReviewHistory.Update(reviewHistory);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int reviewHistoryId)
    {
        var reviewHistory = await Get(reviewHistoryId);
        if (reviewHistory != null)
        {
            _context.ReviewHistory.Remove(reviewHistory);
            await _context.SaveChangesAsync();
        }
    }

    public async Task Create(ReviewHistory reviewHistory)
    {
        await _context.ReviewHistory.AddAsync(reviewHistory);
        await _context.SaveChangesAsync();
    }
}
