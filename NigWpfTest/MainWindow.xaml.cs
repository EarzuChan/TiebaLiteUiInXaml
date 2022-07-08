using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NigWpfTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public int Times = 0;
        public double OutOfWindowsLength = 0;
        private Utils.MiniTiebaApiUtil MTAU;
        public string SelectedBarName = "";
        List<ForumList_ForumUnit> ForumUnits = new();
        List<ForumList_ForumUnit> TopForumUnits = new();
        public MainWindow()
        {
            InitializeComponent();
            initView();
        }
        private void DoScroll(double value)
        {
            //Trace.WriteLine($"{MainContent.RowDefinitions[0].ActualHeight}   {value}");
            if (-value < MainContent.RowDefinitions[0].ActualHeight)
            {
                MainContent.Margin = new(0, value, 0, 0);
                RealContent.Margin = new(0);
                double MG = -60 - value;
                MG = MG < -30 ? -30 : MG;
                SignAll_B.Visibility = MG < -30 ? Visibility.Hidden : Visibility.Visible;
                Trace.WriteLine($"丁真 {MG}");
                MTBC.Margin = new(14, 14, MG, 14);
            }
            else
            {
                MainContent.Margin = new(0, -MainContent.RowDefinitions[0].ActualHeight, 0, 0);
                RealContent.Margin = new(0, value + MainContent.RowDefinitions[0].ActualHeight, 0, 0);
            }
        }
        private bool GetIfIsTop(string BarName)
        {
            bool IsTop = false;
            Dispatcher.Invoke(() =>
            {

                foreach (var Unit in TopForumUnits)
                {
                    if (Unit.Title == BarName)
                    {
                        IsTop = true;
                        break;
                    };
                }
            });


            return IsTop;
        }
        private void RefreshOOWL(bool DE)
        {
            if (DE == true) DoEvents();
            double ClientHeight = MainContent.ActualHeight;
            //Trace.WriteLine($"\n{MainContent.RowDefinitions[0].ActualHeight} + {MainContent.RowDefinitions[1].ActualHeight} + {RealContent.ActualHeight}\n");
            double ContentHeight = MainContent.RowDefinitions[0].ActualHeight + MainContent.RowDefinitions[1].ActualHeight + RealContent.ActualHeight;
            double CuttedHeight = ContentHeight - ClientHeight;
            if (CuttedHeight <= 0) CuttedHeight = 0;
            MyScroll.Visibility = CuttedHeight == 0 ? Visibility.Collapsed : Visibility.Visible;
            OutOfWindowsLength = CuttedHeight;
            //Trace.WriteLine($"\n客户区高度：{ClientHeight}\n内容真实高度：{ContentHeight}\n剪切高度：{CuttedHeight}");
            DoScroll(-(0.01 * CuttedHeight * MyScroll.Value));
        }
        public async void initView()
        {
            TopCard.Visibility = Visibility.Collapsed;
            MTAU = new();
            /*
             添加流程：
            var一个new布局
            判断左中右
            加到单元合集
            刷新数值
            刷新oowl
             */
#if DEBUG
            int i = 0;
            for (i = 1; i < 12;)
            {
                var A = new ForumList_ForumUnit("理塘真", i, (new Random().Next(0, 2)) >= 1 ? true : false, () =>
                {
                    Process.Start(@"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe", "http://tieba.baidu.com/f?kw=" + HttpUtility.UrlEncode("丁真"));
                }, () =>
                {
                    ShowForumMenu("理塘真", GetIfIsTop("理塘真"));
                });
                if (i != 11 || i % 2 == 0)
                {
                    if (i % 2 != 0)
                    {
                        LeftFollowBox.Children.Add(A);
                    }
                    else
                    {
                        RightFollowBox.Children.Add(A);
                    }
                }
                else
                {
                    SingleFollowBox.Children.Add(A);
                }
                ForumUnits.Add(A);
                i++;
            }
            i--;
            Times = i;
            Trace.WriteLine($"Cnm{i}");
            RefreshOOWL(true);
            return;
#endif
            await MTAU.GetTiebaList(() =>
            {
                foreach (Utils.MiniTiebaApiUtil.MyFR.LikeForum Forum in MTAU.TBData)
                {
                    Times++;
                    /* (object obj, RoutedEventArgs arg) =>
                    {
                        Process.Start(@"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe", "http://tieba.baidu.com/f?kw=" + HttpUtility.UrlEncode(Forum.forum_name));
                    }*/
                    var A = new ForumList_ForumUnit(Forum.forum_name, int.Parse(Forum.level_id), Forum.is_sign == "1" ? true : false, () =>
                    {
                        Process.Start(@"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe", "http://tieba.baidu.com/f?kw=" + HttpUtility.UrlEncode(Forum.forum_name));
                    }, () =>
                    {
                        ShowForumMenu(Forum.forum_name, GetIfIsTop(Forum.forum_name));
                    });
                    if (Times != MTAU.TBData.Count || Times % 2 == 0)
                    {
                        if (Times % 2 != 0)
                        {
                            LeftFollowBox.Children.Add(A);
                        }
                        else
                        {
                            RightFollowBox.Children.Add(A);
                        }
                    }
                    else
                    {
                        SingleFollowBox.Children.Add(A);
                    }
                    ForumUnits.Add(A);
                }
                RefreshOOWL(true);
            });
        }
        private static DispatcherOperationCallback exitFrameCallback = new DispatcherOperationCallback(ExitFrame);
        public static void DoEvents()
        {
            DispatcherFrame nestedFrame = new DispatcherFrame();
            DispatcherOperation exitOperation = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, exitFrameCallback, nestedFrame);
            Dispatcher.PushFrame(nestedFrame);
            if (exitOperation.Status !=
            DispatcherOperationStatus.Completed)
            {
                exitOperation.Abort();
            }
        }
        private static Object ExitFrame(Object state)
        {
            DispatcherFrame frame = state as
            DispatcherFrame;
            frame.Continue = false;
            return null;
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool LastIsTheSingleOne = Times % 2 != 0 ? true : false;
                Times++;
                if (LastIsTheSingleOne)
                {
                    SingleFollowBox.Children.Clear();
                    LeftFollowBox.Children.Add(ForumUnits[ForumUnits.Count - 1]);
                }
                string BN = $"看看你的{Times}";
                ForumList_ForumUnit Sxc = new(BN, Times, true, () =>
                {
                    Trace.WriteLine("你点得好啊");
                }, () =>
                {
                    ShowForumMenu(BN, GetIfIsTop(BN));
                });
                if (Times % 2 != 0)
                {
                    SingleFollowBox.Children.Add(Sxc);
                }
                else
                {
                    RightFollowBox.Children.Add(Sxc);
                }
                ForumUnits.Add(Sxc);
                RefreshOOWL(true);
            }
            catch (Exception Ex)
            {
                Trace.WriteLine($"Error:\n{Ex.ToString()}\nMessage:\n{Ex.Message}");
            }
        }
        public void ShowForumMenu(string BarName, bool IsNamedTop)
        {

            Dispatcher.Invoke(() =>
            {
                ToTopButton.Content = IsNamedTop == true ? "取消置顶" : "置顶";
                SelectedBarName = BarName;
                Point p = Mouse.GetPosition(this as FrameworkElement);
                ForumBox.Margin = new(p.X, p.Y, 0, 0);
                ForumBox.IsPopupOpen = true;
                ForumBox.StaysOpen = false;
            });

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetDataObject(SelectedBarName);
        }

        private void SearchButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Trace.WriteLine("\n看看你的\n？！？！\n");
        }

        private void ScrollBar_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            double CuttedMove = (sender as System.Windows.Controls.Primitives.ScrollBar).Value * (OutOfWindowsLength / 100);
            Trace.WriteLine($"Scroll {e.NewValue}, About {CuttedMove}");
            DoScroll(-CuttedMove);
        }


        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshOOWL(false);
        }

        private void ToTopButton_Click(object sender, RoutedEventArgs e)
        {
            if ((string)((sender as Button).Content) == "置顶")
            {
                AddToTop(SelectedBarName);
            }
            else
            {
                RemoveFromTop(SelectedBarName);
            }
        }
        private void AddToTop(string BarName)
        {
            try
            {
                foreach (var YGYG in ForumUnits)
                {
                    if (YGYG.Title == BarName)
                    {
                        bool LastIsTheSingleOne = TopForumUnits.Count % 2 != 0 ? true : false;
                        int Nums = TopForumUnits.Count + 1;
                        if (LastIsTheSingleOne)
                        {
                            SingleTopBox.Children.Clear();
                            LeftTopBox.Children.Add(TopForumUnits[TopForumUnits.Count - 1]);
                        }
                        string tt = YGYG.Title;
                        ForumList_ForumUnit Sxc = new(tt, YGYG.Level, YGYG.IsSigned, () =>
                        {
                            Process.Start(@"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe", "http://tieba.baidu.com/f?kw=" + HttpUtility.UrlEncode(YGYG.Name));
                        }, () =>
                        {
                            ShowForumMenu(tt, GetIfIsTop(tt));
                        });
                        if (Nums % 2 != 0)
                        {
                            SingleTopBox.Children.Add(Sxc);
                        }
                        else
                        {
                            RightTopBox.Children.Add(Sxc);
                        }
                        TopForumUnits.Add(Sxc);
                        RefreshOOWL(true);
                        break;
                    }
                }
                TopCard.Visibility = TopForumUnits.Count != 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error:\n{ex.ToString()}\nMessage:\n{ex.Message}");
            }
        }
        private void RemoveFromTop(string BarName)
        {
            try
            {
                foreach (var YGYG in TopForumUnits)
                {
                    if (YGYG.Title == BarName)
                    {
                        (YGYG.Parent as StackPanel).Children.Remove(YGYG);
                        TopForumUnits.Remove(YGYG);

                        int NNum = 1;
                        foreach (var a in TopForumUnits)
                        {

                            (a.Parent as StackPanel).Children.Remove(a);
                            if (NNum != TopForumUnits.Count || NNum % 2 == 0)
                            {
                                if (NNum % 2 != 0)
                                {
                                    LeftTopBox.Children.Add(a);
                                }
                                else
                                {
                                    RightTopBox.Children.Add(a);
                                }
                            }
                            else
                            {
                                SingleTopBox.Children.Add(a);

                            }
                            NNum++;
                        }
                        break;
                    }
                }
                TopCard.Visibility = TopForumUnits.Count != 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error:\n{ex.ToString()}\nMessage:\n{ex.Message}");
            }

        }
        private void RemoveFromFollow(string BarName)
        {
            try
            {
                foreach (var YGYG in ForumUnits)
                {
                    if (YGYG.Title == BarName)
                    {
                        (YGYG.Parent as StackPanel).Children.Remove(YGYG);
                        ForumUnits.Remove(YGYG);

                        int NNum = 1;
                        foreach (var a in ForumUnits)
                        {

                            (a.Parent as StackPanel).Children.Remove(a);
                            if (NNum != ForumUnits.Count || NNum % 2 == 0)
                            {
                                if (NNum % 2 != 0)
                                {
                                    LeftFollowBox.Children.Add(a);
                                }
                                else
                                {
                                    RightFollowBox.Children.Add(a);
                                }
                            }
                            else
                            {
                                SingleFollowBox.Children.Add(a);

                            }
                            NNum++;
                        }
                        break;
                    }
                }
                RemoveFromTop(BarName);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error:\n{ex.ToString()}\nMessage:\n{ex.Message}");
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            RemoveFromFollow(SelectedBarName);
        }
    }
}
