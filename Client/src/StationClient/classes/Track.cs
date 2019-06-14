using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace StationClient
{
    public class Track
    {
        private List<LedLamp> lamps;
        public int TrackId { get; private set; }
        public int RideNumber { get; set; }
        public TrackState TrackState {get; set;}

        public long LastUpdate {get; set;}
        private List<int> occupation;

        public Track(int trackId, List<LedLamp> lamps)
        {
            TrackId = trackId;
            this.lamps = lamps;
            RideNumber = 0;
            occupation = new List<int>();
            TrackState = TrackState.NoOccupation;
        }

        public void UpdateLamps(List<int> newOccupation)
        {
            Console.WriteLine("Updating lamps");
            Console.WriteLine(newOccupation);
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