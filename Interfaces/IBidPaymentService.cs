using Nafes.CrossCutting.Common.OperationResponse;
using Nafis.Services.DTO.Bid;
using Tanafos.Main.Services.DTO.Bid;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nafis.Services.Contracts
{
    /// <summary>
    /// Service interface for bid payment processing and financial operations
    /// </summary>
    public interface IBidPaymentService
    {
        /// <summary>
        /// Gets the price for bid documents for companies
        /// </summary>
        Task<OperationResult<ReadOnlyGetBidPriceModel>> GetBidPrice(GetBidDocumentsPriceRequestModel request);

        /// <summary>
        /// Gets the price for bid documents for freelancers
        /// </summary>
        Task<OperationResult<ReadOnlyGetBidPriceModel>> GetBidPriceForFreelancer(GetBidDocumentsPriceRequestModel request);

        /// <summary>
        /// Processes purchase of bid terms book
        /// </summary>
        Task<OperationResult<BuyTermsBookResponseModel>> BuyTermsBook(BuyTermsBookModel model);

        /// <summary>
        /// Gets tender docs pill model for provider bid
        /// </summary>
        Task<OperationResult<BuyTenderDocsPillModel>> GetBuyTenderDocsPillModel(long providerBidId);

        /// <summary>
        /// Gets provider data for refundable company bid
        /// </summary>
        Task<OperationResult<GetProviderDataOfRefundableCompanyBidModel>> GetProviderDataOfRefundableCompanyBid(long companyBidId);

        /// <summary>
        /// Gets current user's companies eligible to buy terms book with forbidden reasons if any
        /// </summary>
        Task<OperationResult<List<GetCompaniesToBuyTermsBookResponse>>> GetCurrentUserCompaniesToBuyTermsBookWithForbiddenReasonsIfFoundAsync(long bidId, long? currenctUserSpecificCompanyId = null);

        /// <summary>
        /// Gets current user's freelancers eligible to buy terms book with forbidden reasons if any
        /// </summary>
        Task<OperationResult<GetFreelancersToBuyTermsBookResponse>> GetCurrentUserFreelancersToBuyTermsBookWithForbiddenReasonsIfFoundAsync(long bidId, long? freelancerId);
    }
}
