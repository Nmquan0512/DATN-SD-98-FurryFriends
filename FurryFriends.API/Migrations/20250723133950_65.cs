using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurryFriends.API.Migrations
{
    /// <inheritdoc />
    public partial class _65 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Anhs",
                columns: table => new
                {
                    AnhId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SanPhamChiTietId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DuongDan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenAnh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anhs", x => x.AnhId);
                });

            migrationBuilder.CreateTable(
                name: "ChatLieus",
                columns: table => new
                {
                    ChatLieuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenChatLieu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatLieus", x => x.ChatLieuId);
                });

            migrationBuilder.CreateTable(
                name: "ChucVus",
                columns: table => new
                {
                    ChucVuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenChucVu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MoTaChucVu = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChucVus", x => x.ChucVuId);
                });

            migrationBuilder.CreateTable(
                name: "GiamGias",
                columns: table => new
                {
                    GiamGiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenGiamGia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhanTramKhuyenMai = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiamGias", x => x.GiamGiaId);
                });

            migrationBuilder.CreateTable(
                name: "HinhThucThanhToans",
                columns: table => new
                {
                    HinhThucThanhToanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenHinhThuc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HinhThucThanhToans", x => x.HinhThucThanhToanId);
                });

            migrationBuilder.CreateTable(
                name: "KhachHangs",
                columns: table => new
                {
                    KhachHangId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenKhachHang = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DiemKhachHang = table.Column<int>(type: "int", nullable: true),
                    EmailCuaKhachHang = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgayTaoTaiKhoan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhatCuoiCung = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
                    TaiKhoanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHangs", x => x.KhachHangId);
                });

            migrationBuilder.CreateTable(
                name: "KichCos",
                columns: table => new
                {
                    KichCoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenKichCo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KichCos", x => x.KichCoId);
                });

            migrationBuilder.CreateTable(
                name: "MauSacs",
                columns: table => new
                {
                    MauSacId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenMau = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaMau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MauSacs", x => x.MauSacId);
                });

            migrationBuilder.CreateTable(
                name: "ThanhPhans",
                columns: table => new
                {
                    ThanhPhanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenThanhPhan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhPhans", x => x.ThanhPhanId);
                });

            migrationBuilder.CreateTable(
                name: "ThongBaos",
                columns: table => new
                {
                    ThongBaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Loai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DaDoc = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongBaos", x => x.ThongBaoId);
                });

            migrationBuilder.CreateTable(
                name: "ThuongHieus",
                columns: table => new
                {
                    ThuongHieuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenThuongHieu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThuongHieus", x => x.ThuongHieuId);
                });

            migrationBuilder.CreateTable(
                name: "Vouchers",
                columns: table => new
                {
                    VoucherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenVoucher = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PhanTramGiam = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vouchers", x => x.VoucherId);
                });

            migrationBuilder.CreateTable(
                name: "DiaChiKhachHangs",
                columns: table => new
                {
                    DiaChiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenDiaChi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ThanhPho = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PhuongXa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KhachHangId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiaChiKhachHangs", x => x.DiaChiId);
                    table.ForeignKey(
                        name: "FK_DiaChiKhachHangs_KhachHangs_KhachHangId",
                        column: x => x.KhachHangId,
                        principalTable: "KhachHangs",
                        principalColumn: "KhachHangId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GioHangs",
                columns: table => new
                {
                    GioHangId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KhachHangId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GioHangs", x => x.GioHangId);
                    table.ForeignKey(
                        name: "FK_GioHangs_KhachHangs_KhachHangId",
                        column: x => x.KhachHangId,
                        principalTable: "KhachHangs",
                        principalColumn: "KhachHangId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoans",
                columns: table => new
                {
                    TaiKhoanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTaoTaiKhoan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhatCuoiCung = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    KhachHangId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoans", x => x.TaiKhoanId);
                    table.ForeignKey(
                        name: "FK_TaiKhoans_KhachHangs_KhachHangId",
                        column: x => x.KhachHangId,
                        principalTable: "KhachHangs",
                        principalColumn: "KhachHangId");
                });

            migrationBuilder.CreateTable(
                name: "SanPhams",
                columns: table => new
                {
                    SanPhamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenSanPham = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThuongHieuId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanPhams", x => x.SanPhamId);
                    table.ForeignKey(
                        name: "FK_SanPhams_ThuongHieus_ThuongHieuId",
                        column: x => x.ThuongHieuId,
                        principalTable: "ThuongHieus",
                        principalColumn: "ThuongHieuId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NhanViens",
                columns: table => new
                {
                    NhanVienId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaiKhoanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HoVaTen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GioiTinh = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ChucVuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanViens", x => x.NhanVienId);
                    table.ForeignKey(
                        name: "FK_NhanViens_ChucVus_ChucVuId",
                        column: x => x.ChucVuId,
                        principalTable: "ChucVus",
                        principalColumn: "ChucVuId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NhanViens_TaiKhoans_TaiKhoanId",
                        column: x => x.TaiKhoanId,
                        principalTable: "TaiKhoans",
                        principalColumn: "TaiKhoanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DotGiamGiaSanPhams",
                columns: table => new
                {
                    DotGiamGiaSanPhamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GiamGiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SanPhamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhanTramGiamGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DotGiamGiaSanPhams", x => x.DotGiamGiaSanPhamId);
                    table.ForeignKey(
                        name: "FK_DotGiamGiaSanPhams_GiamGias_GiamGiaId",
                        column: x => x.GiamGiaId,
                        principalTable: "GiamGias",
                        principalColumn: "GiamGiaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DotGiamGiaSanPhams_SanPhams_SanPhamId",
                        column: x => x.SanPhamId,
                        principalTable: "SanPhams",
                        principalColumn: "SanPhamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SanPhamChatLieus",
                columns: table => new
                {
                    SanPhamChatLieuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChatLieuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SanPhamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanPhamChatLieus", x => x.SanPhamChatLieuId);
                    table.ForeignKey(
                        name: "FK_SanPhamChatLieus_ChatLieus_ChatLieuId",
                        column: x => x.ChatLieuId,
                        principalTable: "ChatLieus",
                        principalColumn: "ChatLieuId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SanPhamChatLieus_SanPhams_SanPhamId",
                        column: x => x.SanPhamId,
                        principalTable: "SanPhams",
                        principalColumn: "SanPhamId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SanPhamChiTiets",
                columns: table => new
                {
                    SanPhamChiTietId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SanPhamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KichCoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MauSacId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnhId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    Gia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgaySua = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanPhamChiTiets", x => x.SanPhamChiTietId);
                    table.ForeignKey(
                        name: "FK_SanPhamChiTiets_Anhs_AnhId",
                        column: x => x.AnhId,
                        principalTable: "Anhs",
                        principalColumn: "AnhId");
                    table.ForeignKey(
                        name: "FK_SanPhamChiTiets_KichCos_KichCoId",
                        column: x => x.KichCoId,
                        principalTable: "KichCos",
                        principalColumn: "KichCoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SanPhamChiTiets_MauSacs_MauSacId",
                        column: x => x.MauSacId,
                        principalTable: "MauSacs",
                        principalColumn: "MauSacId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SanPhamChiTiets_SanPhams_SanPhamId",
                        column: x => x.SanPhamId,
                        principalTable: "SanPhams",
                        principalColumn: "SanPhamId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SanPhamThanhPhans",
                columns: table => new
                {
                    SanPhamThanhPhanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ThanhPhanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SanPhamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanPhamThanhPhans", x => x.SanPhamThanhPhanId);
                    table.ForeignKey(
                        name: "FK_SanPhamThanhPhans_SanPhams_SanPhamId",
                        column: x => x.SanPhamId,
                        principalTable: "SanPhams",
                        principalColumn: "SanPhamId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SanPhamThanhPhans_ThanhPhans_ThanhPhanId",
                        column: x => x.ThanhPhanId,
                        principalTable: "ThanhPhans",
                        principalColumn: "ThanhPhanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoaDons",
                columns: table => new
                {
                    HoaDonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaiKhoanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoucherId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    KhachHangId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HinhThucThanhToanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenCuaKhachHang = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SdtCuaKhachHang = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailCuaKhachHang = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayNhanHang = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TongTienSauKhiGiam = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NhanVienId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoaiHoaDon = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDons", x => x.HoaDonId);
                    table.ForeignKey(
                        name: "FK_HoaDons_HinhThucThanhToans_HinhThucThanhToanId",
                        column: x => x.HinhThucThanhToanId,
                        principalTable: "HinhThucThanhToans",
                        principalColumn: "HinhThucThanhToanId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HoaDons_KhachHangs_KhachHangId",
                        column: x => x.KhachHangId,
                        principalTable: "KhachHangs",
                        principalColumn: "KhachHangId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoaDons_NhanViens_NhanVienId",
                        column: x => x.NhanVienId,
                        principalTable: "NhanViens",
                        principalColumn: "NhanVienId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoaDons_TaiKhoans_TaiKhoanId",
                        column: x => x.TaiKhoanId,
                        principalTable: "TaiKhoans",
                        principalColumn: "TaiKhoanId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HoaDons_Vouchers_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Vouchers",
                        principalColumn: "VoucherId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "GioHangChiTiets",
                columns: table => new
                {
                    GioHangChiTietId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GioHangId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SanPhamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SanPhamChiTietId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GioHangChiTiets", x => x.GioHangChiTietId);
                    table.ForeignKey(
                        name: "FK_GioHangChiTiets_GioHangs_GioHangId",
                        column: x => x.GioHangId,
                        principalTable: "GioHangs",
                        principalColumn: "GioHangId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GioHangChiTiets_SanPhamChiTiets_SanPhamChiTietId",
                        column: x => x.SanPhamChiTietId,
                        principalTable: "SanPhamChiTiets",
                        principalColumn: "SanPhamChiTietId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GioHangChiTiets_SanPhams_SanPhamId",
                        column: x => x.SanPhamId,
                        principalTable: "SanPhams",
                        principalColumn: "SanPhamId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HoaDonChiTiets",
                columns: table => new
                {
                    HoaDonChiTietId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SanPhamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HoaDonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SoLuongSanPham = table.Column<int>(type: "int", nullable: false),
                    Gia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDonChiTiets", x => x.HoaDonChiTietId);
                    table.ForeignKey(
                        name: "FK_HoaDonChiTiets_HoaDons_HoaDonId",
                        column: x => x.HoaDonId,
                        principalTable: "HoaDons",
                        principalColumn: "HoaDonId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoaDonChiTiets_SanPhams_SanPhamId",
                        column: x => x.SanPhamId,
                        principalTable: "SanPhams",
                        principalColumn: "SanPhamId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ChucVus",
                columns: new[] { "ChucVuId", "MoTaChucVu", "NgayCapNhat", "NgayTao", "TenChucVu", "TrangThai" },
                values: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), "Quản trị viên hệ thống", new DateTime(2025, 7, 23, 13, 39, 50, 156, DateTimeKind.Utc).AddTicks(4201), new DateTime(2025, 7, 23, 13, 39, 50, 156, DateTimeKind.Utc).AddTicks(4201), "admin", true });

            migrationBuilder.InsertData(
                table: "TaiKhoans",
                columns: new[] { "TaiKhoanId", "KhachHangId", "NgayCapNhatCuoiCung", "NgayTaoTaiKhoan", "Password", "TrangThai", "UserName" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), null, null, new DateTime(2025, 7, 23, 13, 39, 50, 156, DateTimeKind.Utc).AddTicks(4091), "123456", true, "admin" });

            migrationBuilder.InsertData(
                table: "NhanViens",
                columns: new[] { "NhanVienId", "ChucVuId", "DiaChi", "Email", "GioiTinh", "HoVaTen", "NgayCapNhat", "NgaySinh", "NgayTao", "SDT", "TaiKhoanId", "TrangThai" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("22222222-2222-2222-2222-222222222222"), "Hà Nội", "admin@furryfriends.local", "Nam", "Admin hệ thống", new DateTime(2025, 7, 23, 13, 39, 50, 156, DateTimeKind.Utc).AddTicks(4252), new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 7, 23, 13, 39, 50, 156, DateTimeKind.Utc).AddTicks(4251), "0123456789", new Guid("11111111-1111-1111-1111-111111111111"), true });

            migrationBuilder.CreateIndex(
                name: "IX_DiaChiKhachHangs_KhachHangId",
                table: "DiaChiKhachHangs",
                column: "KhachHangId");

            migrationBuilder.CreateIndex(
                name: "IX_DotGiamGiaSanPhams_GiamGiaId",
                table: "DotGiamGiaSanPhams",
                column: "GiamGiaId");

            migrationBuilder.CreateIndex(
                name: "IX_DotGiamGiaSanPhams_SanPhamId",
                table: "DotGiamGiaSanPhams",
                column: "SanPhamId");

            migrationBuilder.CreateIndex(
                name: "IX_GioHangChiTiets_GioHangId",
                table: "GioHangChiTiets",
                column: "GioHangId");

            migrationBuilder.CreateIndex(
                name: "IX_GioHangChiTiets_SanPhamChiTietId",
                table: "GioHangChiTiets",
                column: "SanPhamChiTietId");

            migrationBuilder.CreateIndex(
                name: "IX_GioHangChiTiets_SanPhamId",
                table: "GioHangChiTiets",
                column: "SanPhamId");

            migrationBuilder.CreateIndex(
                name: "IX_GioHangs_KhachHangId",
                table: "GioHangs",
                column: "KhachHangId");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDonChiTiets_HoaDonId",
                table: "HoaDonChiTiets",
                column: "HoaDonId");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDonChiTiets_SanPhamId",
                table: "HoaDonChiTiets",
                column: "SanPhamId");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_HinhThucThanhToanId",
                table: "HoaDons",
                column: "HinhThucThanhToanId");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_KhachHangId",
                table: "HoaDons",
                column: "KhachHangId");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_NhanVienId",
                table: "HoaDons",
                column: "NhanVienId");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_TaiKhoanId",
                table: "HoaDons",
                column: "TaiKhoanId");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_VoucherId",
                table: "HoaDons",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_NhanViens_ChucVuId",
                table: "NhanViens",
                column: "ChucVuId");

            migrationBuilder.CreateIndex(
                name: "IX_NhanViens_TaiKhoanId",
                table: "NhanViens",
                column: "TaiKhoanId",
                unique: true,
                filter: "[TaiKhoanId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamChatLieus_ChatLieuId",
                table: "SanPhamChatLieus",
                column: "ChatLieuId");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamChatLieus_SanPhamId_ChatLieuId",
                table: "SanPhamChatLieus",
                columns: new[] { "SanPhamId", "ChatLieuId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamChiTiets_AnhId",
                table: "SanPhamChiTiets",
                column: "AnhId");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamChiTiets_KichCoId",
                table: "SanPhamChiTiets",
                column: "KichCoId");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamChiTiets_MauSacId",
                table: "SanPhamChiTiets",
                column: "MauSacId");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamChiTiets_SanPhamId",
                table: "SanPhamChiTiets",
                column: "SanPhamId");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhams_ThuongHieuId",
                table: "SanPhams",
                column: "ThuongHieuId");

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamThanhPhans_SanPhamId_ThanhPhanId",
                table: "SanPhamThanhPhans",
                columns: new[] { "SanPhamId", "ThanhPhanId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamThanhPhans_ThanhPhanId",
                table: "SanPhamThanhPhans",
                column: "ThanhPhanId");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoans_KhachHangId",
                table: "TaiKhoans",
                column: "KhachHangId");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoans_UserName",
                table: "TaiKhoans",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiaChiKhachHangs");

            migrationBuilder.DropTable(
                name: "DotGiamGiaSanPhams");

            migrationBuilder.DropTable(
                name: "GioHangChiTiets");

            migrationBuilder.DropTable(
                name: "HoaDonChiTiets");

            migrationBuilder.DropTable(
                name: "SanPhamChatLieus");

            migrationBuilder.DropTable(
                name: "SanPhamThanhPhans");

            migrationBuilder.DropTable(
                name: "ThongBaos");

            migrationBuilder.DropTable(
                name: "GiamGias");

            migrationBuilder.DropTable(
                name: "GioHangs");

            migrationBuilder.DropTable(
                name: "SanPhamChiTiets");

            migrationBuilder.DropTable(
                name: "HoaDons");

            migrationBuilder.DropTable(
                name: "ChatLieus");

            migrationBuilder.DropTable(
                name: "ThanhPhans");

            migrationBuilder.DropTable(
                name: "Anhs");

            migrationBuilder.DropTable(
                name: "KichCos");

            migrationBuilder.DropTable(
                name: "MauSacs");

            migrationBuilder.DropTable(
                name: "SanPhams");

            migrationBuilder.DropTable(
                name: "HinhThucThanhToans");

            migrationBuilder.DropTable(
                name: "NhanViens");

            migrationBuilder.DropTable(
                name: "Vouchers");

            migrationBuilder.DropTable(
                name: "ThuongHieus");

            migrationBuilder.DropTable(
                name: "ChucVus");

            migrationBuilder.DropTable(
                name: "TaiKhoans");

            migrationBuilder.DropTable(
                name: "KhachHangs");
        }
    }
}
