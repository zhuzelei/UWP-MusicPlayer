using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace MusicList_test
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AddSong : Page
    {
        public AddSong()
        {
            this.InitializeComponent();
        }

        ViewModel.SongVM Songs = ViewModel.SongVM.Single();

        private void CreateSong(object sender, RoutedEventArgs e)
        {
            if (Name.Text == "" && Singer.Text == "")
            {
                var j = new MessageDialog("歌曲名不能为空").ShowAsync();
            }
            else if (Singer.Text == "" && Name.Text != "")
            {
                var j = new MessageDialog("歌手不能为空").ShowAsync();
            }
            else if (Name.Text == "" && Singer.Text != "")
            {
                var j = new MessageDialog("歌曲名不能为空").ShowAsync();
            }
            else
            {
                if(Path.Text == "")
                {
                    var i = new MessageDialog("歌曲链接不能为空").ShowAsync();
                }
                else
                {
                    Songs.AddSong(Name.Text, Singer.Text, Path.Text);
                    var j = new MessageDialog("创建成功").ShowAsync();
                    Frame.Navigate(typeof(MainPage),Songs);
                    //update(sender, e);
                }
                
            }
            var db = App.conn;
            try
            {
                using (var custstmt = db.Prepare("INSERT INTO Customer (Name, Singer, Path) VALUES (?, ?, ?)"))
                {
                    custstmt.Bind(1, Name.Text);
                    custstmt.Bind(2, Singer.Text);
                    custstmt.Bind(3, Path.Text);
                    custstmt.Step();
                }
            }
            catch (Exception ex)
            {

                var i = new MessageDialog(ex.ToString()).ShowAsync();
                // TODO: Handle error
            }
        }

        private void BackIntro(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(IntroPage));
        }
    }
    
}
