using System;
using System.Runtime.InteropServices;
using System.Drawing;

using System.Windows;
using System.Threading;
namespace MouseGravity
{
   
class program {
    /*
        This constant tells the SystemParametersInfo uiAction param that
        it will be changing mouse speed
     */
    public const UInt32 SPI_SETMOUSESPEED = 0x0071;

    /*
        Need to specify the fucntin being used from the User32.dll
     */
    [DllImport("User32.dll")]
    static extern Boolean SystemParametersInfo(
        UInt32 uiAction, 
        UInt32 uiParam, 
        UInt32 pvParam,
        UInt32 fWinIni);

    /*
        This statement pulls the GetCursorPos function from the User32.dll
     */
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetCursorPos(out POINT lpPoint); 
    
    /*
        This struct holds the coords of objects on screen, either cursor or target
     */
    public struct POINT {
        public int X,Y;
        public POINT(int x, int y) {
            this.X = x;
            this.Y = y;
        }
    }
    /*
        This function gets the mouse position and prints it to console
        Print is mainly for debuging purposes
     */
    static POINT GetMousePosition() {
        POINT p;
        if (GetCursorPos(out p))
        {
            Console.WriteLine(Convert.ToString(p.X) + ";" + Convert.ToString(p.Y));
        }
        return p;
    }
    /*
        This checks to see if the cursor is within a specified range of the target coords
        within a certain margin
     */
    static Boolean CheckDistance(POINT userPos, POINT targetPos) {
        int margin = 75;
        if (userPos.X >= targetPos.X - margin && userPos.X <= targetPos.X + margin ) {
            if (userPos.Y >= targetPos.Y - margin && userPos.Y <= targetPos.Y + margin ) {
                Console.WriteLine("In the target");
                return true;
            }
        }
        return false;
    }
    /*
        This is hardcoded but will eventually get the targets position on screen from whatever OCR we use
     */
    static POINT GetTargetPostition() {
        return new POINT(600,300);
    }
    /*
        Speed must be between 0 and 20
        */
    static void ChangeMouseSpeed(UInt32 speed) {
        // Parameter validation
        if (speed > 20) {
            speed = 20;
        } else if (speed < 0) {
            speed = 0;
        }

            SystemParametersInfo(
            SPI_SETMOUSESPEED, 
            0, 
            speed, 
            0);
    }
    
        
    static void Main(string[] args)
    {
        while(true) {
            POINT p = GetMousePosition();
            if(CheckDistance( GetMousePosition(), GetTargetPostition() )) {
                ChangeMouseSpeed(1);
            } else {
                ChangeMouseSpeed(8);
            }
            Thread.Sleep(100);
        }
        
    }
}
}
