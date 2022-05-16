using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicLover.models
{
    public class Charts
    {
        public int musicId { get; set; }
        public int chartPos { get; set; }
        public string musicArtist { get; set; }
        public string musicName { get; set; }
        public string musicDuration { get; set; }
    }
}
