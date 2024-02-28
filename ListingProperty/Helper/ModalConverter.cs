using ListingProperty.Models;
using ListingProperty.ViewModal;

namespace ListingProperty.Helper
{
    public static class ModalConverter
    {
        public static ContactApproval ContactApproverDTOToContactApproval(ContactApproverDTO contactApproverDTO)
        {
            return new ContactApproval
            {
                UserId = contactApproverDTO.UserId,
                PropertyId = contactApproverDTO.PropertyId,
                CreatedOn = contactApproverDTO.CreatedOn,
                ApprovalStatus = Enums.ApprovalStatus.PendingApproval
            };
        }
    }
}
