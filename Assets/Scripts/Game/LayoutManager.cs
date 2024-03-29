﻿using Assets.Models;
using Assets.Scripts.Helpers;
using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game
{    
    public class LayoutManager : MonoBehaviour
    {
        [Space]
        public Camera MainCamera;
        public GameObject DominoPrefab;
        public GameObject TopPanel;
        [Header("Animations")]
        public AnimationDefinition PlayerDominoSlideIn;
        public AnimationDefinition EngineSlideIn;
        public AnimationDefinition DominoSelection;
        public AnimationDefinition DominoDeselection;
        public AnimationDefinition DominoSlideToTrack;
        public AnimationDefinition DominoRotateToTrack;
        [Space]
        public Vector3 PlayerTopCenter = new Vector3(0, 0.08f, 0);
        public Vector3 PlayerBottomCenter = new Vector3(0, -0.08f, 0);
        public Vector3 TablePosition = new Vector3(0, 0, 0);
        [Range(0, 2)]
        public float BottomYOffset = 0.01f;
        public float BottomSideMargin = 0.01f;
        public float SelectionRaiseAmount = 0.02f;

        private float playerYPosition = 0;

        public void PlacePlayerDominoes(List<GameObject> playerDominoes)
        {
            PositionHelper.LayoutAcrossAndUnderScreen(playerDominoes, MainCamera, BottomSideMargin);  //place them outside of the camera's view to allow them to slide in

            var objectSize = PositionHelper.GetObjectDimensions(playerDominoes[0]);
            var positions = PositionHelper.GetLayoutAcrossScreen(objectSize, MainCamera, playerDominoes.Count, BottomSideMargin);

            playerYPosition = positions[0].y;

            for (int i = 0; i < playerDominoes.Count; i++)
            {
                var domino = playerDominoes[i];
                var mover = domino.GetComponent<Mover>();

                var staggerDelay = PlayerDominoSlideIn.Delay * i;

                StartCoroutine(mover.MoveOverSeconds(positions[i], PlayerDominoSlideIn.Duration, staggerDelay, PlayerDominoSlideIn.Curve));
            }
        }

        public Vector3 GetPlayerDominoLinePosition()
        {
            var screenSize = PositionHelper.GetScreenSize(MainCamera);
            var bottomCenter = new Vector3(screenSize.x, BottomYOffset, 0);

            return bottomCenter;
        }

        public void PlaceEngine(GameObject engine, Action afterComplete = null)
        {
            var destination = GetEnginePosition(engine);

            var mover = engine.GetComponent<Mover>();
            StartCoroutine(mover.MoveOverSeconds(destination, EngineSlideIn, afterComplete));
        }

        public Vector3 GetEnginePosition(GameObject engine)
        {
            var objectSize = PositionHelper.GetObjectDimensions(engine);
            return PositionHelper.GetScreenLeftCenterPositionForObject(objectSize, MainCamera, 0);
        }

        public void PlaceDominoOnTrack(GameObject domino, Vector3 enginePosition, int trackDominoIndex, Action afterComplete = null)
        {
            var mover = domino.GetComponent<Mover>();

            var destination = enginePosition + new Vector3(0.05f, 0, 0);

            // TODO: Move up to track y position then move over to the X
            // TODO: even better, spring past the x position and come back for a little sample of juice
            StartCoroutine(mover.RotateOverSeconds(Quaternion.Euler(0, 90, 90), DominoRotateToTrack));
            StartCoroutine(mover.MoveOverSeconds(destination, DominoSlideToTrack, afterComplete));   // TODO: new animation definition for adding domino to track
        }

        public void SelectDomino(GameObject domino)
        {
            var destination = new Vector3(domino.transform.position.x, playerYPosition + SelectionRaiseAmount, domino.transform.position.z);

            var mover = domino.GetComponent<Mover>();
            StartCoroutine(mover.MoveOverSeconds(destination, DominoSelection));
        }

        public void DeselectDomino(GameObject domino)
        {
            var destination = new Vector3(domino.transform.position.x, playerYPosition, domino.transform.position.z);

            var mover = domino.GetComponent<Mover>();
            StartCoroutine(mover.MoveOverSeconds(destination, DominoDeselection));
        }

        public void SetHeaderText(string message)
        {
            TopPanel.SetActive(true);
            TopPanel.GetComponent<TopText>().SetHeaderText(message);
        }

        /// <summary>
        /// Destroys all of the objects in the group and removes them from the group
        /// </summary>
        /// <param name="group">Group to clear</param>
        void DestroyGroup(ObjectGroup<GameObject> group)
        {
            var keys = group.GetKeys();
            for (int i = group.Count - 1; i >= 0; i--)
            {
                Destroy(group.GetObjectByKey(keys[i]));
                group.Remove(keys[i]);
            }
        }

        public void DisplayPlayersTurn(bool isPlayerTurn, bool isLocal, bool updateAll)
        {
            var isLocalTurn = false;

            if (isLocal)
            {
                isLocalTurn = true;
                if (isPlayerTurn)
                {
                    SetHeaderText($"It is your turn");
                }
                else
                {
                    SetHeaderText($"It is NOT your turn");
                }
            }

            if (updateAll)
            {
                if (!isLocalTurn)        // TODO: this works for 2 player but would not for more. How do we know who this client is? Instead maybe subscribe to an event in DominoPlayer so this runs for each player?
                {
                    SetHeaderText($"It is your turn");
                }
                else
                {
                    SetHeaderText($"It is NOT your turn");
                }
            }
        }

        //IEnumerator SlideStaggeredToYPosition(List<GameObject> gameObjects, float destinationYPosition, Action afterComplete = null)
        //{
        //    float animationDuration = 0.8f;
        //    float delayBeforeAnimation = 0.5f;
        //    float delayStagger = 0.04f;
        //    float totalAnimationTime = gameObjects.Count * delayStagger + delayBeforeAnimation + animationDuration;

        //    for (int i = 0; i < gameObjects.Count; i++)
        //    {
        //        var currentObj = gameObjects[i];
        //        Vector3 pos = new Vector3(currentObj.transform.position.x, destinationYPosition, 0);
        //        StartCoroutine(AnimationHelper.MoveOverSeconds(currentObj.transform, pos, animationDuration, i * delayStagger + delayBeforeAnimation, SelectionEase));
        //    }

        //    yield return new WaitForSeconds(totalAnimationTime);

        //    if (afterComplete != null)
        //    {
        //        afterComplete();
        //    }
        //}
    }
}
