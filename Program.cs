using Microsoft.VisualBasic;
using Spectre.Console;
using System;
using System.Text;
class Program
{
    static readonly int[,] A = {
            {  1, -2,  2 },
            { -1,  1,  3 },
            {  1, -1, -4 }
        };

    static readonly int[,] A_Inversa = {
            { -1, -10, -8 },
            { -1,  -6, -5 },
            {  0,  -1, -1 }
        };

    private static void Main()
    {
        Menu();
    }


    private static void Menu()
    {
        string username = System.Environment.UserName;
        int option = 0;
        while (option != 4)
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine($"[bold blue]Welcome[/] to [green]My Final Project[/] {username}!");
            var table = new Table()
                .AddColumn("Actions")
                .AddColumn("Description")
                .AddRow("[green]1. Encrypt[/]", "Encrypts the message given by the user")
                .AddRow("[blue]2. Decrypt[/]", "Dencrypts a given message")
                .AddRow("[yellow]3. Encrypt and Decrypt[/]", "Encrypts a message, and after decrypts the message")
                .AddRow("[gray]4. Leave[/]", "Exits the program");
            AnsiConsole.Write(table);
            option = AnsiConsole.Ask<int>("[green]Select an option:[/]");

            switch (option)
            {
                case 1:
                    ShowMessage(Encrypt(ReadMessage()));
                    break;

                case 2:
                    break;

                case 3:
                    break;

                case 4:
                    break;

                default:
                    break;
            }
        }
    }

    private static string ReadMessage()
    {
        string msg = AnsiConsole.Ask<string>("[white]\nInsert the message to[/] [bold blue]encrypt[/]");
        return msg;
    }
    private static void ShowMessage(string msg)
    {
        AnsiConsole.MarkupLine($"[green]The message is: [bold]{msg}[/][/]");
        Console.ReadKey();
    }
    private static string Encrypt(string msg)
    {
        string encryptedMessage = string.Empty;
        AnsiConsole.Status()
        .Start("Message received...", ctx =>
        {
            Thread.Sleep(2500);
            ctx.Status("Cleaning up message...");
            string cleanedMessage = CleanupText(msg.ToUpper());

            Thread.Sleep(2500);
            ctx.Status("Spliting up the message...");
            int residual = cleanedMessage.Length % 3;
            if (residual != 0)
            {
                cleanedMessage = cleanedMessage.PadRight(cleanedMessage.Length + (3 - residual), ' ');
            }

            Thread.Sleep(2500);
            ctx.Status("Starting encription...");
            List<int> criptograma = new List<int>();
            for (int i = 0; i < cleanedMessage.Length; i += 3)
            {
                int x1 = ConvertCode(cleanedMessage[i]);
                int x2 = ConvertCode(cleanedMessage[i + 1]);
                int x3 = ConvertCode(cleanedMessage[i + 2]);

                // Operación Matriz Fila (1x3) * Matriz A (3x3)
                int y1 = (x1 * A[0, 0]) + (x2 * A[1, 0]) + (x3 * A[2, 0]);
                int y2 = (x1 * A[0, 1]) + (x2 * A[1, 1]) + (x3 * A[2, 1]);
                int y3 = (x1 * A[0, 2]) + (x2 * A[1, 2]) + (x3 * A[2, 2]);

                criptograma.Add(y1);
                criptograma.Add(y2);
                criptograma.Add(y3);
            }

            Thread.Sleep(2500);
            ctx.Status("Done...");
            encryptedMessage = string.Join(" ", criptograma);
        });
        return encryptedMessage;
    }
    private static void Decrypt(string msg)
    {

    }
    static string CleanupText(string msg)
    {
        StringBuilder sb = new StringBuilder();
        foreach (char c in msg.ToUpper())
        {
            if (c == ' ' || (c >= 'A' && c <= 'Z'))
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }
    static int ConvertCode(char c)
    {
        if (c == ' ') return 0;
        return c - 'A' + 1;
    }

    static char ConvertChar(int codigo)
    {
        if (codigo == 0) return ' ';
        return (char)('A' + (codigo - 1));
    }
}