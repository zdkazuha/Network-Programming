using Db_Controller.Entities;
using Db_Controller;
using System.Net.Sockets;
using System.Net;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Components;

internal class Program
{
    public static List<TcpClient> tcpClients = new List<TcpClient>();
    public static Dictionary<TcpClient, User> clientToUser = new Dictionary<TcpClient, User>();
    public static Db_functional context = new Db_functional();

    private static async Task Start(TcpClient client)
    {
        Console.WriteLine("Connected!");

        using NetworkStream ns = client.GetStream();
        using StreamWriter sw = new StreamWriter(ns) { AutoFlush = true };
        using StreamReader sr = new StreamReader(ns);

        if (tcpClients.Contains(client))
        {
            tcpClients.Remove(client);
        }
        tcpClients.Add(client);


        try
        {
            while (true)
            {
                string? fullMessage = await sr.ReadLineAsync();
                if (fullMessage == null)
                    break;

                if (fullMessage.Contains(":FILE:"))
                {
                    await ReceiveAndBroadcastFile(client, fullMessage);
                    continue;
                }
                if(fullMessage.Contains(":Invite:"))
                {
                    var parts_ = fullMessage.Split(':');
                    if (parts_.Length < 4) continue;
                    string inviterUserName = parts_[0];
                    string userNameToInvite = parts_[1];
                    string groupName = parts_[2];
                    int newGroupId = int.Parse(parts_[3]);
                    await InviteToGroup(client, userNameToInvite, inviterUserName, newGroupId, groupName);
                    continue;
                }

                var parts = fullMessage.Split(':', 2);
                if (parts.Length != 2)
                    continue;

                string userName = parts[0];
                string message = parts[1];

                context.SaveChanges();

                User user = context.GetUser(userName);
                if (user == null)
                {
                    await sw.WriteLineAsync("Сервер: Користувач не знайдений в базі.");
                    continue;
                }
                
                clientToUser[client] = user;

                Console.WriteLine($"Група користувача {userName}: {user.Group.Name}");
                Console.WriteLine($"{DateTime.Now:T} {userName} :: {message} from -- {client.Client.RemoteEndPoint}");

                if (message == "$<close>")
                {
                    tcpClients.Remove(client);
                    clientToUser.Remove(client);
                    client.Close();
                    break;
                }

                foreach (var tcpClient in tcpClients.ToList())
                {
                    try
                    {
                        if (!tcpClient.Connected)
                        {
                            tcpClients.Remove(tcpClient);
                            continue;
                        }

                        if (clientToUser.TryGetValue(tcpClient, out User joinedUser) &&
                            joinedUser.GroupId == user.GroupId)
                        {
                            NetworkStream clientStream = tcpClient.GetStream();
                            using StreamWriter sw_ = new StreamWriter(clientStream, leaveOpen: true) { AutoFlush = true };
                            await sw_.WriteLineAsync($"{userName}:{message}");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Помилка при надсиланні до клієнта: {e.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка з клієнтом: {ex.Message}");
        }
        finally
        {
            tcpClients.Remove(client);
            clientToUser.Remove(client);
            client.Close();
        }
    }
    public static async Task ReceiveAndBroadcastFile(TcpClient senderClient, string header)
    {
        try
        {
            var parts = header.Split(':');
            if (parts.Length < 4) return;

            string senderName = parts[0];
            string fileName = parts[2];
            int length = int.Parse(parts[3]);

            var user = context.GetUser(senderName);
            if (user == null) return;

            NetworkStream senderStream = senderClient.GetStream();
            byte[] buffer = new byte[length];
            int bytesRead = 0;

            while (bytesRead < length)
            {
                int read = await senderStream.ReadAsync(buffer, bytesRead, length - bytesRead);
                if (read == 0) break;
                bytesRead += read;
            }

            foreach (var tcpClient in tcpClients.ToList())
            {
                try
                {
                    if (!tcpClient.Connected)
                    {
                        tcpClients.Remove(tcpClient);
                        continue;
                    }

                    if (clientToUser.TryGetValue(tcpClient, out User joinedUser) &&
                        joinedUser.GroupId == user.GroupId)
                    {
                        NetworkStream clientStream = tcpClient.GetStream();
                        using StreamWriter writer = new StreamWriter(clientStream, leaveOpen: true) { AutoFlush = true };
                        await writer.WriteLineAsync($"{senderName}:FILE:{fileName}:{length}");
                        await clientStream.WriteAsync(buffer, 0, length);
                        await clientStream.FlushAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Send file error: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ReceiveAndBroadcastFile error: {ex.Message}");
        }
    }
    public static async Task InviteToGroup(TcpClient inviterClient, string userNameToInvite, string inviterUserName, int newGroupId, string groupName)
    {
        var inviterUser = clientToUser[inviterClient];
        if (inviterUser == null)
        {
            Console.WriteLine("Помилка: Користувач не знайдений.");
            return;
        }

        var invitedUser = context.GetUser(userNameToInvite);
        if (invitedUser == null)
        {
            Console.WriteLine($"Користувача {userNameToInvite} не знайдено.");
            return;
        }

        string invitationMessage = $"{inviterUserName}:Invite:{groupName}:{newGroupId}";

        foreach (var tcpClient in tcpClients)
        {
            if (clientToUser.TryGetValue(tcpClient, out var clientUser) && clientUser.Username == userNameToInvite)
            {
                var invitedStream = tcpClient.GetStream();
                using var invitedWriter = new StreamWriter(invitedStream, leaveOpen: true) { AutoFlush = true };
                await invitedWriter.WriteLineAsync(invitationMessage);

                Console.WriteLine($"Запрошення надіслано користувачу {userNameToInvite} в групу {groupName} (ID: {newGroupId})");
                break;
            }
        }

        var inviterStream = inviterClient.GetStream();
        using var inviterWriter = new StreamWriter(inviterStream, leaveOpen: true) { AutoFlush = true };
        await inviterWriter.WriteLineAsync($"Ви запросили {userNameToInvite} в групу {groupName} (ID: {newGroupId})");
    }

    private static void Main(string[] args)
    {
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4040);
        TcpListener server = new TcpListener(ipPoint);

        try
        {
            server.Start();
            Console.WriteLine("Server started! Waiting for connections...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                NetworkStream ns = client.GetStream();

                using StreamWriter sw = new StreamWriter(ns, leaveOpen: true) { AutoFlush = true };

                if (tcpClients.Count < 5)
                {
                    sw.WriteLine("Сервер: Вітаємо на сервері!");
                    Task.Run(() => Start(client));
                }
                else
                {
                    sw.WriteLine("Сервер: Сервер переповнений!");
                    client.Close();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Server error: {ex.Message}");
        }
    }
}