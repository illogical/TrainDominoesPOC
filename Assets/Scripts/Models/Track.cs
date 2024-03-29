﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class Track
    {
        public int? PlayerId { get; set; }  // each player gets only 1 track. When null then anyone can use this track
        public bool HasTrain { get; set; }  // TODO: needs set to true when a player is unable to play a domino
        public List<int> DominoIds { get; set; }

        public Track(int dominoId, int? owner = null)
        {
            DominoIds = new List<int>();
            DominoIds.Add(dominoId);

            PlayerId = owner;
        }

        public void AddDominoToTrack(int dominoId)
        {
            DominoIds.Add(dominoId);
        }

        public bool ContainsDomino(int dominoId) => DominoIds.Contains(dominoId);

        public int GetEndDominoId() => DominoIds[DominoIds.Count - 1];
    }
}
