using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ScienceFunding
{
    /*
     * Check that the current game contains the runtime ScenarioModule
     * and add it if it's missing.
     * 
     * Original code by TaranisElsu in TAC Life Support:
     * https://github.com/taraniselsu/TacLifeSupport
     */
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class ScienceFundingScenarioInjector : MonoBehaviour
    {
        void Start()
        {
            var game = HighLogic.CurrentGame;
            ProtoScenarioModule psm = game.scenarios.Find(s => s.moduleName == typeof(ScienceFunding).Name);

            if (psm == null)
            {
                ScienceFunding.Log("Adding the controller to the game.");
                psm = game.AddProtoScenarioModule(typeof(ScienceFunding), GameScenes.EDITOR,
                                                                          GameScenes.FLIGHT,
                                                                          GameScenes.SPACECENTER,
                                                                          GameScenes.TRACKSTATION);
            }
            else // make sure the scenario is targeting all the scenes
            {
                ScienceFunding.Log("The runtime is already installed (OK).");

                SetTargetScene(psm, GameScenes.EDITOR);
                SetTargetScene(psm, GameScenes.FLIGHT);
                SetTargetScene(psm, GameScenes.SPACECENTER);
                SetTargetScene(psm, GameScenes.TRACKSTATION);
            }
        }


        private static void SetTargetScene(ProtoScenarioModule psm, GameScenes scene)
        {
            if (!psm.targetScenes.Any(s => s == scene))
            {
                psm.targetScenes.Add(scene);
            }
        }

    }
}
