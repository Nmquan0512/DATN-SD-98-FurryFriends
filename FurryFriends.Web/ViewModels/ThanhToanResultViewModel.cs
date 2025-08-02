using System.Text.Json.Serialization;

namespace FurryFriends.Web.ViewModels
{
    public class ThanhToanResultViewModel
    {
        [JsonPropertyName("hoaDonId")]
        public Guid HoaDonId { get; set; }
    }

}
