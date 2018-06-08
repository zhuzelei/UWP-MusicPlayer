using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicList_test.Model
{
    public class Song
    {
        //每个Song的所属编号，外界不可更改
        public int id { get;}
        
        //歌名
        public string name { get; set; }
        
        //歌手
        public string singer { get; set; }

        //链接
        public string path { get; set; }
        
        //构造函数
        public Song(string _name, string _singer ,string _path)
        {
            name = _name;
            path = _path;
            singer = _singer;
            id = getNewId();
        }

        //添加新歌曲后id++
        private static int current_id = 0;
        private static int getNewId()
        {
            return (current_id++);
        }
    }
}
