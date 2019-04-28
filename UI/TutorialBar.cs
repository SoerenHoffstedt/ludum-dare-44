using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarelyUI;
using BarelyUI.Layouts;
using BarelyUI.Styles;
using LD44.Scenes;
using LD44.World;
using Microsoft.Xna.Framework;

namespace LD44.UI
{
    public class TutorialBar : VerticalLayout
    {        

        Text goalText;

        public TutorialBar(GameScene gameScene) : base()
        {

            Padding = new Point(6,6);
            goalText = new Text("goal");
            goalText.SetColor(Color.CadetBlue);
            //goalText.SetFont(Style.fonts["textNormal"]);
            AddChild(goalText, new Space(5));

            string[] text = { "lmouse", "rmouse", "wasd", "tab", "shift", "f", "h" };
            string[] sprite = { "lmouse", "rmouse", "wasd", "tab", "shift", "f", "h" };

            Layout.PushLayout("tutorialKeys");
            Style.PushStyle("tutorialKeys");

            for (int i = 0; i < text.Length; i++)
            {
                HorizontalLayout l = new HorizontalLayout();
                l.layoutFill = LayoutFill.StretchMargin;
                Image im = new Image(Assets.OtherSprites[sprite[i]]);
                im.SetFixedSize(32, 32);

                Text te = new Text(text[i]);                

                l.AddChild(im);
                l.AddChild(te);

                AddChild(l);
            }

            Layout.PopLayout("tutorialKeys");
            Style.PopStyle("tutorialKeys");
        }        

    }
}
