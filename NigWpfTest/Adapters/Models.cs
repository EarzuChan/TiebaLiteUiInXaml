using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Timer = System.Threading.Timer;

namespace NigWpfTest.Adapters
{
    public class TappingAdapter
    {
        private Timer InsidedTimer = null;
        public bool PrintOut = false;
        private double TimeCount = 0;
        public double LongTapping = 1.5;
        public Action DoShort = null;
        public Action DoLong = null;
        public TappingAdapter(UIElement TheControl)
        {
            if(PrintOut==true) Trace.WriteLine($"--->Message:\nMade up Tapping Adapter for {TheControl.ToString()}.\nMessgae<---");
            TheControl.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.TheControl_MouseLeftButtonDown), true);
            TheControl.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.TheControl_MouseLeftButtonUp), true);
            /*
                        TheControl.MouseLeftButtonDown += TheControl_MouseLeftButtonDown;
                        TheControl.MouseLeftButtonUp += TheControl_MouseLeftButtonUp;*/
        }

        private void TheControl_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (TimeCount < LongTapping) DoUp();
        }
        private void DoUp()
        {
            if (InsidedTimer == null) return;
            InsidedTimer.Change(-1, 0);
            InsidedTimer.Dispose();
            bool IsLong = TimeCount >= LongTapping;
            if (PrintOut == true) Trace.WriteLine("--->Message:\nTimer Stopped.\nTap Time: " + TimeCount.ToString() + "\nTap Type: " + (IsLong ? "Long Tap" : "Short Tap") + "\nMessgae<---");
            if (IsLong)
            {
                if (DoLong != null) DoLong();
            }
            else
            {
                if (DoShort != null) DoShort();
            }
        }
        private void TheControl_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TimeCount = 0;
            InsidedTimer = new(TimerCallBack, null, 0, 100);
            if (PrintOut == true) Trace.WriteLine("--->Message:\nTimer Started.\nMessgae<---");
        }
        public void TimerCallBack(object Obj)
        {
            TimeCount += 0.1;
            if (TimeCount >= LongTapping) DoUp();
        }
    }
}
