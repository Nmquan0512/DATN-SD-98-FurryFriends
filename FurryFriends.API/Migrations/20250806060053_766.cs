using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurryFriends.API.Migrations
{
    /// <inheritdoc />
    public partial class _766 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnhSanPhamLucMua",
                table: "HoaDonChiTiets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChatLieuLucMua",
                table: "HoaDonChiTiets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GiaLucMua",
                table: "HoaDonChiTiets",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "KichCoLucMua",
                table: "HoaDonChiTiets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MauSacLucMua",
                table: "HoaDonChiTiets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MoTaSanPhamLucMua",
                table: "HoaDonChiTiets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenSanPhamLucMua",
                table: "HoaDonChiTiets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ThanhPhanLucMua",
                table: "HoaDonChiTiets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThuongHieuLucMua",
                table: "HoaDonChiTiets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ChucVus",
                keyColumn: "ChucVuId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "NgayCapNhat", "NgayTao" },
                values: new object[] { new DateTime(2025, 8, 6, 6, 0, 53, 247, DateTimeKind.Utc).AddTicks(7572), new DateTime(2025, 8, 6, 6, 0, 53, 247, DateTimeKind.Utc).AddTicks(7572) });

            migrationBuilder.UpdateData(
                table: "NhanViens",
                keyColumn: "NhanVienId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "NgayCapNhat", "NgayTao" },
                values: new object[] { new DateTime(2025, 8, 6, 6, 0, 53, 247, DateTimeKind.Utc).AddTicks(7594), new DateTime(2025, 8, 6, 6, 0, 53, 247, DateTimeKind.Utc).AddTicks(7592) });

            migrationBuilder.UpdateData(
                table: "TaiKhoans",
                keyColumn: "TaiKhoanId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "NgayTaoTaiKhoan",
                value: new DateTime(2025, 8, 6, 6, 0, 53, 247, DateTimeKind.Utc).AddTicks(7550));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnhSanPhamLucMua",
                table: "HoaDonChiTiets");

            migrationBuilder.DropColumn(
                name: "ChatLieuLucMua",
                table: "HoaDonChiTiets");

            migrationBuilder.DropColumn(
                name: "GiaLucMua",
                table: "HoaDonChiTiets");

            migrationBuilder.DropColumn(
                name: "KichCoLucMua",
                table: "HoaDonChiTiets");

            migrationBuilder.DropColumn(
                name: "MauSacLucMua",
                table: "HoaDonChiTiets");

            migrationBuilder.DropColumn(
                name: "MoTaSanPhamLucMua",
                table: "HoaDonChiTiets");

            migrationBuilder.DropColumn(
                name: "TenSanPhamLucMua",
                table: "HoaDonChiTiets");

            migrationBuilder.DropColumn(
                name: "ThanhPhanLucMua",
                table: "HoaDonChiTiets");

            migrationBuilder.DropColumn(
                name: "ThuongHieuLucMua",
                table: "HoaDonChiTiets");

            migrationBuilder.UpdateData(
                table: "ChucVus",
                keyColumn: "ChucVuId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "NgayCapNhat", "NgayTao" },
                values: new object[] { new DateTime(2025, 8, 5, 6, 15, 31, 356, DateTimeKind.Utc).AddTicks(750), new DateTime(2025, 8, 5, 6, 15, 31, 356, DateTimeKind.Utc).AddTicks(750) });

            migrationBuilder.UpdateData(
                table: "NhanViens",
                keyColumn: "NhanVienId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "NgayCapNhat", "NgayTao" },
                values: new object[] { new DateTime(2025, 8, 5, 6, 15, 31, 356, DateTimeKind.Utc).AddTicks(771), new DateTime(2025, 8, 5, 6, 15, 31, 356, DateTimeKind.Utc).AddTicks(770) });

            migrationBuilder.UpdateData(
                table: "TaiKhoans",
                keyColumn: "TaiKhoanId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "NgayTaoTaiKhoan",
                value: new DateTime(2025, 8, 5, 6, 15, 31, 356, DateTimeKind.Utc).AddTicks(732));
        }
    }
}
