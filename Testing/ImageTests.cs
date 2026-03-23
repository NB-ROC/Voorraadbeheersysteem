using Backend.Grpc.Helpers;
using Google.Protobuf;

namespace Testing;

public class ImageTests
{
    [SetUp]
    public void Setup()
    {
        Directory.CreateDirectory("Storage");
        Directory.CreateDirectory("Storage/Products");
    }

    [Test]
    public async Task WriteToFile()
    {
        byte[] image = await File.ReadAllBytesAsync(Path.Combine(Directory.GetCurrentDirectory(), "Assets/borger.jpg"));
        ByteString bytes = ByteString.CopyFrom(image);

        string filename = await StorageHelper.SaveFile(bytes, "jpg", "Public");
        byte[] newImage =
            await File.ReadAllBytesAsync(Path.Combine(Directory.GetCurrentDirectory(), "Storage/Public/" + filename));

        Console.WriteLine(Path.Combine(Directory.GetCurrentDirectory(), "Storage/Public/" + filename));
        Assert.AreEqual(image, newImage);
    }
}