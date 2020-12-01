using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mirai_CSharp;
using Mirai_CSharp.Models;

namespace QChatClient
{
    class Program
    {
        public const string TITLE = "QChatClient^Mirai - ";
        public const string DEFAULT_ADDR = "127.0.0.1";
        public const int DEFAULT_PORT = 8888;
        public const long DEFAULT_ACCOUNT = 1234567890;
        public const string DEFAULT_PASSWORD = "PASSW0RD";

        private static QClient client;
        private static CommandHandler cmd;

        public static int cur_l, cur_t;

        static void Main(string[] args)
        {
            Console.Title = TITLE + "轻量便携的QQ客户端";
            try
            {
                Console.Clear();
                Console.CursorTop = 0;
                Console.BufferWidth = Console.WindowWidth;
                Console.BufferHeight = Console.WindowHeight;
                Program.WriteIL("请提供您的MiraiHttpApi认证信息以便登录。\n");
                string addr = Ask("服务器地址", DEFAULT_ADDR);
                int port = int.Parse(Ask("端口号", DEFAULT_PORT.ToString()));
                long account = long.Parse(Ask("账号", DEFAULT_ACCOUNT.ToString()));
                string passwd = Ask("Mirai认证密钥", DEFAULT_PASSWORD);
                client = new QClient(addr, port, passwd, account);
                //client.
                cmd = new CommandHandler(client);
                Console.Clear();
                Console.CursorTop = 0;
                Console.BufferWidth = Console.WindowWidth;
                Console.BufferHeight = Console.WindowHeight;
                Program.WriteLine("\n\n===============\n已连接\n/con 列出会话列表\n" +
                    "/fri 列出好友列表 \n/gro 列出群列表 \n" +
                    "/sel [ID] 切换到指定会话\n/cof [QQ] 与指定好友聊天\n" +
                    "/cog [GROUP] 与指定群聊天\n/cot [GROUP] [QQ] 与指定群中的群员临时聊天");

                cur_l = Console.CursorLeft;
                cur_t = Console.CursorTop;
                while (true)
                {
                    string cmds = "";
                    cur_l = Console.CursorLeft;
                    cur_t = Console.CursorTop;
                    lock ("ConsoleOp")
                    {
                        Console.SetCursorPosition(0, Console.BufferHeight - 1);
                        Console.Write(">>");
                        if (Console.CursorTop + 1 >= Console.BufferHeight)
                        {
                            Console.BufferHeight++;
                        }
                    }
                    cmds = Console.ReadLine();
                    lock ("ConsoleOp")
                    {
                        //Console.BufferHeight++;
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        Console.Write("                                                              ");
                        Console.SetCursorPosition(cur_l, cur_t);
                    }
                    cmd.command(cmds);
                    Thread.Sleep(500);
                }
            }
            catch (Exception e)
            {
                WriteLine("！故障：" + e.Message);
            }
        }

        public static string Ask(string msg, string def)
        {
            WriteIL(msg + "[" + def + "]>");
            string addr = Console.ReadLine();
            if (addr.Length < 2) addr = def;
            return addr;
        }

        public static void WriteLine(string str)
        {
            Write(str + "\n");
        }
        public static void Write(string str)
        {
            Console.CursorLeft = cur_l;
            Console.CursorTop = cur_t;
            int ll = str.Split('\n').Count() - 1;
            //Console.BufferHeight += str.Split('\n').Count() - 1;
            if (Console.CursorTop + ll >= Console.BufferHeight)
            {
                Console.BufferHeight += ll;
            }
            Console.Write(str);
            /*if (Console.CursorTop >= Console.BufferHeight - 1)
            {
                Console.BufferHeight++;
                Console.Write("\n>>");
            }*/
            cur_l = Console.CursorLeft;
            cur_t = Console.CursorTop;
            Console.SetCursorPosition(0, Console.BufferHeight - 1);
            Console.Write(">>");
        }
        public static void WriteIL(string str)
        {
            int ll = str.Split('\n').Count() - 1;
            //Console.BufferHeight += str.Split('\n').Count() - 1;
            if (Console.CursorTop + ll >= Console.BufferHeight)
            {
                Console.BufferHeight += ll;
            }
            Console.Write(str);
            /*if (Console.CursorTop >= Console.BufferHeight - 1)
            {
                Console.BufferHeight++;
                Console.Write("\n>>");
            }*/
            cur_l = Console.CursorLeft;
            cur_t = Console.CursorTop;
        }

        public static void Clear()
        {
            Console.Clear();
            cur_l = Console.CursorLeft;
            cur_t = Console.CursorTop;
        }
    }
}
