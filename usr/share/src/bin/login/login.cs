using System;
using System.IO;
using System.Diagnostics;
namespace cmdlinux {
class login {
static string username;
// this one's needed across all apps.
static string rootpath=Environment.GetEnvironmentVariable("rootpath.sl");
static void Main() {
// wait 2 seconds to let the user see kernel messages.
System.Threading.Thread.Sleep(2000);

// set up some strings
string passwordShaCompare;
string shell;
string shebang;
string passwordSha;
string LoginHostSource=Environment.GetEnvironmentVariable("hostname.sl");

string motd=File.ReadAllText(""+rootpath+"\\etc\\motd");
// strip crlf from /etc/hostname
string LoginHost = System.Text.RegularExpressions.Regex.Replace(LoginHostSource, @"\t|\n|\r", "");
// let the user see the admin's message.
Console.WriteLine(motd);
// the username entry logic
Username:

Console.Write("/dev/console "+LoginHost+" login:");
username=Console.ReadLine();
// couple of exploit stoppy things
if(username.Contains(".")) {

goto Username;
}
if(username.Contains("/")) {
goto Username;
}
if(username.Contains("\\")) {
goto Username;
}
if(username.Contains(" ")) {
goto Username;
}

if(username.Contains("	")) {
goto Username;
}
if(username=="") {
goto Username;
}
// stupid stupid way to bailout if said user doesn't live.
if(!Directory.Exists(""+rootpath+"\\home\\"+username+"")) {
System.Threading.Thread.Sleep(100);
goto Username;
}
// password entry logic
Console.Write("password for "+username+"@"+LoginHost+":");
// deligate password entry to the ReadPassword() method for security.
string password=ReadPassword();
// instantiate libSha for password verification
Console.Clear();
var libSha = new Process();
libSha.StartInfo = new ProcessStartInfo( ""+rootpath+"\\libexec\\libSha.em" ) 
{
        Arguments = password,
        UseShellExecute = false,
        RedirectStandardOutput = true,
        CreateNoWindow = true
};
Console.WriteLine(".....");

libSha.Start();
Console.WriteLine(".....");
// capture libSha's output, as that's the sha512 of the password.
while (!libSha.StandardOutput.EndOfStream) {
Console.WriteLine(".....");
Environment.SetEnvironmentVariable("passwordSha.sl", libSha.StandardOutput.ReadLine());
Console.WriteLine(".....");
// now we have the sha of the password that the user entered.

}
Console.WriteLine(".....");
passwordSha=Environment.GetEnvironmentVariable("passwordSha.sl");
Console.WriteLine(".....");
Environment.SetEnvironmentVariable("passwordSha.sl", "");
// get that out of memory, very very bad!
// now we read ~/.passwd
try {
Console.WriteLine(".....");
passwordShaCompare =File.ReadAllText(""+rootpath+"\\home\\"+username+"\\.passwd");
}
// catch users who don't have password files (non-interactive entities):
catch(System.IO.DirectoryNotFoundException EX) {
Console.WriteLine("You don't exist. Go away!");
// old unix error message.
goto Username;
}
// verification of sha against stored one.
Console.WriteLine(".....");
if(passwordSha == passwordShaCompare) {
Console.Clear();
if(username == "root") {
shebang="#";
}
else {
shebang="$";

}
setupEnvironment();
}
// if the user got here, they failed to authenticate
Console.WriteLine("Bad Password.");


goto Username;

}
     public static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");

                    password += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        // remove one character from the list of password characters
                        password = password.Substring(0, password.Length - 1);
                        // get the location of the cursor
                        int pos = Console.CursorLeft;
                        // move the cursor to the left by one character
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        // replace it with space
                        Console.Write(" ");
                        // move the cursor to the left by one character again
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
if(string.IsNullOrEmpty(password)) {
Console.Beep(800,20);
}
                }
                info = Console.ReadKey(true);
            }
            // add a new line because user pressed enter at the end of their password
            Console.WriteLine();
            return password;
        }
static void setupEnvironment() {
// TODO: SETUP environment.
if(username != "root") {
Environment.SetEnvironmentVariable("shebang.sl", "$");
}
else {
Environment.SetEnvironmentVariable("shebang.sl", "#");
}
Environment.SetEnvironmentVariable("username.sl", username);
Console.Clear();
var shell = new Process();
shell.StartInfo = new ProcessStartInfo( ""+rootpath+"\\bin\\tsh" ) 
{
        UseShellExecute = false,
        CreateNoWindow = false
};

shell.Start();
shell.WaitForExit();
Environment.Exit(0);
}
}
}
