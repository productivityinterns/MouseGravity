using System;
using System.Runtime.InteropServices;
using System.Drawing;

using System.Windows;
using System.Threading;

namespace MouseGravity {
class Cursor {

    public Cursor() {

    }
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
    
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool SetCursorPos(int X, int Y);


   
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
        public struct FloatPoint {
        public float X,Y;
        public FloatPoint(float x, float y) {
            this.X = x;
            this.Y = y;
        }
        public static explicit operator FloatPoint(POINT p) => new FloatPoint((float)p.X, (float) p.Y);
        public static bool operator ==(FloatPoint f1, FloatPoint f2) 
        {
            return (f1.X==f2.X && f1.Y == f2.Y);
        }

        public static bool operator !=(FloatPoint f1, FloatPoint f2) 
        {
            return !(f1.X==f2.X && f1.Y == f2.Y);
        }
    }
    /*
        This function gets the mouse position and prints it to console
        Print is mainly for debuging purposes
     */
    public POINT GetMousePosition() {
        POINT p;
        if (GetCursorPos(out p))
        {
           // Console.WriteLine(Convert.ToString(p.X) + ";" + Convert.ToString(p.Y));
        }
        return p;
    }
    /*
        This checks to see if the cursor is within a specified range of the target coords
        within a certain margin
     */
    public Boolean CheckDistance(POINT userPos, POINT targetPos) {
        int margin = 150;
        if (userPos.X >= targetPos.X - margin && userPos.X <= targetPos.X + margin ) {
            if (userPos.Y >= targetPos.Y - margin && userPos.Y <= targetPos.Y + margin ) {
                
                return true;
            }
        }
        return false;
    }
    public Boolean CheckDistance(POINT userPos, POINT targetPos, int radius) {
        int dx = Math.Abs(userPos.X-targetPos.X);
        int dy = Math.Abs(userPos.Y- targetPos.Y);
        // Console.Write("X: "+dx+" Y: "+ dy);
        // Thread.Sleep(200);
        if (Math.Pow(dx,2) +Math.Pow(dy,2) <=Math.Pow(radius,2) ) {  
            PullCursorLog(userPos,targetPos,dx,dy);
            //PullCursor(userPos,targetPos);
            return true;
        }  
        else {
            return false;
        }
    }
    /*
        Instead of literally Pulling cursor linearly to the point
        apply logarithimc amounts of gravity to the cursor as it gets closer to the target
        on log plot, x is distance from target, y is speed
        the max distance is x = 14 units y = 8 speed 
     */
    public void PullCursorLog(POINT cursor, POINT target, double distX, double distY) {
        FloatPoint f = new FloatPoint(target.X-cursor.X,target.Y - cursor.Y);
        FloatPoint stepper = (FloatPoint) cursor;
        double r = Math.Sqrt(Math.Pow(distX,2) + Math.Pow(distY,2));
        //UInt32 newSpeed =  (UInt32)  (Math.Log(r));
        UInt32 newSpeed = (UInt32) (0.5 * Math.Sqrt(r)) - 2;
        Console.Write("\nRadius: "+ r+ " Speed: "+newSpeed);
        ChangeMouseSpeed(newSpeed);

    }
    /*
        This is hardcoded but will eventually get the targets position on screen from whatever OCR we use
     */
    public POINT GetTargetPostition() {
        return new POINT(600,300);
    }
    /*
        Speed must be between 0 and 20
        */
    public void ChangeMouseSpeed(UInt32 speed) {
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
    /*
    Pull cursor linearly
     */
    public void PullCursor(POINT cursor, POINT target) {
        FloatPoint f = new FloatPoint(target.X-cursor.X,target.Y - cursor.Y);
        FloatPoint stepper = (FloatPoint) cursor;
        FloatPoint[] array = new FloatPoint[20];
        f.X = f.X / 20;
        f.Y = f.Y / 20;
        for(int i = 0; i < 20; i ++) {
            stepper = new FloatPoint(stepper.X + f.X, stepper.Y + f.Y);
            SetCursorPos((int)(stepper.X),(int)(stepper.Y));
            Thread.Sleep(10);
            if((FloatPoint)target == stepper) {
                break;
            }
        }
    }
    
    public void Logic() {
        POINT p1 = GetMousePosition();
        POINT p2 = GetTargetPostition();
        if(!CheckDistance(p1, p2,200)) {
            ChangeMouseSpeed(8);
        } else {
            
        }
        Thread.Sleep(100);
    }
}  
}
