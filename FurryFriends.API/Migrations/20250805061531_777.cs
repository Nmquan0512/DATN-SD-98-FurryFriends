using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurryFriends.API.Migrations
{
    /// <inheritdoc />
    public partial class _777 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "GiaTriGiamToiDa",
                table: "Vouchers",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiaChiCuaKhachHang",
                table: "HoaDons",
                type: "nvarchar(max)",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GiaTriGiamToiDa",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "DiaChiCuaKhachHang",
                table: "HoaDons");

            migrationBuilder.UpdateData(
                table: "ChucVus",
                keyColumn: "ChucVuId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "NgayCapNhat", "NgayTao" },
                values: new object[] { new DateTime(2025, 8, 3, 0, 58, 44, 894, DateTimeKind.Utc).AddTicks(2826), new DateTime(2025, 8, 3, 0, 58, 44, 894, DateTimeKind.Utc).AddTicks(2825) });

            migrationBuilder.UpdateData(
                table: "NhanViens",
                keyColumn: "NhanVienId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "NgayCapNhat", "NgayTao" },
                values: new object[] { new DateTime(2025, 8, 3, 0, 58, 44, 894, DateTimeKind.Utc).AddTicks(2847), new DateTime(2025, 8, 3, 0, 58, 44, 894, DateTimeKind.Utc).AddTicks(2847) });

            migrationBuilder.UpdateData(
                table: "TaiKhoans",
                keyColumn: "TaiKhoanId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "NgayTaoTaiKhoan",
                value: new DateTime(2025, 8, 3, 0, 58, 44, 894, DateTimeKind.Utc).AddTicks(2807));
        }
    }
}
