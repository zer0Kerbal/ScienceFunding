using System.IO;
using UnityEngine;

namespace ScienceFunding
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class ScienceFunding : MonoBehaviour
    {
        public float fundsMult = 1000;
        public float repMult = 1;

        public void Awake()
        {
            LoadConfiguration();
            GameEvents.OnScienceRecieved.Add(ScienceReceivedHandler);
            ScienceFunding.Log("listening for science...");
        }

        /// <summary>
        /// Science transmission handler: computes the funds and reputation boni
        /// and awards them to the player.
        /// </summary>
        public void ScienceReceivedHandler(float science, ScienceSubject sub, ProtoVessel v, bool whoKnows)
        {
            string msg = "Received " + science + " science points";
            ScienceFunding.Log(msg);

            float funds = science * this.fundsMult;
            float rep = science * this.repMult;

            if (Funding.Instance != null)
            {
                Funding.Instance.AddFunds(funds, TransactionReasons.ScienceTransmission);
                ScienceFunding.Log("Added " + funds + " funds");
            }

            if (Reputation.Instance != null)
            {
                Reputation.Instance.AddReputation(rep, TransactionReasons.ScienceTransmission);
                ScienceFunding.Log("Added " + rep + " reputation");
            }

            string msgContent = "Your research efforts in the field of " + sub.title + " have granted you " +
                                funds + " additional funds and increased your reputation by " + rep;

            PostMessage("More funds available!",
                        msgContent,
                        MessageSystemButton.MessageButtonColor.GREEN,
                        MessageSystemButton.ButtonIcons.COMPLETE); 
        }

        public void OnDestroy()
        {
            GameEvents.OnScienceRecieved.Remove(ScienceReceivedHandler);
            ScienceFunding.Log("OnDestroy, removing handler.");
        }

        #region Utilities

        public void LoadConfiguration()
        {
            ConfigNode node = GetConfig();
            this.fundsMult = float.Parse(node.GetValue("funds"));
            this.repMult = float.Parse(node.GetValue("rep"));
        }

        static ConfigNode GetConfig()
        {
            string assemblyPath = Path.GetDirectoryName(typeof(ScienceFunding).Assembly.Location);
            string filePath = Path.Combine(assemblyPath, "settings.cfg");

            ScienceFunding.Log("Loading settings file:" + filePath);

            ConfigNode result = ConfigNode.Load(filePath).GetNode("SCIENCE_FUNDING_SETTINGS");
            ScienceFunding.Log(result.ToString());

            return result;
        }

        static void PostMessage(string title,
                                string message,
                                MessageSystemButton.MessageButtonColor messageButtonColor,
                                MessageSystemButton.ButtonIcons buttonIcons)
        {
            MessageSystem.Message msg = new MessageSystem.Message(
                    title,
                    message,
                    messageButtonColor,
                    buttonIcons);
            MessageSystem.Instance.AddMessage(msg);
        }

        static void Log(string msg)
        {
            Debug.Log("[ScienceFunding]: " + msg);
        }   

        #endregion      
    }
}
