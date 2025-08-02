using FurryFriends.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class SanPhamChiTiet
{
	[Key]
	public Guid SanPhamChiTietId { get; set; }

	[Required]
	public Guid SanPhamId { get; set; }

	[Required]
	public Guid KichCoId { get; set; }

	[Required]
	public Guid MauSacId { get; set; }

	public Guid? AnhId { get; set; }

	[Required]
	public int SoLuong { get; set; }

	[Required]
	public decimal Gia { get; set; }

	[Required]
	public DateTime NgayTao { get; set; }

	public DateTime? NgaySua { get; set; }

	public string MoTa { get; set; }

	[Required]
	public int TrangThai { get; set; }

	[ForeignKey("SanPhamId")]
	public virtual SanPham SanPham { get; set; }

	[ForeignKey("KichCoId")]
	public virtual KichCo KichCo { get; set; }

	[ForeignKey("MauSacId")]
	public virtual MauSac MauSac { get; set; }

	[ForeignKey("AnhId")]
	public virtual Anh Anh { get; set; }

	public virtual ICollection<GioHangChiTiet> GioHangChiTiets { get; set; }

	public virtual ICollection<DotGiamGiaSanPham> DotGiamGiaSanPhams { get; set; }

}
