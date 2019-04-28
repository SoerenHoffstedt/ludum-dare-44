using BarelyUI;
using BarelyUI.Layouts;
using BarelyUI.Styles;
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
            Size = new Point(600, 600);

            string helpFile = File.ReadAllText("Content/help.txt");

            Text t = new Text(helpFile, false);
            t.SetFont(Style.fonts["textSmall"]);
            t.wrapText = true;
            layout.AddChild(t);

            SetContentPanel(layout);

            Style.PopStyle("planetScreenContent");
            Layout.PopLayout("help");

            Close();
        }
    }
}
