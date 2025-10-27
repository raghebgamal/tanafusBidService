using Nafes.CrossCutting.Common.OperationResponse;
using Nafis.Services.Contracts;
using Tanafos.Main.Services.DTO.Bid;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nafis.Services.Implementation
{
    /// <summary>
    /// Service for bid statistics and views tracking
    /// </summary>
    public class BidStatisticsService : IBidStatisticsService
    {
        private readonly BidServiceCore _bidServiceCore;

        public BidStatisticsService(BidServiceCore bidServiceCore)
        {
            _bidServiceCore = bidServiceCore;
        }

        /// <inheritdoc/>
        public async Task<OperationResult<long>> IncreaseBidViewCount(long bidId)
        {
            return await _bidServiceCore.IncreaseBidViewCount(bidId);
        }

        /// <inheritdoc/>
        public async Task<PagedResponse<List<GetBidViewsModel>>> GetBidViews(long bidId, int pageSize, int pageNumber)
        {
            return await _bidServiceCore.GetBidViews(bidId, pageSize, pageNumber);
        }

        /// <inheritdoc/>
        public async Task<OperationResult<BidViewsStatisticsResponse>> GetBidViewsStatisticsAsync(long bidId)
        {
            return await _bidServiceCore.GetBidViewsStatisticsAsync(bidId);
        }
    }
}
