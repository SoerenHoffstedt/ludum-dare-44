using LD44.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LD44.World
{
    /// <summary>
    /// Data defining a random event.
    /// </summary>
    public class RandomEvent
    {
        public string Id;
        public string Text;
        public int Choices;
        public Choice[] AllChoices;

        public RandomEvent(XmlNode node)
        {
            Id = node.Attributes["id"].Value;
            Text = node.Attributes["text"].Value;
            
            var choiceList = node.SelectNodes("choice");
            AllChoices = new Choice[choiceList.Count];
            if (choiceList.Count > 4)
                throw new ArgumentException("A random event must not have more than 4 choices.");

            for (int i = 0; i < choiceList.Count; i++)
            {
                AllChoices[i] = new Choice(choiceList[i]);
            }

            Choices = AllChoices.Length;

        }

        /// <summary>
        /// Data representing a selecteable choice in a random event.
        /// </summary>
        public class Choice
        {
            public string Text;
            public string Result;
            public Stats[] StatChanged; 
            public int[] ChangeAmount;
            public string BattleBlueprintId;
            public string soundId;

            public Choice(XmlNode node)
            {
                Text = node.Attributes["text"].Value;
                Result = node.Attributes["result"].Value;
                if (node.Attributes["sound"] != null)
                    soundId = node.Attributes["sound"].Value;

                XmlNodeList gainNodes = node.SelectNodes("gain");
                if(gainNodes.Count > 0)
                {
                    StatChanged  = new Stats[gainNodes.Count];
                    ChangeAmount = new int[gainNodes.Count];
                    for (int i = 0; i < gainNodes.Count; i++)
                    {
                        StatChanged[i]  = (Stats)Enum.Parse(typeof(Stats), gainNodes[i].Attributes["stat"].Value);
                        ChangeAmount[i] = int.Parse(gainNodes[i].Attributes["amount"].Value);
                    }
                }                

                if (node.Attributes["battle"] != null)
                    BattleBlueprintId = node.Attributes["battle"].Value;


            }
        }
    }
}
