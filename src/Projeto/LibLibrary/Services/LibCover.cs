using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EFLibrary;
using EFLibrary.Models;

namespace LibLibrary.Services
{
    public class LibCover
    {

        public static void AddCover(byte[] image)
        {
            try
            {
                using(LibraryContext context = new LibraryContext())
                {
                    Cover cover = new Cover { CoverImage = image };

                    context.Add(cover);
                    context.SaveChanges();
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public static async Task<byte[]> ConvertFileToImage(string filePath)
        {
            byte[] image = null;

            try
            {
                Uri uriResult;
                bool isUrl = Uri.TryCreate(filePath, UriKind.Absolute, out uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                if(isUrl && uriResult is not null)
                {
                    using(HttpClient webClient = new HttpClient())
                    {
                        image = await webClient.GetByteArrayAsync(filePath);

                        if (image == null) throw new Exception("Unable to convert image.");
                    }
                }

                if(!isUrl)
                {
                    image = await File.ReadAllBytesAsync(filePath);
                }

            }
            catch(Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }

            return image;
        }
    }
}
