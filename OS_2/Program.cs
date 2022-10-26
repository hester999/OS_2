
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Security.Cryptography;

// 1115dd800feaacefdf481f1f9070374a2a81e27880f187396db67958b207cbad
// 3a7bd3e2360a3d29eea436fcfb7e44c735d117c42d1c1835420b6b9942dd4f1b
// 74e1bb62f8dabb8125a58852b63bdf6eaef667cb56ac7f7cdba6d7305c50a22f

    
class Program
{
    
    static void Main(string[] args)
    {
        Print.menu();
    }
}




 class Print
{
    static public bool flag = false;
    
    public static void menu()
    {
        bool menu_flag = true;
        while (menu_flag)
        {
            Console.WriteLine("1. Complete the task.");
            Console.WriteLine("2. Clear console.");
            Console.WriteLine("3. Exit program.");
            Console.Write("Choose point menu: ");

            int answer = int.Parse(Console.ReadLine());

            c:switch (answer)
            {
                case 1:
                    Console.WriteLine("Enter count stream ");
                    
                    int count = int.Parse(Console.ReadLine());

                    Console.WriteLine("Enter hash");
                    
                    string passw = Console.ReadLine();
        
                    Channel<string> channel = Channel.CreateBounded<string>(count);
                    
                    Stopwatch time = new();
                    
                    time.Reset();
                    
                    time.Start();
        
                    var prod = Task.Run(() => { new Producer(channel.Writer); });
        
                    Task[] streams = new Task[count + 1];
        
                    streams[0] = prod;
        
                    for (int i = 1; i < count + 1; i++)
                    {
                        streams[i] = Task.Run(() => { new Consumer(channel.Reader, passw); });
                    }
       
                    Task.WaitAny(streams);
                    
                    time.Stop();
                    
                    Console.WriteLine($"Selection time: {time.Elapsed}");
                    
                    Console.WriteLine("Press enter to exit the menu ");
                    Console.ReadLine();
                    
                    flag = false;
                    break;
                
                case 2:
                    Console.Clear();
                    break;

                case 3:
                {
                    menu_flag = false;
                    break;
                    
                }
                
            }
            
        }
        
        
    }
}

class Producer
{
    private ChannelWriter<string> Writer;
    

    public Producer(ChannelWriter<string> _writer)
    {
        Writer = _writer;

        Task.WaitAll(pass());
    }

    public async Task pass()
    {

        while (await Writer.WaitToWriteAsync())
        {

            char[] Dictionary =
            {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i',
                'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r',
                's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
            };
            int length = Dictionary.Length;
            for (int ch1 = 0; ch1 < length; ch1++)
            {
                string a = Convert.ToString(Dictionary[ch1]);
                for (int ch2 = 0; ch2 < length; ch2++)
                {
                    string b = Convert.ToString(Dictionary[ch2]);
                    for (int ch3 = 0; ch3 < length; ch3++)
                    {
                        string c = Convert.ToString(Dictionary[ch3]);
                        for (int ch4 = 0; ch4 < length; ch4++)
                        {
                            string d = Convert.ToString(Dictionary[ch4]);
                            for (int ch5 = 0; ch5 < length; ch5++)
                            {
                                string e = Convert.ToString(Dictionary[ch5]);

                                string password = a + b + c + d + e;

                                if (!Print.flag)
                                {
                                   await Writer.WriteAsync((password));

                                }
                                else
                                {
                                    Writer.Complete();
                                }

                            }
                        }
                    }
                }
            }
        }

    }
}

class Consumer
    {
        private ChannelReader<string> Reader;
        private string pass;

        public Consumer(ChannelReader<string> _reader, string _pass)
        {
            Reader = _reader;
            pass = _pass;
            Task.WaitAll(Run());

        }

        async Task Run()
        {
            while (await Reader.WaitToReadAsync())
            {
                if (!Print.flag)
                {
                    var item = await Reader.ReadAsync();
                    
                    
                    
                    if (FoundHash(item) == pass)
                    {
                        Console.WriteLine($"Password - {item}");
                        Print.flag = true;
                    }
                }
                else return;
            }
        }

        static string FoundHash(string str)
        {
            using var sha265 = SHA256.Create();

            byte[] byts = sha265.ComputeHash(Encoding.UTF8.GetBytes(str));

            var sb = new StringBuilder();

            for (int i = 0; i < byts.Length; i++)
            {
                sb.Append(byts[i].ToString("x2"));
                
            }

            return sb.ToString();
        }

    }



    
