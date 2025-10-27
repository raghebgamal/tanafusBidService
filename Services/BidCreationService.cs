using Microsoft.AspNetCore.Http;
using Nafes.CrossCutting.Common.OperationResponse;
using Nafes.CrossCutting.Model.Entities;
using Nafis.Services.Contracts;
using Nafis.Services.DTO.Bid;
using Tanafos.Main.Services.DTO.Bid;
using Tanafos.Main.Services.DTO.BidAddresses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nafis.Services.Implementation
{
    /// <summary>
    /// Service for bid creation, update, and deletion operations
    /// </summary>
    public class BidCreationService : IBidCreationService
    {
        private readonly BidServiceCore _bidServiceCore;

        public BidCreationService(BidServiceCore bidServiceCore)
        {
            _bidServiceCore = bidServiceCore;
        }

        public async Task<OperationResult<AddBidResponse>> AddBidNew(AddBidModelNew model)
            => await _bidServiceCore.AddBidNew(model);

        public async Task<OperationResult<AddBidResponse>> AddInstantBid(AddInstantBid addInstantBidRequest)
            => await _bidServiceCore.AddInstantBid(addInstantBidRequest);

        public async Task<OperationResult<long>> AddBidAddressesTimes(AddBidAddressesTimesModel model)
            => await _bidServiceCore.AddBidAddressesTimes(model);

        public async Task<OperationResult<List<QuantitiesTable>>> AddBidQuantitiesTable(AddQuantitiesTableRequest model)
            => await _bidServiceCore.AddBidQuantitiesTable(model);

        public async Task<OperationResult<AddBidAttachmentsResponse>> AddBidAttachments(AddBidAttachmentRequest model)
            => await _bidServiceCore.AddBidAttachments(model);

        public async Task<OperationResult<AddInstantBidAttachmentResponse>> AddInstantBidAttachments(AddInstantBidsAttachments addInstantBidsAttachmentsRequest)
            => await _bidServiceCore.AddInstantBidAttachments(addInstantBidsAttachmentsRequest);

        public async Task<OperationResult<List<UploadFileResponse>>> UploadBidAttachments(IFormCollection formCollection)
            => await _bidServiceCore.UploadBidAttachments(formCollection);

        public async Task<OperationResult<List<UploadFileResponse>>> UploadBidAttachmentsNewsFile(IFormCollection formCollection)
            => await _bidServiceCore.UploadBidAttachmentsNewsFile(formCollection);

        public async Task<OperationResult<long>> AddBidClassificationAreaAndExecution(AddBidClassificationAreaAndExecutionModel model)
            => await _bidServiceCore.AddBidClassificationAreaAndExecution(model);

        public async Task<OperationResult<long>> AddBidNews(AddBidNewsModel model)
            => await _bidServiceCore.AddBidNews(model);

        public async Task<OperationResult<long>> TenderExtend(AddBidAddressesTimesTenderExtendModel model)
            => await _bidServiceCore.TenderExtend(model);

        public async Task<OperationResult<bool>> CopyBid(CopyBidRequest model)
            => await _bidServiceCore.CopyBid(model);

        public async Task<OperationResult<bool>> DeleteDraftBid(long bidId)
            => await _bidServiceCore.DeleteDraftBid(bidId);
    }
}
