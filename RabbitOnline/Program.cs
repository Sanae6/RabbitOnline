﻿using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using CommandLine;
using Esprima;
using Jint;
using Jint.Runtime;
using Jint.Runtime.Interop;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace RabbitOnline
{
    public class Program
    {
        public class Options
        {
            [Value(0, HelpText = "Where the data.win for My Rabbits Are Gone is.", MetaValue = "data.win"
                , Required = true)]
            public string FileLocation { get; set; }
            
            [Option('d', "dont-run"
                , HelpText = "Don't run the game after patching.", Default = false)]
            public bool DoNotRunLater { get; set; }

            [Option('c', "close"
                , HelpText = "Close the game before patching if it's open.", Default = false)]
            public bool CloseBeforeStarting { get; set; }

            [Option('m', "dump"
                , HelpText = "Dump all code objects to GML.", Default = false)]
            public bool Dump { get; set; }
            
            [Option('r',"run-two",Default = false)]
            public bool RunTwo { get; set; }//helps for development
        }

        public static UndertaleData Data;
        public static void Main(string[] args)
        {
            UndertaleData data;
            Parser.Default.ParseArguments<Options>(args).WithParsed(opts =>
            {
                try
                {
                    Process[] processes = Process.GetProcessesByName("MyRabbitsAreGone");
                    if (processes.Length != 0)
                    {
                        if (opts.CloseBeforeStarting)
                        {
                            Console.WriteLine("Closing all open instances of My Rabbits Are Gone");
                            foreach (var proc in processes) proc.CloseMainWindow();
                        }
                        else Console.WriteLine("Close My Rabbits Are Gone before continuing.");

                        while (processes.Length != 0)
                        {
                            Thread.Sleep(1000);
                            processes = Process.GetProcessesByName("MyRabbitsAreGone");
                        }

                        Console.WriteLine("Continuing!");
                    }

                    if (Path.GetFileNameWithoutExtension(opts.FileLocation) != "data") //cross platform momento
                        throw new Exception("File name is not data.win!");
                    if (File.Exists(opts.FileLocation + ".bak"))
                    {
                        Console.WriteLine("Found a backup, overwriting current data.win with it.");
                        File.Delete(opts.FileLocation);
                        opts.FileLocation += ".bak";
                    }
                    else
                    {
                        Console.WriteLine("Backup not found, doing that now.");
                        File.Copy(opts.FileLocation, opts.FileLocation + ".bak");
                    }

                    if (!File.Exists(opts.FileLocation)) throw new Exception("File does not exist!");

                    var read = new UndertaleReader(File.OpenRead(opts.FileLocation));
                    Data = data = read.ReadUndertaleData();
                    read.Close();
                    var md5 = MD5.Create();
                    Engine engine = new Engine(cfg =>
                        cfg.AllowClr(
                            typeof(UndertaleData).Assembly,
                            typeof(UndertaleGameObject).Assembly,
                            typeof(File).Assembly).DebugMode());
                    engine = engine.SetValue("RoomGameObject",
                        TypeReference.CreateTypeReference(engine, typeof(UndertaleRoom.GameObject))).SetValue(
                        "EventAction",
                        TypeReference.CreateTypeReference(engine, typeof(UndertaleGameObject.EventAction))).SetValue(
                        "UEvent", TypeReference.CreateTypeReference(engine, typeof(UndertaleGameObject.Event)));
                    engine = engine.SetValue("log", new Action<object>(Console.WriteLine))
                        .SetValue("Data", data)
                        .SetValue("emmdeefive", new Func<string, string>((s) =>
                        {
                            var hash = md5.ComputeHash(Encoding.ASCII.GetBytes(s));
                            StringBuilder sb = new StringBuilder();
                            for (int i = 0; i < hash.Length; i++) sb.Append(hash[i].ToString("X2"));
                            return sb.ToString();
                        }));
                    Console.WriteLine("Passing control to patcher.js");
                    engine.Execute(File.ReadAllText("patcher.js")).Invoke("main");
                    Console.WriteLine("Exited JavaScript mode.");
                    Console.WriteLine("Now writing data.win...");
                    if (opts.FileLocation.EndsWith(".bak"))
                        opts.FileLocation = opts.FileLocation.Slice(0, opts.FileLocation.Length - 4);
                    Console.WriteLine(opts.FileLocation);
                    var write = new UndertaleWriter(File.OpenWrite(opts.FileLocation));
                    write.WriteUndertaleData(data);
                    write.Close();
                    Console.WriteLine("Written!");
                    if (!opts.DoNotRunLater)
                    {
                        Process.Start(Path.Combine(
                            Path.GetDirectoryName(opts.FileLocation) ?? throw new InvalidOperationException(),
                            "MyRabbitsAreGone.exe"));
                        if (opts.RunTwo)
                            Process.Start(Path.Combine(
                                Path.GetDirectoryName(opts.FileLocation) ?? throw new InvalidOperationException(),
                                "MyRabbitsAreGone.exe"));
                        Console.WriteLine("Now starting the game :)");
                        Process.GetCurrentProcess().Kill();
                    }

                    Console.WriteLine("Seeya!");
                }
                catch (JavaScriptException e)
                {
                    Console.Error.WriteLine("Error \"{0}\" at {1}:{2}", e.Message, e.LineNumber, e.Column);
                    Console.Error.WriteLine(e.CallStack);
                    Environment.Exit(1);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("{0}: \"{1}\"", e.GetType().Name, e.Message);
                    Console.Error.WriteLine(e.Source);
                    Environment.Exit(1);
                }
            });
        }
    }
}