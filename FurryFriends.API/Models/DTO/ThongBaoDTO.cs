namespace FurryFriends.API.Models.DTO
{
    public class ThongBaoDTO
    {
        public Guid ThongBaoId { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public string Loai { get; set; }
        public string UserName { get; set; } = "admin";
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public bool DaDoc { get; set; }
    }
}
