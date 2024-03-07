namespace Mango.Web.Utitlity
{
    public class StaticDetails
    {
        public static string CouponApiBaseUrl { get; set; }
        public static string AuthApiBaseUrl { get; set; }
        public static string ProductApiBaseUrl { get; set; }
        public static string ShoppingCartBaseUrl { get; set; }
        public static string OrderApiBaseUrl { get; set; }

        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";

        public const string JwtTokenCookie = "JwtToken";

        public const string Status_Pending = "Pending";
        public const string Status_Approved = "Approved";
        public const string Status_ReadyForPickup = "ReadyForPickup";
        public const string Status_Completed = "Completed";
        public const string Status_Refunded = "Refunded";
        public const string Status_Cancelled = "Cancelled";

        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}
