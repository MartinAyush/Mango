namespace Mango.Web.Utitlity
{
    public class StaticDetails
    {
        public static string CouponApiBaseUrl { get; set; }
        public static string AuthApiBaseUrl { get; set; }

        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";

        public const string JwtTokenCookie = "JwtToken";

        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}
