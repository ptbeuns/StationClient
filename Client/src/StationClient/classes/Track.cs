using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace StationClient
{
    public class Track
    {
        private List<I2CLamp> lamps;
        public int TrackId { get; private set; }
        public int RideNumber { get; set; }
        public TrackState TrackState {get; set;}
        private List<int> occupation;

        public Track(int trackId, List<I2CLamp> lamps)
        {
            TrackId = trackId;
            this.lamps = lamps;
            RideNumber = 0;
            occupation = new List<int>();
            TrackState = TrackState.NoOccupation;
        }

        public void UpdateLamps(List<int> newOccupation)
        {
            if (!occupation.SequenceEqual(newOccupation))
            {
                occupation = newOccupation;
                for (int i = 0; i < occupation.Count; i++)
                {
                    if (i < lamps.Count)
                    {
                        lamps[i].SetLampColor(newOccupation[i]);
                    }
                }
            }
        }
    }
}