using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Media;

namespace Amadues
{
    class BackgroundMusicTrack
    {
        public String title;
        public String artist;
        public Song track;
        
        public BackgroundMusicTrack()
        {
            title = "";
            artist = "";
        }

        public BackgroundMusicTrack(String aTitle, String aArtist, Song aSong)
        {
            title = aTitle;
            artist = aArtist;
            track = aSong;
        }
    }
}
