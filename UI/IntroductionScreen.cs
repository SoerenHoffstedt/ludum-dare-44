using BarelyUI;
using BarelyUI.Layouts;
using BarelyUI.Styles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD44.UI
{
    public class IntroductionScreen : ModalWindow
    {
        string introText =
@"Oh, hello. I didn't even see you there.
Let me tell you a story about...yourself. You are the captain of a small crew of rebels. Your rebel group fights the DCA - the DOG CATCHER ASSOCIATION that terrorizes the doggo populations in your solar system. Booooo. The DCA is a strong military organization that patrols space regularly.

Today you attacked a DCA outpost and freed seven cute puppies. Several DCA ships are hunting you as a result. They hacked your bank account and you are not able to access your money. You must reach your Home planet. They can defend you from the DCA.

Your fuel is limited and you will have to trade parts of your ship for fuel to reach Home. Selling parts will make you slower and weaker. Selling too much of the wrong parts can mean death. Your ship has four types of parts: Scanning, Speed, Damage, and Defense parts.
Check the help screen for explanations.

Let's hope you can make it Home.
FOR THE DOGGOS!";

        public IntroductionScreen(Canvas canvas) : base("introTitle", canvas)
        {
            Style.PushStyle("planetScreenContent");
            Layout.PushLayout("introScreen");
            VerticalLayout layout = new VerticalLayout();
            layout.SetFixedSize(900, 600);            

            var mainText = new Text(introText);
            mainText.wrapText = true;            
            
            

            layout.AddChild(mainText);

            SetContentPanel(layout);

            Style.PopStyle("planetScreenContent");
            Layout.PopLayout("introScreen");
            Open();
        }
    }
}
