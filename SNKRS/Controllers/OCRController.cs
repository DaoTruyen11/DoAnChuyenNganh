using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Tesseract;

namespace PortfolioWeb.Controllers
{
    public class OCRController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        // Hàm tiền xử lý ảnh
        private string PreprocessImage(string filePath)
        {
            // Đọc ảnh gốc
            using (var originalImage = new Bitmap(filePath))
            {
                // Chuyển sang grayscale
                var grayscaleImage = new Bitmap(originalImage.Width, originalImage.Height);
                for (int x = 0; x < originalImage.Width; x++)
                {
                    for (int y = 0; y < originalImage.Height; y++)
                    {
                        var originalColor = originalImage.GetPixel(x, y);
                        var grayValue = (int)(originalColor.R * 0.3 + originalColor.G * 0.59 + originalColor.B * 0.11);
                        var grayColor = Color.FromArgb(grayValue, grayValue, grayValue);
                        grayscaleImage.SetPixel(x, y, grayColor);
                    }
                }

                // Tăng độ tương phản bằng cách điều chỉnh cường độ pixel
                var contrastImage = new Bitmap(grayscaleImage.Width, grayscaleImage.Height);
                const double contrastLevel = 1.2; // Tăng cường độ tương phản
                for (int x = 0; x < grayscaleImage.Width; x++)
                {
                    for (int y = 0; y < grayscaleImage.Height; y++)
                    {
                        var grayColor = grayscaleImage.GetPixel(x, y);
                        int newGrayValue = (int)((grayColor.R - 128) * contrastLevel + 128);
                        newGrayValue = Math.Max(0, Math.Min(255, newGrayValue));
                        var adjustedColor = Color.FromArgb(newGrayValue, newGrayValue, newGrayValue);
                        contrastImage.SetPixel(x, y, adjustedColor);
                    }
                }

                // Lưu ảnh đã xử lý
                string processedFilePath = Path.Combine(Path.GetDirectoryName(filePath), "processed_" + Path.GetFileName(filePath));
                contrastImage.Save(processedFilePath, System.Drawing.Imaging.ImageFormat.Png);


                return processedFilePath;
            }
        }

        [HttpPost]
        public ActionResult ExtractText(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    // Đường dẫn thư mục lưu file
                    var uploadDir = Server.MapPath("~/temps");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    // Lưu file gốc
                    var filePath = Path.Combine(uploadDir, file.FileName);
                    file.SaveAs(filePath);

                    // Tiền xử lý ảnh
                    string processedFilePath = PreprocessImage(filePath);

                    // Đường dẫn đến thư mục tessdata
                    var tessdataPath = Server.MapPath("~/tessdata");

                    // Ghi log đường dẫn tessdata
                    System.Diagnostics.Debug.WriteLine("Tessdata Path: " + tessdataPath);

                    // Khởi tạo Tesseract Engine và xử lý OCR
                    string extractedText;
                    using (var engine = new TesseractEngine(tessdataPath, "vie", EngineMode.Default))
                    {
                        using (var img = Pix.LoadFromFile(processedFilePath))
                        {
                            using (var page = engine.Process(img))
                            {
                                extractedText = page.GetText();
                            }
                        }
                    }

                    // Truyền thông tin vào ViewBag để hiển thị
                    ViewBag.ExtractedText = extractedText;
                    ViewBag.UploadedImagePath = $"/temps/{Path.GetFileName(filePath)}"; // Đường dẫn ảnh gốc
                    return View("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Lỗi khi xử lý OCR: " + ex.Message;
                    System.IO.File.WriteAllText(Server.MapPath("~/App_Data/Logs/TesseractError.txt"), ex.ToString());
                    return View("Index");
                }
            }

            ViewBag.Error = "Vui lòng chọn một hình ảnh.";
            return View("Index");
        }
    }
}
