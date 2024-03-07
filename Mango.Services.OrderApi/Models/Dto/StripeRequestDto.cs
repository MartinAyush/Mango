namespace Mango.Services.OrderApi.Models.Dto
{
    public class StripeRequestDto
    {
        public string? StripSessionId { get; set; }
        public string? StripSessionUrl { get; set; }
        public string ApprovedUrl { get; set; }
        public string CancelUrl { get; set; }
        public OrderHeaderDto OrderHeader { get; set; }
    }
}
