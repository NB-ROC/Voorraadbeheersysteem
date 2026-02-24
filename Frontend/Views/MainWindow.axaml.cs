using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Grpc.Net.Client;

namespace Frontend.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void TestRequest(object? sender, RoutedEventArgs e)
    {
        GrpcChannel channel = GrpcChannel.ForAddress("http://127.0.0.1:8080");
        Greeter.GreeterClient client = new(channel);
        HelloReply reply = client.SayHello(new HelloRequest { Name = "World" });

        Console.WriteLine(reply.Message);
    }
}