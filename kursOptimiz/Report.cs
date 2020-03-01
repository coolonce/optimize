using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace kursOptimiz
{
    class Report
    {
        public void Create(Bitmap bmp)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Report";
            dlg.DefaultExt = ".PDF";
            bool? result = dlg.ShowDialog();
            FileStream fs = new FileStream(dlg.FileName, FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            bmp = resizeImage(bmp, new Size(640, 360));
            iTextSharp.text.Image pdfImage = iTextSharp.text.Image.GetInstance(bmp, System.Drawing.Imaging.ImageFormat.Png);

            doc.Open();
            doc.Add(new Paragraph(@"Отчет по курсовой работе "));
            doc.Add(pdfImage);
            doc.AddCreator("Kiselev Vadim");
            doc.Close();
        }
        public Bitmap resizeImage(Bitmap imgToResize, Size size)
        {
            return new Bitmap(imgToResize, size);
        }
    }
}
