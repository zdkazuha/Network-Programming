using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MimeKit;
using System.IO;
using System.Text;


internal class Program
{
    const string username = "aartemchelyk@gmail.com";
    const string password = "hdqs eaev szsi ndby ";
    const string otherUsers = "jabeka8662@javbing.com";
    private static void Main(string[] args)
    {
        //MimeMessage message = new();

        //message.From.Add(new MailboxAddress("Artem",username));
        //message.To.Add(new MailboxAddress("User", otherUsers));

        //message.Subject = "Hello";
        //message.Importance = MessageImportance.High;

        //var body = new TextPart("plain")
        //{
        //    Text = @"Hey Alice,

        //    What are you up to this weekend? Monica is throwing one of her parties on
        //    Saturday and I was hoping you could make it.

        //    Will you be my +1?

        //    -- Joey
        //    "
        //};
        //var path = @"C:\Users\SystemX\Desktop\Array\Network Programming\Network Programming\07_MailKit_Imap\07_MailKit_Imap\bin\Debug\net9.0\images.jpg";

        //var attachment = new MimePart("image", "gif")
        //{
        //    Content = new MimeContent(File.OpenRead(path), ContentEncoding.Default),
        //    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
        //    ContentTransferEncoding = ContentEncoding.Base64,
        //    FileName = Path.GetFileName(path)
        //};

        //var multipart = new Multipart("mixed");
        //multipart.Add(body);
        //multipart.Add(attachment);
        //message.Body = multipart;

        //using (var client = new SmtpClient())
        //{
        //    client.Connect("smtp.gmail.com",465,MailKit.Security.SecureSocketOptions.SslOnConnect);
        //    client.Authenticate(username, password);

        //    client.Send(message);

        //}

        // Get MAils (IMAP)
        Console.OutputEncoding = Encoding.UTF8;
        using (var client = new ImapClient())
        {
            client.Connect("imap.gmail.com",993, MailKit.Security.SecureSocketOptions.SslOnConnect);
            client.Authenticate(username, password);

            foreach (var item in client.GetFolders(client.PersonalNamespaces[0]))
            {
                Console.WriteLine(item.Name);
            }

            client.GetFolder(MailKit.SpecialFolder.Sent).Open(MailKit.FolderAccess.ReadWrite);
            var folder = client.GetFolder(MailKit.SpecialFolder.Sent);

            var id = folder.Search(SearchQuery.All)[folder.Search(SearchQuery.All).Count - 1];
            var m = folder.GetMessage(id);
            Console.WriteLine($"{m.Date} {m.Subject}");

            folder.MoveTo(id, client.GetFolder(SpecialFolder.Junk));
            folder.AddFlags(id, MessageFlags.Deleted, true);
            folder.Expunge();

            //foreach (var item in folder.Search(MailKit.Search.SearchQuery.All))
            //{ 
            //    var m = folder.GetMessage(item);
            //    Console.WriteLine($"{m.Date} {m.Subject}");
            //
        }
    }
}