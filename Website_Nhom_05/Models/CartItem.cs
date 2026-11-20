using System;
using System.Linq;
using Website_Nhom_05.Models;

namespace Website_Nhom_05.Models
{
    public class CartItem
    {
        public int MaDongHo { get; set; }
        public string TenDongHo { get; set; }
        public string TenThuongHieu { get; set; }
        public string MaSanPham { get; set; }
        public string HinhAnh { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuong { get; set; }

        // Giải quyết lỗi CS1061 tại HomeController.cs
        public int iSoLuong
        {
            get { return SoLuong; }
            set { SoLuong = value; }
        }

        public string ThongSoNgan { get; set; }

        public decimal ThanhTien
        {
            get { return GiaBan * SoLuong; }
        }

        public CartItem(int id)
        {
            // Sửa tên lớp Context đúng theo QL_DongHo.sql (có dấu gạch dưới)
            using (var db = new QL_DongHoEntities())
            {
                var sp = db.DongHoes.SingleOrDefault(n => n.MaDongHo == id);
                if (sp != null)
                {
                    MaDongHo = sp.MaDongHo;
                    TenDongHo = sp.TenDongHo;
                    TenThuongHieu = sp.ThuongHieu != null ? sp.ThuongHieu.TenThuongHieu : "WATCH STORE";
                    MaSanPham = "DH-" + sp.MaDongHo.ToString("D4");
                    HinhAnh = sp.HinhAnh;
                    GiaBan = sp.GiaBan;
                    SoLuong = 1;
                    ThongSoNgan = !string.IsNullOrEmpty(sp.MoTa) && sp.MoTa.Length > 40
                                  ? sp.MoTa.Substring(0, 40) + "..."
                                  : "Chuẩn kháng nước · Thiết kế cao cấp";
                }
            }
        }
    }
}