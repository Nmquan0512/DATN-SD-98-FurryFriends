using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurryFriends.API.Migrations
{
    /// <inheritdoc />
    public partial class _654646 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThongTinVoucherLucMua",
                table: "HoaDons",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ChucVus",
                keyColumn: "ChucVuId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "NgayCapNhat", "NgayTao" },
                values: new object[] { new DateTime(2025, 8, 9, 13, 45, 45, 896, DateTimeKind.Utc).AddTicks(633), new DateTime(2025, 8, 9, 13, 45, 45, 896, DateTimeKind.Utc).AddTicks(633) });

            migrationBuilder.UpdateData(
                table: "NhanViens",
                keyColumn: "NhanVienId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "NgayCapNhat", "NgayTao" },
                values: new object[] { new DateTime(2025, 8, 9, 13, 45, 45, 896, DateTimeKind.Utc).AddTicks(652), new DateTime(2025, 8, 9, 13, 45, 45, 896, DateTimeKind.Utc).AddTicks(652) });

            migrationBuilder.UpdateData(
                table: "TaiKhoans",
                keyColumn: "TaiKhoanId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "NgayTaoTaiKhoan",
                value: new DateTime(2025, 8, 9, 13, 45, 45, 896, DateTimeKind.Utc).AddTicks(615));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThongTinVoucherLucMua",
                table: "HoaDons");

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
    }
}
