namespace Mango.Web.Utitlity
{
    public class StaticDetails
    {
        public static string CouponApiBaseUrl { get; set; }
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}
