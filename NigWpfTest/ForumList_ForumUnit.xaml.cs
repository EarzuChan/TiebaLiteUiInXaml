using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NigWpfTest
{
    /// <summary>
    /// ForumList_ForumUnit.xaml 的交互逻辑
    /// </summary>
    public partial class ForumList_ForumUnit : UserControl
    {
        private Action LTEvent = null;
        private Adapters.TappingAdapter MyAdapter = null;
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(ForumList_ForumUnit));

        public int Level
        {
            get { return (int)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }
        public static readonly DependencyProperty LevelProperty = DependencyProperty.Register("Level", typeof(int), typeof(ForumList_ForumUnit));

        public bool IsSigned
        {
            get { return (bool)GetValue(IsSignedProperty); }
            set { SetValue(IsSignedProperty, value); }
        }
        public static readonly DependencyProperty IsSignedProperty = DependencyProperty.Register("IsSigned", typeof(bool), typeof(ForumList_ForumUnit));

        public ForumList_ForumUnit()
        {
            initView(null, null, null, null, null);
        }
        private void initView(string? Ti, int? Le, bool? Is, Action? LeftShortTapEvent, Action? LeftLongTapEvent) {
            Title = Ti != null ? Ti : "未设定吧名";
            Level = (int)(Le != null ? Le : 114514);
            IsSigned = (bool)(Is != null ? Is : true);
            InitializeComponent();
            MyAdapter = new(BasedBox);
            MyAdapter.LongTapping = 0.8;
            if (LeftShortTapEvent != null) SetLeftShortTapEvent(LeftShortTapEvent);
            if (LeftLongTapEvent != null) SetLeftLongTapEvent(LeftLongTapEvent);
            RefreshUi();
        }
        public ForumList_ForumUnit(string? Ti, int? Le, bool? Is, Action? LeftShortTapEvent, Action? LeftLongTapEvent)
        {
            initView(Ti, Le, Is, LeftShortTapEvent, LeftLongTapEvent);
        }

        private void BasedBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        /*public ForumList_ForumUnit(string? Ti, int? Le, bool? Is)
        {
            Title = Ti != null ? Ti : "未设定吧名";
            Level = (int)(Le != null ? Le : 114514);
            IsSigned = (bool)(Is != null ? Is : true);
            InitializeComponent();
            RefreshUi();
        }*/

        public void SetLeftShortTapEvent(Action Event)
        {
            MyAdapter.DoShort = Event;
        }
        public void SetLeftLongTapEvent(Action Event)
        {
            MyAdapter.DoLong = Event;
        }
        public void RefreshUi()
        {
            BarNameText.Text = Title;
            var color = System.Windows.Media.Color.FromArgb(255, 77, 173, 161);
            if (Level < 5) LevelBox.Background = new SolidColorBrush(color);
            string TextOfLevelBox = IsSigned ? $"{Level}  已签" : Level.ToString();
            LevelTextBlock.Text = TextOfLevelBox;
        }
    }
}
