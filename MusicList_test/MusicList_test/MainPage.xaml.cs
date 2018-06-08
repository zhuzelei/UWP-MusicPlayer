using MusicList_test.ViewModel;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace MusicList_test
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested += ShareRequested;
            
            //Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Collapsed;
        }


        ViewModel.SongVM songList = SongVM.Single();
        int index;
        int IsPlaying = 0;

        private void SongClick(object sender, ItemClickEventArgs e)
        {

            songList.SelectedItem = (Model.Song)(e.ClickedItem);
            //动态调整没写
            Name.Text = songList.SelectedItem.name;
            Singer.Text = songList.SelectedItem.singer;
            Path.Text = songList.SelectedItem.path;
            
            
            try
            {
                Uri pathUri = new Uri(songList.SelectedItem.path);
                mediaPlayer.Source = pathUri;
                index = songList.SelectedItem.id;
                mediaPlayer.Play();
                IsPlaying = 1;
                PlayPauseButton.Icon = new SymbolIcon(Symbol.Pause);
                PlayPauseButton.Label = "Pause";
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    System.Diagnostics.Debug.WriteLine("MusicError");
                    // handle exception. 
                    // For example: Log error or notify user problem with file
                }
            }
        }

        private void GoCreate(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddSong));
        }

        private void Element_MediaOpened(object sender, RoutedEventArgs e)
        {
            TimeLine.Value = 0;
            TimeLine.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
        }

        private void Element_MediaEnded(object sender, RoutedEventArgs e)
        {
            
            if(index >= songList.AllItems.Count-1)
            {
                index = 0;
            }
            else
            {
                index++;
            }
            //更改当前选择歌曲
            SongList.SelectedItem = songList.AllItems[index];
            songList.SelectedItem = songList.AllItems[index];
            Uri SongUri = new Uri(songList.SelectedItem.path);
            mediaPlayer.Source = SongUri;
            mediaPlayer.Play();
            TimeLine.Value = 0;
            TimeLine.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;


            Name.Text = songList.SelectedItem.name;
            Singer.Text = songList.SelectedItem.singer;
            Path.Text = songList.SelectedItem.path;

            //实现自动播放下一曲
        }

        private void ChangeMediaVolume(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Volume = (double)VolumeLine.Value / 100.0;
        }

        private void PlayClick(object sender, RoutedEventArgs e)
        {
            if(songList.SelectedItem == null)
            {
                return;
            }
            
            if(IsPlaying == 0)
            {
                mediaPlayer.Play();
                IsPlaying = 1;
                PlayPauseButton.Icon = new SymbolIcon(Symbol.Pause);
                PlayPauseButton.Label = "Pause";
            }
            else
            {
                mediaPlayer.Pause();
                IsPlaying = 0;
                PlayPauseButton.Icon = new SymbolIcon(Symbol.Play);
                PlayPauseButton.Label = "Play";
            }
        }

        private void NextSong(object sender, RoutedEventArgs e)
        {
            Element_MediaEnded(sender, e);
        }

        private void PrevSong(object sender, RoutedEventArgs e)
        {
            if (index == 0)
            {
                index = songList.AllItems.Count - 1;
            }
            else
            {
                index--;
            }
            //更改当前选择歌曲
            SongList.SelectedItem = songList.AllItems[index];
            songList.SelectedItem = songList.AllItems[index];
            Uri SongUri = new Uri(songList.SelectedItem.path);
            mediaPlayer.Source = SongUri;
            mediaPlayer.Play();
            TimeLine.Value = 0;
            TimeLine.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;

            Name.Text = songList.SelectedItem.name;
            Singer.Text = songList.SelectedItem.singer;
            Path.Text = songList.SelectedItem.path;
        }

        private void DeleteSong(object sender, RoutedEventArgs e)
        {
            using (var statement = App.conn.Prepare("DELETE FROM Customer WHERE Id = ?"))
            {
                statement.Bind(1, GetId());
                statement.Step();
            }
            songList.RemoveSong();
            var j = new MessageDialog("删除成功!").ShowAsync();
            //update(sender, e);
            Name.Text = "";
            Singer.Text = "";
            Path.Text = "";
        }

        private int GetId()
        {
            string query = "%%";
            int id = 0;
            string now_name = songList.SelectedItem.name;
            string now_singer = songList.SelectedItem.singer;
            using (var statement = App.conn.Prepare("SELECT Id, Name, Singer FROM Customer WHERE Id LIKE ? OR Name LIKE ? OR Singer LIKE ? "))
            {
                statement.Bind(1, query);
                statement.Bind(2, query);
                statement.Bind(3, query);

                while (statement.Step() != SQLiteResult.DONE)
                {
                    string did = statement[0].ToString();
                    string dname = statement[1].ToString();
                    string dsinger = statement[2].ToString();
                    if (now_name == dname &&
                        now_singer == dsinger)
                    {
                        id = int.Parse(did);
                        break;
                    }
                }
            }
            return id;
        }

        private void UpdateSong(object sender, RoutedEventArgs e)
        {
            if (songList.SelectedItem != null)
            {
                using (var custstmt = App.conn.Prepare("UPDATE Customer SET Name = ?, Singer = ?, Path = ? WHERE Id = ?"))
                {
                    custstmt.Bind(1, Name.Text);
                    custstmt.Bind(2, Singer.Text);
                    custstmt.Bind(3, Path.Text);
                    custstmt.Bind(4, GetId());
                    custstmt.Step();
                }
                songList.UpdateSong(songList.SelectedItem.id, Name.Text, Singer.Text, Path.Text);
                var j = new MessageDialog("更新成功!").ShowAsync();
                //磁贴更新update(sender, e);
                 Frame.Navigate(typeof(MainPage), songList);
            }
        }

        //
        //share song part
        private void ShareSong(object sender, RoutedEventArgs e)
        {
            if (songList.SelectedItem != null)
            {
                DataTransferManager.ShowShareUI();
            }
            else
            {
                var j = new MessageDialog("请选择item!").ShowAsync();
            }
        }
        async void ShareRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {

            DataRequest request = args.Request;
            DataRequestDeferral getFile = args.Request.GetDeferral();
            if (songList.SelectedItem != null)
            {
                request.Data.Properties.Title = "给你分享一首好歌：" + songList.SelectedItem.name;
                request.Data.SetText("歌手是" + songList.SelectedItem.singer + "\n" + "复制链接到浏览器收听" + songList.SelectedItem.path);
            }
            else
            {
                request.Data.Properties.Title = Name.Text;
                request.Data.SetText(Singer.Text);
            }
            
            try
            {   
                //图片分享未完成
                //StorageFile image_File = await StorageFile.GetFileFromApplicationUriAsync(new Uri("Assets/音符.png"));
                //request.Data.Properties.Thumbnail = RandomAccessStreamReference.CreateFromFile(image_File);
                //request.Data.SetBitmap(RandomAccessStreamReference.CreateFromFile(image_File));
            }
            finally
            {
                getFile.Complete();
            }
        }

        private void FindSong(object sender, RoutedEventArgs e)
        {
            string q = "%" + Query.Text + "%";
            using (var statement = App.conn.Prepare("SELECT Name, Singer, Path FROM Customer WHERE Name LIKE ? OR Singer LIKE ? OR Path LIKE ?"))
            {
                statement.Bind(1, q);
                statement.Bind(2, q);
                statement.Bind(3, q);
                string os = "";
                while (statement.Step() != SQLiteResult.DONE)
                {
                    string dname = statement[0].ToString();
                    string dsinger = statement[1].ToString();
                    string dpath = statement[2].ToString();
                    os += " Name: " + dname + ";    Singer: " + dsinger + ";    Path: " + dpath + "\n\n";
                }
                if (os == "")
                {
                    os = "Sorry, this Song is not in your MusicList !";
                }

                var i = new MessageDialog(os).ShowAsync();

            }
        }

        private void BackIntro(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(IntroPage));
        }
    }
}
