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
            Choices = int.Parse(node.Attributes["choices"].Value);

            var choiceList = node.SelectNodes("choice");
            AllChoices = new Choice[choiceList.Count];
            if (choiceList.Count > 4)
                throw new ArgumentException("A random event must not have more than 4 choices.");

            for (int i = 0; i < choiceList.Count; i++)
            {
                AllChoices[i] = new Choice(choiceList[i]);
            }

        }

        /// <summary>
        /// Data representing a selecteable choice in a random event.
        /// </summary>
        public class Choice
        {
            public string Text;
            public string Result;
            public Stats StatChanged; 
            public int ChangeAmount;
            public string BattleBlueprintId;

            public Choice(XmlNode node)
            {
                Text = node.Attributes["text"].Value;
                Result = node.Attributes["result"].Value;
                if (node.Attributes["stat"] != null)
                {
                    StatChanged     = (Stats)Enum.Parse(typeof(Stats), node.Attributes["stat"].Value);
                    ChangeAmount    = int.Parse(node.Attributes["amount"].Value);
                } else                
                    StatChanged = Stats.Count;

                if (node.Attributes["battle"] != null)
                    BattleBlueprintId = node.Attributes["battle"].Value;


            }
        }
    }
}
