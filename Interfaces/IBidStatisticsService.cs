using Nafes.CrossCutting.Common.OperationResponse;
using Tanafos.Main.Services.DTO.Bid;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nafis.Services.Contracts
{
    /// <summary>
    /// Service interface for bid statistics and views tracking
    /// </summary>
    public interface IBidStatisticsService
    {
        /// <summary>
        /// Increases the view count for a specific bid
        /// </summary>
        /// <param name="bidId">The ID of the bid to track</param>
        /// <returns>Operation result with the updated view count</returns>
        Task<OperationResult<long>> IncreaseBidViewCount(long bidId);

        /// <summary>
        /// Gets paginated list of bid views
        /// </summary>
        /// <param name="bidId">The ID of the bid</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="pageNumber">Page number to retrieve</param>
        /// <returns>Paged response containing bid views</returns>
        Task<PagedResponse<List<GetBidViewsModel>>> GetBidViews(long bidId, int pageSize, int pageNumber);

        /// <summary>
        /// Gets statistical information about bid views
        /// </summary>
        /// <param name="bidId">The ID of the bid</param>
        /// <returns>Operation result containing bid view statistics</returns>
        Task<OperationResult<BidViewsStatisticsResponse>> GetBidViewsStatisticsAsync(long bidId);
    }
}
