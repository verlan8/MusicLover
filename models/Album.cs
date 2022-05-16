using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MusicLover.models
{
    public class Album
    {
        public int albumId { get; set; }
        public string albumName { get; set; }

        public BitmapImage albumImage { get; set; }
    }
}
