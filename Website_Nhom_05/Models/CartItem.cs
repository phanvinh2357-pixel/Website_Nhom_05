using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website_Nhom_05.Models
{
    public class CartItem
    {
        QL_DongHoEntities db = new QL_DongHoEntities();

        public int iMaDH { get; set; }
        public string sTenDH { get; set; }
        public string sAnhBia { get; set; }
        public double dDonGia { get; set; }
        public int iSoLuong { get; set; }
        public double ThanhTien
        {
            get { return iSoLuong * dDonGia; }
        }

        // Hàm tạo cho giỏ hàng
        public CartItem(int MaDongHo)
        {
            DongHo dh = db.DongHoes.SingleOrDefault(n => n.MaDongHo == MaDongHo);
            if (dh != null)
            {
                iMaDH = MaDongHo;
                sTenDH = dh.TenDongHo;
                sAnhBia = dh.HinhAnh;
                dDonGia = double.Parse(dh.GiaBan.ToString());
                iSoLuong = 1;
            }
        }
    }
}