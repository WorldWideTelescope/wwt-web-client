using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Html.Media.Graphics;

namespace wwtlib
{

    public class ContextMenuStrip
    {
        public ContextMenuStrip()
        {

        }

        public List<ToolStripMenuItem> Items = new List<ToolStripMenuItem>();

        internal void Dispose()
        {
        }

        internal void NonMenuClick(ElementEvent e)
        {
            DivElement menu = Document.GetElementById<DivElement>("contextmenu");
            menu.Style.Display = "none";
            Window.RemoveEventListener("click", NonMenuClick, false);


            DivElement popup = Document.GetElementById<DivElement>("popoutmenu");
            while (popup.FirstChild != null)
            {
                popup.RemoveChild(popup.FirstChild);
            }

            popup.Style.Display = "none";

        }

        internal void MenuItemClicked(ElementEvent e)
        {
            TagMe me = (TagMe)(Object)e.CurrentTarget;

           

            me.ItemTag.Click(me.ItemTag, new EventArgs());
        }

        internal void Show(Vector2d position)
        {
            DivElement menu = Document.GetElementById<DivElement>("contextmenu");
            while(menu.FirstChild != null)
            {
                menu.RemoveChild(menu.FirstChild);
            }

            menu.ClassName = "contextmenu";
            menu.Style.Display = "block";
            menu.Style.Left = position.X.ToString() + "px";
            menu.Style.Top = position.Y.ToString() + "px";

            Window.AddEventListener("click", NonMenuClick, true);

            foreach (ToolStripMenuItem item in Items)
            {
                if (item.Visible)
                {
                    DivElement md = (DivElement)Document.CreateElement("div");
                    if (item.DropDownItems.Count > 0)
                    {
                        md.ClassName = "contextmenuitem submenu";
                    }
                    else
                    {
                        if (item.Checked)
                        {
                            md.ClassName = "contextmenuitem checkedmenu";
                        }
                        else
                        {
                            md.ClassName = "contextmenuitem";
                        }
                    }
                    md.InnerText = item.Name;

                    TagMe it = (TagMe)(Object)md;
                    it.ItemTag = item;
                   
                    md.AddEventListener("mouseover", OpenSubMenu, false);

                    if (item.Click != null)
                    {
                        md.AddEventListener("click", MenuItemClicked, false);
                    }

                    menu.AppendChild(md);
                }
            }
        }
        internal void OpenSubMenu(ElementEvent e)
        {
            TagMe me = (TagMe)(Object)e.CurrentTarget;
            ToolStripMenuItem child = me.ItemTag;

            DivElement menu = Document.GetElementById<DivElement>("popoutmenu");
            while (menu.FirstChild != null)
            {
                menu.RemoveChild(menu.FirstChild);
            }

            menu.Style.Display = "none";

            if (child.DropDownItems.Count == 0)
            {
                return;
            }

            Vector2d position = new Vector2d();
            position.X = e.CurrentTarget.ParentNode.OffsetLeft + e.CurrentTarget.ParentNode.ClientWidth;
            position.Y = e.CurrentTarget.ParentNode.OffsetTop + e.CurrentTarget.OffsetTop;


            menu.ClassName = "contextmenu";
            menu.Style.Display = "block";
            menu.Style.Left = position.X.ToString() + "px";
            menu.Style.Top = position.Y.ToString() + "px";

            Window.AddEventListener("click", NonMenuClick, true);

            foreach (ToolStripMenuItem item in child.DropDownItems)
            {
                if (item.Visible)
                {
                    DivElement md = (DivElement)Document.CreateElement("div");
                    md.ClassName = "contextmenuitem";
                    md.InnerText = item.Name;

                    TagMe it = (TagMe)(Object)md;
                    it.ItemTag = item;

                    md.AddEventListener("click", MenuItemClicked, false);
                    menu.AppendChild(md);
                }
            }
        }
    }

    public delegate void MenuEvent(object sender, EventArgs e);

    public class ToolStripMenuItem 
    {
        public string Name;
        public object Tag = null;

        public List<ToolStripMenuItem> DropDownItems = new List<ToolStripMenuItem>();

        public MenuEvent Click;
        public bool Checked = false;
        public bool Enabled = true;
        public bool Visible = true;

        public static ToolStripMenuItem Create(string name)
        {
            ToolStripMenuItem tsmi = new ToolStripMenuItem();
            tsmi.Name = name;

            return tsmi;
        }
    }

    public class ToolStripSeparator : ToolStripMenuItem
    {
        public ToolStripSeparator()
        {
            this.Name = "--------------------------------------";
        }
    }

    public class TagMe
    {
        public ToolStripMenuItem ItemTag;
    }
}
