using BarelyUI;
using BarelyUI.Layouts;
using BarelyUI.Styles;
using LD44.Actors;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD44.UI
{
    public class HelpScreen : ModalWindow
    {
        public HelpScreen(Canvas canvas) : base("helpScreenTitle", canvas)
        {
            Style.PushStyle("planetScreenContent");
            Layout.PushLayout("help");
            VerticalLayout layout = new VerticalLayout();
            //layout.SetFixedSize(new Point(600, 600));

            string helpFile = File.ReadAllText("Content/help.txt");

            Text t = new Text(helpFile, false);
            t.SetFont(Style.fonts["textSmall"]);
            t.wrapText = true;
            layout.AddChild(t, new Space(10), new Text("Stat Icons:", false).SetColor(Color.CadetBlue).SetFont(Style.fonts["textNormal"]));
            
            string[] icons = { "healthIcon", "fuelIcon", "speedIcon", "damageIcon", "defenseIcon", "scanningIcon" };
            Stats[] stat = { Stats.Health, Stats.Fuel, Stats.Speed, Stats.Damage, Stats.Defense, Stats.Scanning };

            Layout.PushLayout("helpIcons");
            for (int i = 0; i < icons.Length; i++)
            {
                HorizontalLayout hor = new HorizontalLayout();
                hor.Padding = new Point(2, 2);
                Image im = new Image(Assets.OtherSprites[icons[i]]);
                Text text = new Text(stat[i].ToString());
                hor.AddChild(im, text);
                layout.AddChild(hor);
            }
            Layout.PopLayout("helpIcons");


            SetContentPanel(layout);

            Style.PopStyle("planetScreenContent");
            Layout.PopLayout("help");

            Close();
        }
    }
}
