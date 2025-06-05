//using System.ComponentModel.DataAnnotations;

//namespace FurryFriends.API.Models
//{
//	public class ChatLieu
//	{
//		[Key]
//		public Guid ChatLieuId { get; set; }

//		[Required]
//		[StringLength(50)]
//		[RegularExpression(@"^[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠƯàáâãèéêìíòóôõùúăđĩũơưẠ-ỹ\s0-9]+$", ErrorMessage = "Tên chỉ được chứa chữ cái tiếng Việt, số và khoảng trắng")]
//		public string TenChatLieu { get; set; }

//		public string MoTa { get; set; }

//		public bool TrangThai { get; set; }
//	}
//}