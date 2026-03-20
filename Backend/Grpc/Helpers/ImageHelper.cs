using Backend.Entities;
using Google.Protobuf;

namespace Backend.Grpc.Helpers;

public static class ImageHelper
{
 
    /**
     * This returns the name of the newly created image, throws In
     */
    public static async Task<string> SaveImage(ByteString data, string extension, string path)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        const int nameLength = Product.ImageLength - 4; // dot and extension
    
        Random random = new();
        string fileName;
        string fullPath;

        do
        {
            char[] stringChars = new char[nameLength];
            for (int i = 0; i < nameLength; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            fileName = $"{new string(stringChars)}.{extension}";
            fullPath = Path.Combine("Storage/", path, fileName);
        
        } while (File.Exists(fullPath));

        await File.WriteAllBytesAsync(fullPath, data.ToByteArray());
    
        return fileName;
    }
}