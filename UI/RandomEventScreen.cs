using Barely.Util;
using BarelyUI;
using BarelyUI.Layouts;
using BarelyUI.Styles;
using LD44.Scenes;
using LD44.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LD44.World.RandomEvent;

namespace LD44.UI
{
    public class RandomEventScreen : ModalWindow
    {
        private const int MAX_BUTTON_COUNT = 4;
        RandomEventInfo info = null;

        Text eventText;
        Button[] buttons;

        GameScene scene;

        public RandomEventScreen(GameScene scene, Canvas canvas) : base(Texts.Get("randomEvent"), canvas)
        {
            this.scene = scene;
            DeactivateCloseButton();

            Style.PushStyle("planetScreenContent");
            Layout.PushLayout("randomEvent");

            Size = new Point(500, 500);

            VerticalLayout layout = new VerticalLayout();

            eventText = new Text("as\nasd\asdasdasdasdasdasdasdadsasdasd\nasdasdasdasdasdasd\nasdassadasds\nasdasd\nasdasd\nasdasdasd\nasdasdasdasdasd");
            eventText.wrapText = true;
            layout.AddChild(eventText);

            buttons = new Button[MAX_BUTTON_COUNT];

            for(int i = 0; i < MAX_BUTTON_COUNT; ++i)
            {
                Button b = new Button("this is a very long choice text, just in case, you know?");
                buttons[i] = b;
                layout.AddChild(b);
            }            

            SetContentPanel(layout);
            OnClose += () => info = null;
            Close();

            Style.PopStyle("planetScreenContent");
            Layout.PopLayout("randomEvent");
        }

        public void OpenFor(Tile planet)
        {
            Sounds.Play("openScreen");
            SetTitleText($"{Texts.Get("randomEvent")} on {planet.Name}");
            Open();

            info = (RandomEventInfo)planet.planetInfo;
            if (info.AlreadyExecuted)
            {
                UpdateTextsNothing();
            }
            else
            {
                info.AlreadyExecuted = true;
                UpdateTexts();                
            }
        }

        private void UpdateTextsNothing()
        {
            eventText.SetText("eventAlreadyDone");
            CloseAllButtons();
            buttons[3].Open();
            buttons[3].ChangeText("close");
            buttons[3].OnMouseClick = Close;
        }

        private void CloseAllButtons()
        {
            for (int i = 0; i < MAX_BUTTON_COUNT; i++)
            {
                buttons[i].Close();
            }
        }

        private void UpdateTexts()
        {
            eventText.SetText(info.RandomEvent.Text);
            CloseAllButtons();
            int i = MAX_BUTTON_COUNT - info.RandomEvent.Choices;

            foreach(Choice c in info.RandomEvent.AllChoices)
            {
                var b = buttons[i];
                b.Open();
                b.ChangeText(c.Text);
                Choice local = c;
                b.OnMouseClick = () => ChoiceSelected(local);
                ++i;
            }

        }

        private void ChoiceSelected(Choice c)
        {
            eventText.SetText(c.Result);
            CloseAllButtons();

            if(c.StatChanged != null && c.ChangeAmount != null)
            {
                var ship = scene.GetPlayerShip();
                for (int i = 0; i < c.StatChanged.Length; ++i)
                {
                    ship.ChangeStat(c.StatChanged[i], c.ChangeAmount[i]);
                }
            }
            else if (c.BattleBlueprintId != null)
            {
                //TODO: start battle
            }

            buttons[3].Open();
            buttons[3].ChangeText("close");
            buttons[3].OnMouseClick = Close;
        }

    }
}
