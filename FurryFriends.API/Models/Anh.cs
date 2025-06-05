namespace FurryFriends.API.Models
{
    public class Anh
    {
        public Guid AnhId { get; set; }
        public string DuongDan { get; set; }
        public string TenAnh { get; set; }
        public bool TrangThai { get; set; }
        public Anh()
        {
            AnhId = Guid.NewGuid();
            TrangThai = true;
        }
    }
}
