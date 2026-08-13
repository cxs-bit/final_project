using Spectre.Console;
using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
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

    private static async Task Main()
    {
        await Menu();
    }


    private static async Task Menu()
    {
        string username = System.Environment.UserName;
        int option = 0;
        while (option != 4)
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine($"[bold Gold3_1]Welcome[/] to [blue]My Final Project[/] {username}!");
            var table = new Table()
            .SimpleHeavyBorder()
            .Expand()
                .AddColumn("[white]Actions[/]")
                .AddColumn("[white]Description[/]")
                .AddRow("[teal]1. Encrypt[/]", "[teal]Encrypts the message given by the user.[/]")
                .AddRow("[cyan]2. Decrypt[/]", "[cyan]Decrypts a given message.[/]")
                .AddRow("[blue]3. Encrypt and Decrypt[/]", "[blue]Encrypts a message, and after decrypts the message.[/]")
                .AddRow("[white]4. Leave[/]", "[white]Exits the program.[/]");
            AnsiConsole.Write(table);
            option = AnsiConsole.Ask<int>("Select an [bold blue]option[/]:");

            switch (option)
            {
                case 1:
                    ShowCryptogram(await EncryptAsync(ReadMessage("message")));
                    break;

                case 2:
                    ShowMessage(await DecryptAsync(ReadMessage("cryptogram")));
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

    private static string ReadMessage(string type)
    {
        switch (type)
        {
            case "message":
                string msg = AnsiConsole.Ask<string>("\nInsert the message to [bold blue]encrypt[/]");
                return msg;

            case "cryptogram":
                string cryptogram = AnsiConsole.Ask<string>("\nInsert the [bold blue]cryptogram[/](numbers separated with spaces):");
                return cryptogram;

            default:
                AnsiConsole.MarkupLine("[bold red]ERROR:[/][red]The type specified doenst exist[/]");
                return "ERROR";

        }
    }
    private static void ShowCryptogram(string msg)
    {
        AnsiConsole.MarkupLine($"[green]Done[/], the encrypted message is: [/][bold teal]{msg}[/]");
        HandleResultOptions(msg);
    }

    private static void ShowMessage(string msg)
    {
        AnsiConsole.MarkupLine($"[green]Done![/], the original message is: [bold teal]{msg}[/]");
        HandleResultOptions(msg);
    }

    private static void HandleResultOptions(string msg)
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select an option:")
                .AddChoices(new[] { "Copy to clipboard", "Continue", "Exit" })
        );

        switch (choice)
        {
            case "Copy to clipboard":
                if (TryCopyToClipboard(msg))
                {
                    AnsiConsole.MarkupLine("[green]Copied to clipboard.[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Clipboard not available on this system.[/]");
                }
                Console.ReadKey(true);
                break;

            case "Continue":
                break;

            case "Exit":
                Environment.Exit(0);
                break;
        }
    }

    private static bool TryCopyToClipboard(string text)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var psi = new ProcessStartInfo("cmd", "/c clip")
                {
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (var p = Process.Start(psi))
                {
                    p.StandardInput.Write(text);
                    p.StandardInput.Close();
                    p.WaitForExit();
                }
                return true;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var psi = new ProcessStartInfo("pbcopy")
                {
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (var p = Process.Start(psi))
                {
                    p.StandardInput.Write(text);
                    p.StandardInput.Close();
                    p.WaitForExit();
                }
                return true;
            }

            // Try wl-copy (Wayland)
            var psiWl = new ProcessStartInfo("wl-copy")
            {
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            try
            {
                using (var p = Process.Start(psiWl))
                {
                    p.StandardInput.Write(text);
                    p.StandardInput.Close();
                    p.WaitForExit();
                }
                return true;
            }
            catch { }

            // Try xclip (X11)
            var psiXclip = new ProcessStartInfo("xclip", "-selection clipboard")
            {
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            try
            {
                using (var p = Process.Start(psiXclip))
                {
                    p.StandardInput.Write(text);
                    p.StandardInput.Close();
                    p.WaitForExit();
                }
                return true;
            }
            catch { }

            return false;
        }
        catch
        {
            return false;
        }
    }
    private static async Task<string> DecryptAsync(string cryptogram)
    {
        string originalMessage = string.Empty;
        await AnsiConsole.Status()
        .Spinner(Spinner.Known.Star)
        .SpinnerStyle(Style.Parse("DarkCyan"))
        .StartAsync("[DarkCyan]Cryptogram received...[/]", async ctx =>
        {
            await Task.Delay(1000);
            ctx.Spinner(Spinner.Known.Arc);
            ctx.SpinnerStyle(Style.Parse("White"));
            ctx.Status("[White]Spliting up the cryptogram...[/]");

            var tokens = cryptogram.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            var numbers = new List<int>();
            foreach (var t in tokens)
            {
                if (!int.TryParse(t, out var n))
                {
                    AnsiConsole.MarkupLine($"[red]ERROR: Invalid token'{t}'[/]");
                }
                numbers.Add(n);
            }

            await Task.Delay(1000);
            ctx.SpinnerStyle(Style.Parse("Yellow"));
            ctx.Status("[Yellow]Validating the length...[/]");
            if (numbers.Count % 3 != 0)
            {
                AnsiConsole.MarkupLine($"[red]ERROR: The cryptogram is not divisible by 3[/]");
            }

            else
            {
                await Task.Delay(1000);
                ctx.SpinnerStyle(Style.Parse("Cyan"));
                ctx.Status("[Cyan]Starting decrypting the cryptogram...[/]");

                var sb = new StringBuilder();
                for (int i = 0; i < numbers.Count; i += 3)
                {

                    // Convierte cada uno de los caracteres a numeros
                    int y1 = numbers[i];
                    int y2 = numbers[i + 1];
                    int y3 = numbers[i + 2];

                    // Operación Matriz Fila * Matriz A inversa para recuperar los valores iniciales
                    int x1 = (y1 * A_Inversa[0, 0]) + (y2 * A_Inversa[1, 0]) + (y3 * A_Inversa[2, 0]);
                    int x2 = (y1 * A_Inversa[0, 1]) + (y2 * A_Inversa[1, 1]) + (y3 * A_Inversa[2, 1]);
                    int x3 = (y1 * A_Inversa[0, 2]) + (y2 * A_Inversa[1, 2]) + (y3 * A_Inversa[2, 2]);

                    sb.Append(ConvertChar(x1));
                    sb.Append(ConvertChar(x2));
                    sb.Append(ConvertChar(x3));

                    await Task.Delay(100);
                }
                originalMessage = sb.ToString().TrimEnd();
            }
        });
        return originalMessage;
    }
    private static async Task<string> EncryptAsync(string msg)
    {
        string encryptedMessage = string.Empty;
        await AnsiConsole.Status()
        .Spinner(Spinner.Known.Dots)
        .SpinnerStyle(Style.Parse("DarkCyan"))
        .StartAsync("[DarkCyan]Message received...[/]", async ctx =>
        {
            await Task.Delay(1000);
            ctx.Spinner(Spinner.Known.Arc);
            ctx.SpinnerStyle(Style.Parse("Cyan"));
            ctx.Status("[Cyan]Cleaning up message...[/]");
            string cleanedMessage = CleanupText(msg.ToUpper());

            await Task.Delay(1000);
            ctx.SpinnerStyle(Style.Parse("White"));
            ctx.Status("[White]Spliting up the message...[/]");
            int residual = cleanedMessage.Length % 3;
            if (residual != 0)
            {
                cleanedMessage = cleanedMessage.PadRight(cleanedMessage.Length + (3 - residual), ' ');
            }

            await Task.Delay(1500);
            ctx.SpinnerStyle(Style.Parse("Aquamarine1"));
            ctx.Spinner(Spinner.Known.Point);
            ctx.Status("[Aquamarine1]Starting encription...[/]");
            List<int> criptograma = new List<int>();
            for (int i = 0; i < cleanedMessage.Length; i += 3)
            {
                // Convierte cada uno de los caracteres a numeros
                int x1 = ConvertCode(cleanedMessage[i]);
                int x2 = ConvertCode(cleanedMessage[i + 1]);
                int x3 = ConvertCode(cleanedMessage[i + 2]);

                // Operación Matriz Fila * Matriz A 
                int y1 = (x1 * A[0, 0]) + (x2 * A[1, 0]) + (x3 * A[2, 0]);
                int y2 = (x1 * A[0, 1]) + (x2 * A[1, 1]) + (x3 * A[2, 1]);
                int y3 = (x1 * A[0, 2]) + (x2 * A[1, 2]) + (x3 * A[2, 2]);

                criptograma.Add(y1);
                criptograma.Add(y2);
                criptograma.Add(y3);
            }

            await Task.Delay(2500);
            ctx.Status("Done...");
            encryptedMessage = string.Join(" ", criptograma);
        });
        return encryptedMessage;
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