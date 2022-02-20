using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using QRCoder;

namespace ZO1.Identity.WebApp.Commons
{
    public class QrCodeHelpers
    {
        public static byte[] GenerateQRCodeBytes(string provider, 
            string key, string userEmail)
        {
            var qrCodeGenerator = new QRCodeGenerator();
            var qrCodeData = qrCodeGenerator.CreateQrCode(
                $"otpauth://totp//{provider}:{userEmail}?secret={key}&issuer={provider}", 
                QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);

            return BitmapToByteArray(qrCodeImage);
        }

        private static byte[] BitmapToByteArray(Bitmap image)
        {
            using var stream = new MemoryStream();
            image.Save(stream, ImageFormat.Png);
            return stream.ToArray();
        }
    }
}