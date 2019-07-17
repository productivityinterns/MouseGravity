using System;
using System.Runtime.InteropServices;
using System.Drawing;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
namespace MouseGravity
{
   
class program {
    /*
        This constant tells the SystemParametersInfo uiAction param that
        it will be changing mouse speed
     */
    [DllImport("User32.dll")]
    public static extern IntPtr GetDC(IntPtr hwnd);
    [DllImport("User32.dll")]
    public static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);

    public static Task DoWork(Boolean toggled) {
        while(true) {
             if (Console.KeyAvailable) {
                 if ( Console.ReadKey(true).Key == ConsoleKey.Escape) {
                      toggled = !toggled;
                 }
            }
        } 
    }
    static async Task Main(string[] args)
    {
        Cursor c = new Cursor();
        Boolean toggled = true;
        ScriptEngine engine = Python.CreateEngine();
        engine.ExecuteFile(@"test.py");
        Task.Run(() => {
            while(true) {
             if (Console.KeyAvailable) {
                 if ( Console.ReadKey(true).Key == ConsoleKey.Escape) {
                     Console.WriteLine("ay lamoe");
                      toggled = !toggled;
                      c.ChangeMouseSpeed(10);
                      
                 }
            }
        } 
        });
        
        while(true) {         
            if(toggled) {
                c.Logic();
            }
            

            /* 
            THis is how to draw to a regulare screen without needing a form windo, so we can draw an overlay 
            ontop of the screen
            thanks SO: https://stackoverflow.com/questions/14385838/draw-on-screen-without-form
            */
            // IntPtr desktopPtr = GetDC(IntPtr.Zero);
            // Graphics g = Graphics.FromHdc(desktopPtr);
            // SolidBrush b = new SolidBrush(Color.White);
            // g.FillEllipse(b,new Rectangle(800, 500, 400, 400));
            // g.Dispose();
            // ReleaseDC(IntPtr.Zero, desktopPtr);
            
            
        }
        
        
    }
}
}
