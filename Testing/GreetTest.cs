using Backend;
using Grpc.Net.Client;

namespace Testing;

public class GreetTest
{
    private Greeter.GreeterClient _client;
    
    [SetUp]
    public void Setup()
    {
        
        GrpcChannel channel = GrpcChannel.ForAddress("http://127.0.0.1:8080");
        _client = new Greeter.GreeterClient(channel);
    }

    [Test]
    public void GreeterTest()
    {
        HelloReply reply = _client.SayHello(new HelloRequest { Name = "World" });
        Assert.That(reply.Message, Is.EqualTo("Hello World"));
        
        reply = _client.SayHello(new HelloRequest { Name = "Anthony" });
        Assert.That(reply.Message, Is.EqualTo("Hello Anthony"));
        
        reply = _client.SayHello(new HelloRequest { Name = "!@#%^&*&^%$%^#@&*(*&#*$eifuhrgyfeg^#&%#RTGNJlthrkler]g" });
        Assert.That(reply.Message, Is.EqualTo("Hello !@#%^&*&^%$%^#@&*(*&#*$eifuhrgyfeg^#&%#RTGNJlthrkler]g"));
    }
}