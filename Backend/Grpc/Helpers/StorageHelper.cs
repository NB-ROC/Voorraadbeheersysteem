using Backend.Entities;
using Google.Protobuf;

namespace Backend.Grpc.Helpers;

public static class StorageHelper
{
    public static async Task<ByteString> GetFile(string path)
    {
        string fullPath = Path.Combine("Storage/", path);

        return ByteString.CopyFrom(await File.ReadAllBytesAsync(fullPath));
    }

    /**
     * This returns the name of the newly created image
     */
    public static async Task<string> SaveFile(ByteString data, string extension, string path)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        const int nameLength = Product.ImageLength - 4; // dot and extension
        
        Random random = new();
        string fileName;
        string fullPath;

        do
        {
            char[] stringChars = new char[nameLength];
            for (int i = 0; i < nameLength; i++) stringChars[i] = chars[random.Next(chars.Length)];

            fileName = $"{new string(stringChars)}.{extension}";
            fullPath = Path.Combine("Storage/", path, fileName);
        } while (File.Exists(fullPath));

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        await File.WriteAllBytesAsync(fullPath, data.ToByteArray());

        return fileName;
    }

    public static async Task<string> ModifyFile(string oldFile, ByteString data, string extension, string path)
    {
        DeleteFile(oldFile);
        return await SaveFile(data, extension, path);
    }

    public static void DeleteFile(string path)
    {
        string fullPath = Path.Combine("Storage/", path);

        if (File.Exists(fullPath)) File.Delete(fullPath);
    }
}