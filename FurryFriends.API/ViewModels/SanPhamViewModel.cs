using FurryFriends.API.Models;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.API.ViewModels
{
	public class SanPhamViewModel
	{
		public int STT { get; set; }
		public Guid SanPhamId { get; set; }
		public string TenSanPham { get; set; }
		public int SoLuong { get; set; }
		public decimal Gia { get; set; }
	}

}