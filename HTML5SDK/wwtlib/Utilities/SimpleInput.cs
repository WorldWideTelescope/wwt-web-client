using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Html.Services;
using System.Html.Media.Graphics;


namespace wwtlib
{
    public class SimpleInput
    {

        public SimpleInput(string title, string label, string text, int v3)
        {
            Title = title;
            Label = label;
            Text = text;
        }

        public DialogResult ShowDialog()
        {

            return DialogResult.OK;
        }

        public string Title = "Tile";
        public string Label = "Enter Text Below";
        public string Text = "";

        private Action okCallback;
        public void NonMenuClick(ElementEvent e)
        {
            if (!ignoreNextClick)
            {
                Close();
            }
            ignoreNextClick = false;
        }

        InputElement textElement = null;

        public void Show(Vector2d position, Action callback)
        {
            DivElement simpleInputElement = Document.GetElementById<DivElement>("simpleinput");
            DivElement modalElement = Document.GetElementById<DivElement>("simplemodal");
            modalElement.Style.Display = "block";
            simpleInputElement.Style.Display = "block";
            simpleInputElement.Style.MarginLeft = position.X.ToString() + "px";
            simpleInputElement.Style.MarginTop = position.Y.ToString() + "px";

            textElement = Document.GetElementById<SelectElement>("inputtext");
            textElement.Value = Text;

            DivElement titleDiv = Document.GetElementById<DivElement>("simpletitle");
            DivElement labelDiv = Document.GetElementById<DivElement>("inputlabel");
            titleDiv.InnerText = Title;
            labelDiv.InnerText = Label;



            textElement.AddEventListener("change", TextChanged, false);
            textElement.AddEventListener("click", IgnoreMe, true);

            //Window.AddEventListener("click", NonMenuClick, true);

            AnchorElement okButton = Document.GetElementById<AnchorElement>("simpleinputok");
            AnchorElement cancelButton = Document.GetElementById<AnchorElement>("simpleinputcancel");

            okButton.AddEventListener("click", OkClicked, false);
            cancelButton.AddEventListener("click", CancelClicked, false);
            okCallback = callback;
        }

        public void OkClicked(ElementEvent e)
        {
            Close();
            if (okCallback != null)
            {
                okCallback();
            }
        }

        public void CancelClicked(ElementEvent e)
        {
            Close();
        }

        private void Close()
        {
            DivElement simpleInputElement = Document.GetElementById<DivElement>("simplemodal");
            simpleInputElement.Style.Display = "none";
            //Window.RemoveEventListener("click", NonMenuClick, true);
            textElement.RemoveEventListener("change", TextChanged, false);

            AnchorElement okButton = Document.GetElementById<AnchorElement>("simpleinputok");
            AnchorElement cancelButton = Document.GetElementById<AnchorElement>("simpleinputcancel");

            okButton.RemoveEventListener("click", OkClicked, false);
            cancelButton.RemoveEventListener("click", CancelClicked, false);
        }

        public void IgnoreMe(ElementEvent e)
        {
            ignoreNextClick = true;
        }

        public void TextChanged(ElementEvent e)
        {
            Text = textElement.Value;
            ignoreNextClick = true;
        }

      
        bool ignoreNextClick = false;

    }
}
