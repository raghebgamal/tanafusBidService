using Nafes.CrossCutting.Common.OperationResponse;
using Nafis.Services.DTO.Bid;
using Tanafos.Main.Services.DTO.Bid;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nafis.Services.Contracts
{
    /// <summary>
    /// Service interface for bid search, filtering, and listing operations
    /// </summary>
    public interface IBidSearchService
    {
        /// <summary>
        /// Gets filtered and paginated list of bids
        /// </summary>
        Task<PagedResponse<IReadOnlyList<ReadOnlyFilterBidModel>>> GetBidsList(FilterBidsSearchModel request);

        /// <summary>
        /// Gets bids created by the current user
        /// </summary>
        Task<PagedResponse<IReadOnlyList<ReadOnlyFilterBidModel>>> GetBidsCreatedByUser(GetBidsCreatedByUserModel request);

        /// <summary>
        /// Gets bids for a specific association
        /// </summary>
        Task<PagedResponse<IReadOnlyList<ReadOnlyFilterBidModel>>> GetAssociationBids(GetBidsCreatedByUserModel request);

        /// <summary>
        /// Gets public bids list
        /// </summary>
        Task<PagedResponse<IReadOnlyList<ReadOnlyPublicBidListModel>>> GetPublicBidsList(int pageSize, int pageNumber);

        /// <summary>
        /// Gets public freelancing bids list
        /// </summary>
        Task<PagedResponse<IReadOnlyList<ReadOnlyPublicBidListModel>>> GetPublicFreelancingBidsList(FilterBidsSearchModel request);

        /// <summary>
        /// Gets bids for the current user (my bids)
        /// </summary>
        Task<PagedResponse<List<GetMyBidResponse>>> GetMyBidsAsync(FilterBidsSearchModel model);

        /// <summary>
        /// Gets provider bids
        /// </summary>
        Task<PagedResponse<IReadOnlyList<GetProvidersBidsReadOnly>>> GetpProviderBids(int pageSize = 10, int pageNumber = 1);

        /// <summary>
        /// Gets all bids (admin view)
        /// </summary>
        Task<PagedResponse<IReadOnlyList<GetMyBidResponse>>> GetAllBids(FilterBidsSearchModel request);

        /// <summary>
        /// Gets search headers for bids filtering
        /// </summary>
        Task<OperationResult<GetBidsSearchHeadersResponse>> GetBidsSearchHeadersAsync();

        /// <summary>
        /// Gets association provider bids
        /// </summary>
        Task<PagedResponse<IReadOnlyList<GetProvidersBidsReadOnly>>> GetAssociationProviderBids(int pageSize = 10, int pageNumber = 1);
    }
}
