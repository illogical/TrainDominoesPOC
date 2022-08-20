﻿using UnityEngine;

namespace Assets.Scripts.Game.States
{
    public class PlayerTurnStartedState : GameStateBase
    {
        public int? SelectedDominoId;
        public bool IsTrackDominoSelected;

        // TODO: how do we know if this player has a track yet?

        public override void EnterState(GameStateContext gameStateContext)
        {
            // add player dominoes
            Debug.Log("STATE: PlayerTurnStartedState");

            SelectedDominoId = null;
            IsTrackDominoSelected = false;

            //gameStateContext.GameplayManager.OnEventRaised.AddListener(SelectDomino);
        }

        public override void UpdateState(GameStateContext gameStateContext)
        {
            if (!SelectedDominoId.HasValue)
            {
                return;
            }

            //var playerSelectedDominoID = gameStateContext.GameplayManager.GetSelectedDominoID();
            //if (!playerSelectedDominoID.HasValue)
            //{
            //    gameStateContext.GameplayManager.SelectDomino(SelectedDominoId.Value);
            //    gameStateContext.SwitchState(gameStateContext.PlayerSelectedPlayerDominoState);
            //    return;
            //}

        }

        public override void LeaveState(GameStateContext gameStateContext)
        {
            //gameStateContext.GameplayManager.DominoSelected.OnEventRaised.RemoveListener(SelectDomino);
        }

        public void SelectDomino(int dominoId)
        {
            SelectedDominoId = dominoId;
        }

    }
}