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
                ApprovalStatus = false,
                CreatedOn = contactApproverDTO.CreatedOn
            };
        }
    }
}
