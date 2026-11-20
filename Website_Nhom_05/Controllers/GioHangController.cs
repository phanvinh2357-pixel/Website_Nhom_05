using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Website_Nhom_05.Models;

namespace Website_Nhom_05.Controllers
{
    public class GioHangController : Controller
    {
        // 1. Sửa DbContext thành QL_DongHoEntities
        private QL_DongHoEntities db = new QL_DongHoEntities();

        public List<CartItem> LayGioHang()
        {
            List<CartItem> lstGioHang = Session["GioHang"] as List<CartItem>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<CartItem>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }

        public ActionResult ThemMatHang(int msp, string strURL)
        {
            List<CartItem> lstGioHang = LayGioHang();
            CartItem sanpham = lstGioHang.Find(n => n.MaDongHo == msp);
            if (sanpham == null)
            {
                sanpham = new CartItem(msp);
                lstGioHang.Add(sanpham);
            }
            else
            {
                sanpham.SoLuong++;
            }

            if (string.IsNullOrEmpty(strURL))
                return RedirectToAction("GioHang");
            return Redirect(strURL);
        }

        public ActionResult CapNhatSoLuong(int id, int step)
        {
            List<CartItem> lstGioHang = LayGioHang();
            CartItem sp = lstGioHang.Find(n => n.MaDongHo == id);
            if (sp != null)
            {
                sp.SoLuong += step;
                if (sp.SoLuong <= 0)
                {
                    lstGioHang.Remove(sp);
                }
            }
            return RedirectToAction("GioHang");
        }

        public ActionResult XoaMatHang(int id)
        {
            List<CartItem> lstGioHang = LayGioHang();
            CartItem sp = lstGioHang.Find(n => n.MaDongHo == id);
            if (sp != null)
            {
                lstGioHang.Remove(sp);
            }
            return RedirectToAction("GioHang");
        }

        public ActionResult XoaTatCa()
        {
            List<CartItem> lstGioHang = LayGioHang();
            lstGioHang.Clear();
            Session["VoucherCode"] = null;
            Session["GiamGiaVoucher"] = 0m;
            Session["DungDiem"] = false;
            return RedirectToAction("GioHang");
        }

        [HttpPost]
        public ActionResult ApDungVoucher(string maVoucher)
        {
            if (maVoucher != null && maVoucher.Trim().ToUpper() == "WATCH500")
            {
                Session["VoucherCode"] = "WATCH500";
                Session["GiamGiaVoucher"] = 500000m;
            }
            else
            {
                Session["VoucherCode"] = null;
                Session["GiamGiaVoucher"] = 0m;
            }
            return RedirectToAction("GioHang");
        }

        [HttpPost]
        public ActionResult SuDungDiem(bool? dungDiem)
        {
            Session["DungDiem"] = (dungDiem == true);
            return RedirectToAction("GioHang");
        }

        public ActionResult GioHang()
        {
            List<CartItem> lstGioHang = LayGioHang();
            return View(lstGioHang);
        }

        [HttpGet]
        public ActionResult DatHang()
        {
            List<CartItem> lstGioHang = LayGioHang();
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var kh = Session["KhachHang"] as KhachHang;
            ViewBag.KhachHang = kh;
            return View(lstGioHang);
        }

        [HttpPost]
        public ActionResult DatHang(FormCollection f)
        {
            List<CartItem> lstGioHang = LayGioHang();
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var kh = Session["KhachHang"] as KhachHang;
            string hoTen = f["HoTen"];
            string sdt = f["SoDienThoai"];
            string diaChiChiTiet = f["DiaChiChiTiet"];
            string phuongXa = f["PhuongXa"];
            string quanHuyen = f["QuanHuyen"];
            string tinhThanh = f["TinhThanh"];
            string phuongThucTT = f["PhuongThucThanhToan"] ?? "COD";

            string diaChiGiao = string.Format("{0}, {1}, {2}, {3}", diaChiChiTiet, phuongXa, quanHuyen, tinhThanh);

            // Cập nhật thông tin khách hàng vào CSDL
            if (kh != null)
            {
                var khDB = db.KhachHangs.Find(kh.MaKH);
                if (khDB != null)
                {
                    khDB.HoTen = hoTen;
                    khDB.SoDienThoai = sdt;
                    khDB.DiaChi = diaChiGiao;

                    bool dungDiem = Session["DungDiem"] != null && (bool)Session["DungDiem"];
                    if (dungDiem && (khDB.DiemTichLuy ?? 0) >= 150)
                    {
                        khDB.DiemTichLuy -= 150;
                    }
                }
            }

            // Tính tổng tiền đơn hàng
            decimal tamTinh = lstGioHang.Sum(n => n.ThanhTien);
            decimal voucher = Session["GiamGiaVoucher"] != null ? (decimal)Session["GiamGiaVoucher"] : 0m;
            bool coDungDiem = Session["DungDiem"] != null && (bool)Session["DungDiem"];
            decimal tienDiem = coDungDiem ? 150000m : 0m;
            decimal tongTien = Math.Max(0, tamTinh - voucher - tienDiem);

            // 2. Khởi tạo DonHang với các cột khớp 100% với bảng DonHang trong QL_DongHo.sql
            DonHang dh = new DonHang();
            // MaKH trong bảng DonHang là NOT NULL, nếu chưa đăng nhập gán mặc định tài khoản mẫu (MaKH = 1)
            dh.MaKH = kh != null ? kh.MaKH : 1;
            dh.NgayDat = DateTime.Now;
            dh.NgayGiao = DateTime.Now.AddDays(3);
            // SỬA DÒNG NÀY (Bỏ chữ N phía trước chuỗi):
            dh.TinhTrangGiaoHang = "Chờ xử lý"; dh.PhuongThucThanhToan = phuongThucTT;
            dh.DaThanhToan = (phuongThucTT == "VNPAY" || phuongThucTT == "MOMO");

            db.DonHangs.Add(dh);
            db.SaveChanges(); // Lưu để phát sinh MaDonHang

            // 3. Khởi tạo ChiTietDonHang với các cột khớp 100% với bảng ChiTietDonHang
            foreach (var item in lstGioHang)
            {
                ChiTietDonHang ct = new ChiTietDonHang();
                ct.MaDonHang = dh.MaDonHang;
                ct.MaDongHo = item.MaDongHo;
                ct.SoLuong = item.SoLuong;
                ct.DonGia = item.GiaBan; // Không ép kiểu double, giữ nguyên decimal

                db.ChiTietDonHangs.Add(ct);

                // Trừ SoLuongTon trong bảng DongHo
                var sp = db.DongHoes.Find(item.MaDongHo);
                if (sp != null && sp.SoLuongTon >= item.SoLuong)
                {
                    sp.SoLuongTon -= item.SoLuong;
                }
            }
            db.SaveChanges();

            // Lưu thông tin phiên đặt hàng
            Session["DonHangVuaDat"] = dh.MaDonHang;
            Session["TongTienVuaDat"] = tongTien;

            // Xóa sạch giỏ hàng
            lstGioHang.Clear();
            Session["VoucherCode"] = null;
            Session["GiamGiaVoucher"] = 0m;
            Session["DungDiem"] = false;

            return RedirectToAction("XacNhanDonHang");
        }

        public ActionResult XacNhanDonHang()
        {
            ViewBag.MaDonHang = Session["DonHangVuaDat"];
            ViewBag.TongTien = Session["TongTienVuaDat"];
            return View();
        }
    }
}