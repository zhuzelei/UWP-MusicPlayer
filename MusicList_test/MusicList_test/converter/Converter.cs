using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace MusicList_test.Converter
{
    /*时间条转换器*/
    class ScheduleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((TimeSpan)value).TotalSeconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return TimeSpan.FromSeconds((double)value);
        }
    }

    /*音量条转换器*/
    class VolumeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double temp_value = (double)value / 100;
            return temp_value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            double temp_value = (double)value * 100;
            return temp_value;
        }
    }

  

}
