using Microsoft.AspNetCore.Authorization;

namespace ListingProperty.Models
{
    public class Policies
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string Buyer = "Buyer";
        public const string Seller = "Seller";

        public static AuthorizationPolicy AdminPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(Admin).Build();
        }
        public static AuthorizationPolicy UserPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(User).Build();
        }
        public static AuthorizationPolicy BuyerPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(Buyer).Build();
        }
        public static AuthorizationPolicy SellerPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(Seller).Build();
        }
    }
}
