using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Html.Media.Graphics;

namespace wwtlib
{
    public class TourEditor : IUiController
    {
        public TourEditor()
        {
        }

        public Selection Selection = new Selection();



        ContextMenuStrip contextMenu = new ContextMenuStrip();

        public static bool Capturing = false;

        //public static bool Scrubbing = false;
        //public static bool ScrubbingBackwards = false;
        //public static Date ScrubStartTime = Date.Now;

        public static IUiController CurrentEditor = null;

        //public void PreRender(RenderContext renderContext)
        //{
        //    if (tour.CurrentTourStop != null)
        //    {
        //        //if (Scrubbing)
        //        //{
        //        //    Settings.TourSettings = tour.CurrentTourStop;
        //        //    TimeSpan slideElapsedTime = SpaceTimeController.Now - ScrubStartTime;

        //        //    if (ScrubbingBackwards)
        //        //    {
        //        //        if (slideElapsedTime > tour.CurrentTourStop.Duration)
        //        //        {
        //        //            Scrubbing = false;
        //        //            tour.CurrentTourStop.TweenPosition = 0.0f;
        //        //        }
        //        //        else
        //        //        {
        //        //            tour.CurrentTourStop.TweenPosition = Math.Min(1, 1 - (double)(slideElapsedTime.TotalMilliseconds / tour.CurrentTourStop.Duration.TotalMilliseconds));
        //        //            TimeLine.RefreshUi();
        //        //        }
        //        //    }
        //        //    else
        //        //    {
        //        //        if (slideElapsedTime > tour.CurrentTourStop.Duration)
        //        //        {
        //        //            Scrubbing = false;
        //        //            tour.CurrentTourStop.TweenPosition = 1.0f;
        //        //        }
        //        //        else
        //        //        {
        //        //            tour.CurrentTourStop.TweenPosition = (double)(slideElapsedTime.TotalMilliseconds / tour.CurrentTourStop.Duration.TotalMilliseconds);
        //        //            TimeLine.RefreshUi();
        //        //        }
        //        //    }
        //        //}
        //        //else
        //        //{
        //            if (CurrentEditor != null)
        //            {
        //                CurrentEditor.PreRender(renderContext);
        //            }
        //        //}
        //    }
        //}

        //public void StartScrubbing(bool reverse)
        //{
        //    if (tour.CurrentTourStop != null)
        //    {
        //        Scrubbing = true;
        //        ScrubbingBackwards = reverse;

        //        if (ScrubbingBackwards)
        //        {
        //            if (tour.CurrentTourStop.TweenPosition == 0)
        //            {
        //                tour.CurrentTourStop.TweenPosition = 1.0f;
        //            }
        //            ScrubStartTime = SpaceTimeController.Now - TimeSpan.FromSeconds(tour.CurrentTourStop.Duration.TotalSeconds * (1 - tour.CurrentTourStop.TweenPosition));
        //        }
        //        else
        //        {
        //            if (tour.CurrentTourStop.TweenPosition == 1.0f)
        //            {
        //                tour.CurrentTourStop.TweenPosition = 0;
        //            }
        //            ScrubStartTime = SpaceTimeController.Now - TimeSpan.FromSeconds(tour.CurrentTourStop.Duration.TotalSeconds * (tour.CurrentTourStop.TweenPosition));
        //        }
        //    }
        //}


        //internal void PauseScrubbing(bool p)
        //{
        //    if (Scrubbing)
        //    {
        //        Scrubbing = false;
        //    }
        //    else
        //    {
        //        StartScrubbing(ScrubbingBackwards);
        //    }
        //}

        public void Render(RenderContext renderContext)
        {
            renderContext.SetupMatricesOverlays();
            //window.RenderContext11.DepthStencilMode = DepthStencilMode.Off;


            if (tour == null || tour.CurrentTourStop == null)
            {
                //if (Settings.GlobalSettings.ShowSafeArea && tour != null && tour.EditMode && !Capturing)
                //{
                //    DrawSafeZone(window);
                //}
                return;
            }

            foreach (Overlay overlay in tour.CurrentTourStop.Overlays)
            {
                if (overlay.Animate && Tour.CurrentTourStop.KeyFramed)
                {
                    overlay.TweenFactor = tour.CurrentTourStop.TweenPosition;
                }
                else if (!Tour.CurrentTourStop.KeyFramed)
                {
                    overlay.TweenFactor = tour.CurrentTourStop.TweenPosition < .5f ? 0f : 1f;
                }
                overlay.Draw3D(renderContext, true);
            }

            //if (Properties.Settings.Default.ShowSafeArea && tour != null && tour.EditMode && !Capturing)
            //{
            //    DrawSafeZone(window);
            //}
            Selection.Draw3D(renderContext, 1.0f);

            //if (!Scrubbing)
            {
                if (CurrentEditor != null)
                {
                    CurrentEditor.Render(renderContext);
                }

                Settings.TourSettings = null;
            }
        }

        //private void DrawSafeZone(Earth3d window)
        //{
        //    Rectangle rect = window.RenderWindow.ClientRectangle;

        //    int x = rect.Width / 2;
        //    int y = rect.Height / 2;

        //    int ratioWidth = rect.Height * 4 / 3;
        //    int halfWidth = (rect.Width - ratioWidth) / 2;


        //    DrawTranparentBox(window.RenderContext11, new Rectangle(-x, -y, halfWidth, rect.Height));
        //    DrawTranparentBox(window.RenderContext11, new Rectangle((rect.Width - halfWidth) - x, -y, halfWidth, rect.Height));
        //}

        //PositionColoredTextured[] boxPoints = new PositionColoredTextured[4];
        //private void DrawTranparentBox(RenderContext11 renderContext, Rectangle rect)
        //{


        //    Color Color = Color.FromArgb(128, 32, 32, 128);
        //    boxPoints[0].X = rect.X;
        //    boxPoints[0].Y = rect.Y;
        //    boxPoints[0].Z = .9f;
        //    boxPoints[0].W = 1;
        //    boxPoints[0].Tu = 0;
        //    boxPoints[0].Tv = 0;
        //    boxPoints[0].Color = Color;

        //    boxPoints[1].X = (double)(rect.X + (rect.Width));
        //    boxPoints[1].Y = (double)(rect.Y);
        //    boxPoints[1].Tu = 1;
        //    boxPoints[1].Tv = 0;
        //    boxPoints[1].Color = Color;
        //    boxPoints[1].Z = .9f;
        //    boxPoints[1].W = 1;

        //    boxPoints[2].X = (double)(rect.X);
        //    boxPoints[2].Y = (double)(rect.Y + (rect.Height));
        //    boxPoints[2].Tu = 0;
        //    boxPoints[2].Tv = 1;
        //    boxPoints[2].Color = Color;
        //    boxPoints[2].Z = .9f;
        //    boxPoints[2].W = 1;

        //    boxPoints[3].X = (double)(rect.X + (rect.Width));
        //    boxPoints[3].Y = (double)(rect.Y + (rect.Height));
        //    boxPoints[3].Tu = 1;
        //    boxPoints[3].Tv = 1;
        //    boxPoints[3].Color = Color;
        //    boxPoints[3].Z = .9f;
        //    boxPoints[3].W = 1;

        //    SharpDX.Matrix mat = SharpDX.Matrix.OrthoLH(renderContext.ViewPort.Width, renderContext.ViewPort.Height, 1, -1);

        //    Sprite2d.Draw(renderContext, boxPoints, 4, mat, true);

        //}

        TourDocument tour = null;

        public TourDocument Tour
        {
            get { return tour; }
            set { tour = value; }
        }

        public void Close()
        {
            if (tour != null)
            {
                // todo check for changes
                tour = null;
                Focus = null;
            }
        }

        public void ClearSelection()
        {
            Selection.ClearSelection();


            OverlayList.UpdateOverlayListSelection(Selection);
            Focus = null;
        }

        bool mouseDown = false;



        public Overlay Focus
        {
            get { return Selection.Focus; }
            set { Selection.Focus = value; }
        }

        Vector2d pointDown;


        public Vector2d PointToView(Vector2d pnt)
        {
            double clientHeight = WWTControl.Singleton.RenderContext.Height;
            double clientWidth = WWTControl.Singleton.RenderContext.Width;
            double viewWidth = (WWTControl.Singleton.RenderContext.Width / WWTControl.Singleton.RenderContext.Height) * 1116f;
            double x = (((double)pnt.X) / ((double)clientWidth) * viewWidth) - ((viewWidth - 1920) / 2);
            double y = ((double)pnt.Y) / clientHeight * 1116;

            return Vector2d.Create(x, y);
        }

        SelectionAnchor selectionAction = SelectionAnchor.None;
        bool needUndoFrame = false;
        public bool MouseDown(object sender, ElementEvent e)
        {
            brokeThreshold = false;
            needUndoFrame = true;
            Vector2d location = PointToView(Vector2d.Create(e.OffsetX, e.OffsetY));

            if (tour == null || tour.CurrentTourStop == null)
            {
                needUndoFrame = false;
                return false;
            }

            if (CurrentEditor != null)
            {
                if (CurrentEditor.MouseDown(sender, e))
                {
                    return true;
                }
            }


            //            if (e.Button == MouseButtons.Left)
            {
                if (Focus != null)
                {
                    if (Selection.MultiSelect)
                    {
                        foreach (Overlay overlay in Selection.SelectionSet)
                        {
                            if (overlay.HitTest(location))
                            {
                                selectionAction = SelectionAnchor.Move;
                                mouseDown = true;
                                pointDown = location;
                                Focus = overlay;
                                if (e.CtrlKey)
                                {
                                    dragCopying = true;
                                }

                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (Focus.HitTest(location))
                        {
                            selectionAction = SelectionAnchor.Move;
                            mouseDown = true;
                            pointDown = location;

                            if (e.CtrlKey)
                            {
                                dragCopying = true;
                            }

                            return true;
                        }
                    }

                    SelectionAnchor hit = Selection.HitTest(location);
                    if (hit != SelectionAnchor.None)
                    {
                        selectionAction = hit;
                        mouseDown = true;
                        if (hit == SelectionAnchor.Rotate)
                        {
                            pointDown = location;
                        }
                        else
                        {
                            pointDown = Selection.PointToSelectionSpace(location);
                        }
                        return true;
                    }
                }

                for (int i = tour.CurrentTourStop.Overlays.Count - 1; i >= 0; i--)
                {
                    if (tour.CurrentTourStop.Overlays[i].HitTest(location))
                    {
                        selectionAction = SelectionAnchor.Move;
                        Focus = tour.CurrentTourStop.Overlays[i];
                        if (e.CtrlKey || e.ShiftKey)
                        {
                            Selection.AddSelection(Focus);
                        }
                        else
                        {
                            Selection.SetSelection(Focus);
                        }

                        OverlayList.UpdateOverlayListSelection(Selection);
                        mouseDown = true;
                        pointDown = location;
                        return true;
                    }
                }
                Focus = null;
                ClearSelection();

            }
            needUndoFrame = false;
            return false;

        }
        Vector2d contextPoint = new Vector2d();
        public bool MouseUp(object sender, ElementEvent e)
        {
            brokeThreshold = false;
            if (CurrentEditor != null)
            {
                if (CurrentEditor.MouseUp(sender, e))
                {
                    return true;
                }
            }

            contextPoint = Vector2d.Create(e.OffsetX, e.OffsetY);
            if (mouseDown)
            {
                mouseDown = false;
                if (e.Button == 2)
                {
                    if (Focus != null)
                    {
                        ShowSelectionContextMenu(Vector2d.Create(e.OffsetX, e.OffsetY));
                    }
                }
                return true;
            }

            if (e.Button == 2)
            {
                if (Focus == null)
                {
                    ShowNoSelectionContextMenu(Vector2d.Create(e.OffsetX, e.OffsetY));
                }
                return true;
            }
            return false;
        }

        bool dragCopying = false;
        bool brokeThreshold = false;
        public bool MouseMove(object sender, ElementEvent e)
        {
            if (CurrentEditor != null)
            {
                if (CurrentEditor.MouseMove(sender, e))
                {
                    return true;
                }
            }


            Vector2d location = PointToView(Vector2d.Create(e.OffsetX, e.OffsetY));

            if (mouseDown && Focus != null)
            {
                UndoTourStopChange undoFrame = null;
                //todo localize
                string actionText = Language.GetLocalizedText(502, "Edit");
                if (needUndoFrame)
                {
                    undoFrame = new UndoTourStopChange(Language.GetLocalizedText(502, "Edit"), tour);
                }

                double moveX;
                double moveY;
                if (selectionAction != SelectionAnchor.Move && selectionAction != SelectionAnchor.Rotate)
                {
                    Vector2d newPoint = Selection.PointToSelectionSpace(location);
                    moveX = newPoint.X - pointDown.X;
                    moveY = newPoint.Y - pointDown.Y;
                    pointDown = newPoint;
                }
                else
                {
                    moveX = location.X - pointDown.X;
                    moveY = location.Y - pointDown.Y;
                    if (selectionAction == SelectionAnchor.Move && !brokeThreshold)
                    {
                        if (Math.Abs(moveX) > 3 || Math.Abs(moveY) > 3)
                        {
                            brokeThreshold = true;
                        }
                        else
                        {
                            return true;
                        }
                    }


                    pointDown = location;

                }

                if (dragCopying)
                {

                    if (Selection.MultiSelect)
                    {
                        Overlay[] set = Selection.SelectionSet;

                        ClearSelection();
                        foreach (Overlay overlay in set)
                        {
                            Overlay newOverlay = AddOverlay(overlay);
                            newOverlay.X = overlay.X;
                            newOverlay.Y = overlay.Y;
                            Focus = newOverlay;
                            Selection.AddSelection(Focus);

                        }
                        OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
                        dragCopying = false;
                    }
                    else
                    {
                        Overlay newOverlay = AddOverlay(Focus);
                        newOverlay.X = Focus.X;
                        newOverlay.Y = Focus.Y;
                        Focus = newOverlay;
                        Selection.SetSelection(Focus);
                        OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
                        dragCopying = false;
                    }
                }

                double aspect = Focus.Width / Focus.Height;

                Vector2d center = Vector2d.Create(Focus.X, Focus.Y);
                if (e.CtrlKey)
                {
                    actionText = Language.GetLocalizedText(537, "Resize");
                    switch (selectionAction)
                    {
                        case SelectionAnchor.TopLeft:
                            Focus.Width = Math.Max(2, Focus.Width - moveX * 2);
                            Focus.Height = Math.Max(2, Focus.Height - (moveX / aspect) * 2);
                            break;
                        case SelectionAnchor.Top:
                            Focus.Height = Math.Max(2, Focus.Height - moveY * 2);
                            break;
                        case SelectionAnchor.TopRight:
                            Focus.Width = Math.Max(2, Focus.Width + moveX * 2);
                            Focus.Height = Math.Max(2, Focus.Height + (moveX / aspect) * 2);
                            break;
                        case SelectionAnchor.Right:
                            Focus.Width = Math.Max(2, Focus.Width + moveX * 2);
                            break;
                        case SelectionAnchor.BottomRight:
                            Focus.Width = Math.Max(2, Focus.Width + moveX * 2);
                            Focus.Height = Math.Max(2, Focus.Height + (moveX / aspect) * 2);
                            break;
                        case SelectionAnchor.Bottom:
                            Focus.Height = Math.Max(2, Focus.Height + moveY * 2);
                            break;
                        case SelectionAnchor.BottomLeft:
                            Focus.Width = Math.Max(2, Focus.Width - moveX * 2);
                            Focus.Height = Math.Max(2, Focus.Height - (moveX / aspect) * 2);
                            break;
                        case SelectionAnchor.Left:
                            Focus.Width = Math.Max(2, Focus.Width - moveX * 2);
                            break;
                        case SelectionAnchor.Rotate:
                            actionText = Language.GetLocalizedText(538, "Rotate");
                            Focus.RotationAngle = Focus.RotationAngle + moveX / 10;
                            break;
                        case SelectionAnchor.Move:
                            actionText = Language.GetLocalizedText(539, "Drag Copy");
                            center.X += moveX;
                            center.Y += moveY;
                            break;
                        case SelectionAnchor.Center:
                            break;
                        case SelectionAnchor.None:
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    if (selectionAction != SelectionAnchor.Rotate && selectionAction != SelectionAnchor.Move)
                    {
                        if (moveX > (Focus.Width - 2))
                        {
                            moveX = 0;
                        }

                        if (moveY > (Focus.Height - 2))
                        {
                            moveY = 0;
                        }
                    }

                    //todo localize
                    actionText = Language.GetLocalizedText(537, "Resize");
                    switch (selectionAction)
                    {
                        case SelectionAnchor.TopLeft:
                            Focus.Width -= moveX;
                            Focus.Height -= (moveX / aspect);
                            center.X += (moveX / 2);
                            center.Y += ((moveX / aspect) / 2);
                            break;
                        case SelectionAnchor.Top:
                            Focus.Height -= moveY;
                            center.Y += (moveY / 2);
                            break;
                        case SelectionAnchor.TopRight:
                            Focus.Width += moveX;
                            Focus.Height += (moveX / aspect);
                            center.X += (moveX / 2);
                            center.Y -= ((moveX / aspect) / 2);
                            break;
                        case SelectionAnchor.Right:
                            Focus.Width += moveX;
                            center.X += (moveX / 2);
                            break;
                        case SelectionAnchor.BottomRight:
                            Focus.Width += moveX;
                            Focus.Height += (moveX / aspect);
                            center.X += (moveX / 2);
                            center.Y += ((moveX / aspect) / 2);
                            break;
                        case SelectionAnchor.Bottom:
                            Focus.Height += moveY;
                            center.Y += (moveY / 2);
                            break;
                        case SelectionAnchor.BottomLeft:
                            Focus.Width -= moveX;
                            Focus.Height -= (moveX / aspect);
                            center.X += (moveX / 2);
                            center.Y -= ((moveX / aspect) / 2);
                            break;
                        case SelectionAnchor.Left:
                            Focus.Width -= moveX;
                            center.X += (moveX / 2);
                            break;
                        case SelectionAnchor.Rotate:
                            actionText = Language.GetLocalizedText(538, "Rotate");
                            Focus.RotationAngle = Focus.RotationAngle + moveX;
                            break;
                        case SelectionAnchor.Move:
                            actionText = Language.GetLocalizedText(540, "Move");
                            center.X += moveX;
                            center.Y += moveY;
                            break;
                        case SelectionAnchor.Center:
                            break;
                        case SelectionAnchor.None:
                            break;
                        default:
                            break;
                    }
                }


                if (selectionAction != SelectionAnchor.Move && selectionAction != SelectionAnchor.Rotate)
                {
                    center = Selection.PointToScreenSpace(center);
                }

                if (Selection.MultiSelect)
                {
                    foreach (Overlay overlay in Selection.SelectionSet)
                    {
                        overlay.X += moveX;
                        overlay.Y += moveY;
                    }
                }

                else
                {
                    Focus.X = center.X;
                    Focus.Y = center.Y;
                }

                if (needUndoFrame)
                {
                    needUndoFrame = false;
                    undoFrame.ActionText = actionText;
                    Undo.Push(undoFrame);
                }
            }
            else
            {
                if (Focus != null)
                {
                    if (Focus.HitTest(location))
                    {
                        Cursor.Current = Cursors.SizeAll;
                        return false;
                    }

                    SelectionAnchor hit = Selection.HitTest(location);
                    if (hit == SelectionAnchor.None)
                    {
                        return false;
                    }

                    switch (hit)
                    {
                        case SelectionAnchor.TopLeft:
                            Cursor.Current = Cursors.SizeNWSE;
                            break;
                        case SelectionAnchor.Top:
                            Cursor.Current = Cursors.SizeNS;
                            break;
                        case SelectionAnchor.TopRight:
                            Cursor.Current = Cursors.SizeNESW;
                            break;
                        case SelectionAnchor.Right:
                            Cursor.Current = Cursors.SizeWE;
                            break;
                        case SelectionAnchor.BottomRight:
                            Cursor.Current = Cursors.SizeNWSE;
                            break;
                        case SelectionAnchor.Bottom:
                            Cursor.Current = Cursors.SizeNS;
                            break;
                        case SelectionAnchor.BottomLeft:
                            Cursor.Current = Cursors.SizeNESW;
                            break;
                        case SelectionAnchor.Left:
                            Cursor.Current = Cursors.SizeWE;
                            break;
                        case SelectionAnchor.Rotate:
                            Cursor.Current = Cursors.SizeWE;
                            break;
                        case SelectionAnchor.Center:
                            break;
                        case SelectionAnchor.None:
                            break;
                        default:
                            break;
                    }

                }
            }
            return false;
        }

        private void ShowNoSelectionContextMenu(Vector2d position)
        {
            if (contextMenu != null)
            {
                contextMenu.Dispose();
            }
            if (tour.CurrentTourStop == null)
            {
                return;
            }

            contextMenu = new ContextMenuStrip();

            ToolStripMenuItem pasteMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(425, "Paste"));
            //   IDataObject data = Clipboard.GetDataObject();

            pasteMenu.Enabled = clipboardType == Overlay.ClipboardFormat;

            pasteMenu.Click =pasteMenu_Click;
            contextMenu.Items.Add(pasteMenu);
            // Hide these menu types. This was for early testing before the UI matching Windows was working
            //ToolStripMenuItem AddCircle = ToolStripMenuItem.Create(Language.GetLocalizedText(444, "Circle"));
            //ToolStripMenuItem AddRectangle = ToolStripMenuItem.Create(Language.GetLocalizedText(445, "Rectangle"));
            //ToolStripMenuItem AddOpenRectangle = ToolStripMenuItem.Create(Language.GetLocalizedText(446, "Open Rectangle"));
            //ToolStripMenuItem AddRing = ToolStripMenuItem.Create(Language.GetLocalizedText(447, "Ring"));
            //ToolStripMenuItem AddLine = ToolStripMenuItem.Create(Language.GetLocalizedText(448, "Line"));
            //ToolStripMenuItem AddArrow = ToolStripMenuItem.Create(Language.GetLocalizedText(449, "Arrow"));
            //ToolStripMenuItem AddStar = ToolStripMenuItem.Create(Language.GetLocalizedText(450, "Star"));

            //AddCircle.Click = InsertShapeCircle_Click;
            //AddRectangle.Click = InsertShapeRectangle_Click;
            //AddOpenRectangle.Click = AddOpenRectangle_Click;
            //AddRing.Click = insertDonut_Click;
            //AddLine.Click = InsertShapeLine_Click;
            //AddArrow.Click = AddArrow_Click;
            //AddStar.Click = AddStar_Click;


            //contextMenu.Items.Add(AddCircle);
            //contextMenu.Items.Add(AddRectangle);
            //contextMenu.Items.Add(AddOpenRectangle);
            //contextMenu.Items.Add(AddRing);
            //contextMenu.Items.Add(AddLine);
            //contextMenu.Items.Add(AddArrow);
            //contextMenu.Items.Add(AddStar);
            contextMenu.Show(position);
        }

        void AddOpenRectangle_Click(object sender, EventArgs e)
        {
            AddShape("", ShapeType.OpenRectagle);
        }

        void AddStar_Click(object sender, EventArgs e)
        {
            AddShape("", ShapeType.Star);

        }

        private void InsertShapeCircle_Click(object sender, EventArgs e)
        {
            AddShape("", ShapeType.Circle);

        }

        private void InsertShapeRectangle_Click(object sender, EventArgs e)
        {
            AddShape("", ShapeType.Rectagle);

        }

        private void InsertShapeLine_Click(object sender, EventArgs e)
        {
            AddShape("", ShapeType.Line);

        }

        private void insertDonut_Click(object sender, EventArgs e)
        {
            AddShape("", ShapeType.Donut);

        }


        void AddArrow_Click(object sender, EventArgs e)
        {
            AddShape("", ShapeType.Arrow);

        }

        public void ShowSelectionContextMenu( Vector2d position)
        {
            if (Focus == null)
            {
                return;
            }

            bool multiSelect = Selection.MultiSelect;

            if (contextMenu != null)
            {
                contextMenu.Dispose();
            }

            contextMenu = new ContextMenuStrip();

            ToolStripMenuItem cutMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(427, "Cut"));
            ToolStripMenuItem copyMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(428, "Copy"));
            ToolStripMenuItem pasteMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(425, "Paste"));
            ToolStripMenuItem deleteMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(167, "Delete"));
            ToolStripSeparator sep1 = new ToolStripSeparator();
            ToolStripSeparator sep2 = new ToolStripSeparator();
            ToolStripSeparator sep3 = new ToolStripSeparator();
            ToolStripMenuItem bringToFront = ToolStripMenuItem.Create(Language.GetLocalizedText(452, "Bring to Front"));
            ToolStripMenuItem sendToBack = ToolStripMenuItem.Create(Language.GetLocalizedText(453, "Send to Back"));
            ToolStripMenuItem bringForward = ToolStripMenuItem.Create(Language.GetLocalizedText(454, "Bring Forward"));
            ToolStripMenuItem sendBackward = ToolStripMenuItem.Create(Language.GetLocalizedText(455, "Send Backward"));
            ToolStripMenuItem properties = ToolStripMenuItem.Create(Language.GetLocalizedText(20, "Properties"));
            ToolStripMenuItem editText = ToolStripMenuItem.Create(Language.GetLocalizedText(502, "Edit"));
            //ToolStripMenuItem fullDome = new ToolStripMenuItem(Language.GetLocalizedText(574, "Full Dome"));
            ToolStripMenuItem url = ToolStripMenuItem.Create(Language.GetLocalizedText(587, "Hyperlink"));
            string linkString = Focus.LinkID;
            switch (Focus.LinkID)
            {
                case "":
                case null:
                    //linkString = " (No Link)";
                    linkString = " (" + Language.GetLocalizedText(609, "No Link") + ")";
                    break;
                case "Next":
                    //linkString = " (Next Slide)";
                    linkString = " (" + Language.GetLocalizedText(610, "Next Slide") + ")";
                    break;
                case "Return":
                    //linkString = " (Return to Caller)";
                    linkString = " (" + Language.GetLocalizedText(602, "Return to Caller") + ")";
                    break;
                default:
                    int index = Tour.GetTourStopIndexByID(Focus.LinkID);
                    if (index > -1)
                    {
                        if (String.IsNullOrEmpty(tour.TourStops[index].Description))
                        {
                            linkString = String.Format(" (" + Language.GetLocalizedText(1340, "Slide") + " {0})", index);
                        }
                        else
                        {
                            linkString = " (" + tour.TourStops[index].Description + ")";
                        }
                    }
                    break;
            }

            ToolStripMenuItem animateMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(588, "Animate"));
            //  ToolStripMenuItem addKeyframes = new ToolStripMenuItem(Language.GetLocalizedText(1280, "Add Keyframe"));
            //  ToolStripMenuItem addToTimeline = new ToolStripMenuItem(Language.GetLocalizedText(1290, "Add to Timeline"));

            ToolStripMenuItem linkID = ToolStripMenuItem.Create(Language.GetLocalizedText(589, "Link to Slide") + linkString);
            ToolStripMenuItem pickColor = ToolStripMenuItem.Create(Language.GetLocalizedText(458, "Color/Opacity"));
            ToolStripMenuItem flipbookProperties = ToolStripMenuItem.Create(Language.GetLocalizedText(630, "Flipbook Properties"));
            ToolStripMenuItem interpolateMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(1029, "Animation Tween Type"));

            ToolStripMenuItem Linear = ToolStripMenuItem.Create(Language.GetLocalizedText(1030, "Linear"));
            ToolStripMenuItem Ease = ToolStripMenuItem.Create(Language.GetLocalizedText(1031, "Ease In/Out"));
            ToolStripMenuItem EaseIn = ToolStripMenuItem.Create(Language.GetLocalizedText(1032, "Ease In"));
            ToolStripMenuItem EaseOut = ToolStripMenuItem.Create(Language.GetLocalizedText(1033, "Ease Out"));
            ToolStripMenuItem Exponential = ToolStripMenuItem.Create(Language.GetLocalizedText(1034, "Exponential"));
            ToolStripMenuItem Default = ToolStripMenuItem.Create(Language.GetLocalizedText(1035, "Slide Default"));

            ToolStripMenuItem Align = ToolStripMenuItem.Create(Language.GetLocalizedText(790, "Align"));
            ToolStripMenuItem AlignTop = ToolStripMenuItem.Create(Language.GetLocalizedText(1333, "Top"));
            ToolStripMenuItem AlignBottom = ToolStripMenuItem.Create(Language.GetLocalizedText(1334, "Bottom"));
            ToolStripMenuItem AlignLeft = ToolStripMenuItem.Create(Language.GetLocalizedText(1335, "Left"));
            ToolStripMenuItem AlignRight = ToolStripMenuItem.Create(Language.GetLocalizedText(1336, "Right"));
            ToolStripMenuItem AlignHorizon = ToolStripMenuItem.Create(Language.GetLocalizedText(1337, "Horizontal"));
            ToolStripMenuItem AlignVertical = ToolStripMenuItem.Create(Language.GetLocalizedText(1338, "Vertical"));
            ToolStripMenuItem AlignCenter = ToolStripMenuItem.Create(Language.GetLocalizedText(1339, "Centered"));

            Align.DropDownItems.Add(AlignTop);
            Align.DropDownItems.Add(AlignBottom);
            Align.DropDownItems.Add(AlignLeft);
            Align.DropDownItems.Add(AlignRight);
            Align.DropDownItems.Add(AlignHorizon);
            Align.DropDownItems.Add(AlignVertical);
            Align.DropDownItems.Add(AlignCenter);



            Linear.Tag = InterpolationType.Linear;
            Ease.Tag = InterpolationType.EaseInOut;
            EaseIn.Tag = InterpolationType.EaseIn;
            EaseOut.Tag = InterpolationType.EaseOut;
            Exponential.Tag = InterpolationType.Exponential;
            Default.Tag = InterpolationType.DefaultV;

            Linear.Click = Interpolation_Click;
            Ease.Click = Interpolation_Click;
            EaseIn.Click = Interpolation_Click;
            EaseOut.Click = Interpolation_Click;
            Exponential.Click = Interpolation_Click;
            Default.Click = Interpolation_Click;

            switch (Focus.InterpolationType)
            {
                case InterpolationType.Linear:
                    Linear.Checked = true;
                    break;
                case InterpolationType.EaseIn:
                    EaseIn.Checked = true;
                    break;
                case InterpolationType.EaseOut:
                    EaseOut.Checked = true;
                    break;
                case InterpolationType.EaseInOut:
                    Ease.Checked = true;
                    break;
                case InterpolationType.Exponential:
                    Exponential.Checked = true;
                    break;
                case InterpolationType.DefaultV:
                    Default.Checked = true;
                    break;
                default:
                    break;
            }


            interpolateMenu.DropDownItems.Add(Default);
            interpolateMenu.DropDownItems.Add(Linear);
            interpolateMenu.DropDownItems.Add(Ease);
            interpolateMenu.DropDownItems.Add(EaseIn);
            interpolateMenu.DropDownItems.Add(EaseOut);
            interpolateMenu.DropDownItems.Add(Exponential);



            cutMenu.Click = cutMenu_Click;
            copyMenu.Click = copyMenu_Click;
            deleteMenu.Click = deleteMenu_Click;
            bringToFront.Click = bringToFront_Click;
            sendToBack.Click = sendToBack_Click;
            sendBackward.Click = sendBackward_Click;
            bringForward.Click = bringForward_Click;
            properties.Click = properties_Click;
            editText.Click = editText_Click;
            url.Click = url_Click;
            pickColor.Click = pickColor_Click;
            pasteMenu.Click = pasteMenu_Click;
            animateMenu.Click = animateMenu_Click;

            //  addToTimeline.Click =addKeyframes_Click;
            // addKeyframes.Click =addKeyframes_Click;

            //fullDome.Click =fullDome_Click;
            flipbookProperties.Click = flipbookProperties_Click;
            linkID.Click = linkID_Click;

            AlignTop.Click = AlignTop_Click;
            AlignBottom.Click = AlignBottom_Click;
            AlignLeft.Click = AlignLeft_Click;
            AlignRight.Click = AlignRight_Click;
            AlignHorizon.Click = AlignHorizon_Click;
            AlignVertical.Click = AlignVertical_Click;
            AlignCenter.Click = AlignCenter_Click;



            contextMenu.Items.Add(cutMenu);
            contextMenu.Items.Add(copyMenu);
            contextMenu.Items.Add(pasteMenu);
            contextMenu.Items.Add(deleteMenu);
            contextMenu.Items.Add(sep1);
            contextMenu.Items.Add(bringToFront);
            contextMenu.Items.Add(sendToBack);
            contextMenu.Items.Add(bringForward);
            contextMenu.Items.Add(sendBackward);
            contextMenu.Items.Add(Align);
            contextMenu.Items.Add(sep2);

            //IDataObject data = Clipboard.GetDataObject();

            //pasteMenu.Enabled = Clipboard.ContainsImage() | Clipboard.ContainsText() | Clipboard.ContainsAudio() | data.GetDataPresent(Overlay.ClipboardFormat);
            pasteMenu.Enabled = false; // until we can fix clipboard
            contextMenu.Items.Add(pickColor);
            contextMenu.Items.Add(url);
            contextMenu.Items.Add(linkID);
            contextMenu.Items.Add(animateMenu);

            //if (Focus.AnimationTarget == null)
            //{
            //    contextMenu.Items.Add(addToTimeline);
            //}
            //else
            //{
            //    contextMenu.Items.Add(addKeyframes);
            //}


            // contextMenu.Items.Add(fullDome);
            contextMenu.Items.Add(sep3);
            contextMenu.Items.Add(flipbookProperties);
            animateMenu.Checked = Focus.Animate;
            // fullDome.Checked = Focus.Anchor == OverlayAnchor.Dome;
            contextMenu.Items.Add(interpolateMenu);
            interpolateMenu.Enabled = Focus.Animate;

            flipbookProperties.Visible = (Focus is FlipbookOverlay);
            sep3.Visible = (Focus is FlipbookOverlay);

            if (multiSelect)
            {
                url.Visible = false;
                linkID.Visible = false;
                properties.Visible = false;
                flipbookProperties.Visible = false;
                bringForward.Visible = false;
                sendBackward.Visible = false;
            }
            else
            {
                Align.Visible = false;
            }

            contextMenu.Items.Add(properties);
            if (Focus != null)
            {
                if (Focus.GetType() == typeof(TextOverlay))
                {
                    contextMenu.Items.Add(editText);
                }
            }

            contextMenu.Show(position);
        }

        void editText_Click(object sender, EventArgs e)
        {
            if (Focus != null)
            {
                if (Focus.GetType() == typeof(TextOverlay))
                {
                    EditText();
                }
            }
        }

        //void addKeyframes_Click(object sender, EventArgs e)
        //{
        //    if (Focus != null)
        //    {
        //        Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1280, "Add Keyframe"), tour));

        //        Tour.CurrentTourStop.KeyFramed = true;

        //        foreach (Overlay overlay in selection.SelectionSet)
        //        {

        //            if (overlay.AnimationTarget == null)
        //            {
        //                double savedTween = overlay.TweenFactor;
        //                overlay.TweenFactor = 0;
        //                overlay.AnimationTarget = new AnimationTarget(Tour.CurrentTourStop);
        //                overlay.AnimationTarget.Target = overlay;
        //                overlay.AnimationTarget.ParameterNames.AddRange(overlay.GetParamNames());
        //                overlay.AnimationTarget.CurrentParameters = overlay.GetParams();
        //                overlay.AnimationTarget.SetKeyFrame(0, Key.KeyType.Linear);
        //                if (overlay.Animate)
        //                {
        //                    overlay.TweenFactor = 1;
        //                    overlay.AnimationTarget.CurrentParameters = overlay.GetParams();
        //                    overlay.AnimationTarget.SetKeyFrame(1, Key.KeyType.Linear);
        //                    overlay.TweenFactor = savedTween;
        //                    overlay.Animate = false;
        //                }

        //                Tour.CurrentTourStop.AnimationTargets.Add(overlay.AnimationTarget);
        //                TimeLine.RefreshUi();
        //            }
        //            else
        //            {
        //                overlay.AnimationTarget.SetKeyFrame(Tour.CurrentTourStop.TweenPosition, Key.KeyType.Linear);
        //            }
        //        }
        //        TimeLine.RefreshUi();
        //    }
        //}

        //void fullDome_Click(object sender, EventArgs e)
        //{
        //    Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1341, "Anchor Full Dome"), tour));
        //    if (Focus != null)
        //    {
        //        bool fullDome = Focus.Anchor != OverlayAnchor.Dome;

        //        foreach (Overlay overlay in selection.SelectionSet)
        //        {
        //            overlay.Anchor = fullDome ? OverlayAnchor.Dome : OverlayAnchor.Screen;
        //        }
        //    }
        //}

        void AlignVertical_Click(object sender, EventArgs e)
        {
            if (Focus == null)
            {
                return;
            }

            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1036, "Vertical Align"), tour));

            double xCenter = Focus.X;

            foreach (Overlay overlay in Selection.SelectionSet)
            {
                overlay.X = xCenter;
            }
        }

        void AlignHorizon_Click(object sender, EventArgs e)
        {
            if (Focus == null)
            {
                return;
            }

            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1037, "Horizontal Align"), tour));

            double yCenter = Focus.Y;

            foreach (Overlay overlay in Selection.SelectionSet)
            {
                overlay.Y = yCenter;
            }
        }

        void AlignCenter_Click(object sender, EventArgs e)
        {
            if (Focus == null)
            {
                return;
            }

            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1038, "Align Centers"), tour));

            double yCenter = Focus.Y;
            double xCenter = Focus.X;
            foreach (Overlay overlay in Selection.SelectionSet)
            {
                overlay.Y = yCenter;
                overlay.X = xCenter;
            }
        }

        void AlignRight_Click(object sender, EventArgs e)
        {
            if (Focus == null)
            {
                return;
            }

            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1040, "Align Right"), tour));

            double left = Focus.X + Focus.Width / 2;

            foreach (Overlay overlay in Selection.SelectionSet)
            {
                overlay.X = left - overlay.Width / 2;
            }
        }

        void AlignLeft_Click(object sender, EventArgs e)
        {
            if (Focus == null)
            {
                return;
            }

            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1041, "Align Left"), tour));

            double right = Focus.X - Focus.Width / 2;

            foreach (Overlay overlay in Selection.SelectionSet)
            {
                overlay.X = right + overlay.Width / 2;
            }

        }

        void AlignBottom_Click(object sender, EventArgs e)
        {

            if (Focus == null)
            {
                return;
            }

            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1042, "Align Bottoms"), tour));

            double top = Focus.Y + Focus.Height / 2;

            foreach (Overlay overlay in Selection.SelectionSet)
            {
                overlay.Y = top - overlay.Height / 2;
            }

        }

        void AlignTop_Click(object sender, EventArgs e)
        {
            if (Focus == null)
            {
                return;
            }

            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1039, "Align Tops"), tour));

            double top = Focus.Y - Focus.Height / 2;

            foreach (Overlay overlay in Selection.SelectionSet)
            {
                overlay.Y = top + overlay.Height / 2;
            }
        }

        void Interpolation_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            if (Focus != null)
            {
                foreach (Overlay overlay in Selection.SelectionSet)
                {
                    overlay.InterpolationType = (InterpolationType)item.Tag;
                }
            }
        }

        public SetNextSlideDelegate nextSlideCallback = null;
        private void NextSlideChosen()
        {
            if (selectDialog.OK)
            {
                Focus.LinkID = selectDialog.ID;
            }
        }
        private SelectLink selectDialog;
        void linkID_Click(object sender, EventArgs e)
        {
            SelectLink selectDialog = new SelectLink();
            selectDialog.ID = Focus.LinkID;
            nextSlideCallback(selectDialog, NextSlideChosen);
            
        }

        void flipbookProperties_Click(object sender, EventArgs e)
        {
            //FlipbookOverlay flipbook = (FlipbookOverlay)Focus;
            //FlipbookSetup properties = new FlipbookSetup();

            //properties.LoopType = flipbook.LoopType;
            //properties.FramesY = flipbook.FramesY;
            //properties.FramesX = flipbook.FramesX;
            //properties.FrameSequence = flipbook.FrameSequence;
            //properties.StartFrame = flipbook.StartFrame;
            //properties.Frames = flipbook.Frames;

            //if (properties.ShowDialog() == DialogResult.OK)
            //{
            //    flipbook.LoopType = properties.LoopType;
            //    flipbook.FramesY = properties.FramesY;
            //    flipbook.FramesX = properties.FramesX;
            //    flipbook.FrameSequence = properties.FrameSequence;
            //    flipbook.StartFrame = properties.StartFrame;
            //    flipbook.Frames = properties.Frames;
            //}
        }

        void animateMenu_Click(object sender, EventArgs e)
        {
            if (Focus != null)
            {
                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(588, "Animate"), tour));

                bool animate = !Focus.Animate;

                foreach (Overlay overlay in Selection.SelectionSet)
                {
                    overlay.Animate = animate;
                }

            }
        }

        void url_Click(object sender, EventArgs e)
        {
            if (Focus != null)
            {
                SimpleInput input = new SimpleInput(Language.GetLocalizedText(541, "Edit Hyperlink"), Language.GetLocalizedText(542, "Url"), Focus.Url, 2048);
                if (input.ShowDialog() == DialogResult.OK)
                {
                    Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(541, "Edit Hyperlink"), tour));
                    Focus.Url = input.ResultText;
                }
            }
        }

        void pickColor_Click(object sender, EventArgs e)
        {
            ColorPicker picker = new ColorPicker();

            //picker.Location = Cursor.Position;

            //picker.Color = Focus.Color;

            // if (picker.ShowDialog() == DialogResult.OK)

            picker.CallBack = delegate
            {
                //todo localize
                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(543, "Edit Color"), tour));
                foreach (Overlay overlay in Selection.SelectionSet)
                {
                    overlay.Color = picker.Color;
                }
            };

            picker.Show( Vector2d.Create(500, 500));
        }

        void volume_Click(object sender, EventArgs e)
        {
            PopupVolume vol = new PopupVolume();
            vol.Volume = ((AudioOverlay)Focus).Volume;
            vol.ShowDialog();
            ((AudioOverlay)Focus).Volume = vol.Volume;
        }

        void deleteMenu_Click(object sender, EventArgs e)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return;
            }
            //todo localize
            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(167, "Delete"), tour));

            foreach (Overlay overlay in Selection.SelectionSet)
            {
                tour.CurrentTourStop.RemoveOverlay(overlay);
            }

            Focus = null;
            ClearSelection();
            OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
        }

        void properties_Click(object sender, EventArgs e)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {

            }

            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(549, "Properties Edit"), tour));
            OverlayProperties props = new OverlayProperties();
            props.Overlay = Focus;

            props.ShowDialog();
            OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
        }

        void bringForward_Click(object sender, EventArgs e)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return;
            }
            //todo localize
            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(454, "Bring Forward"), tour));
            foreach (Overlay overlay in GetSortedSelection(false))
            {
                tour.CurrentTourStop.BringForward(overlay);
            }
            OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
        }

        void sendBackward_Click(object sender, EventArgs e)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return;
            }
            //todo localize
            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(455, "Send Backward"), tour));
            foreach (Overlay overlay in GetSortedSelection(true))
            {
                tour.CurrentTourStop.SendBackward(overlay);
            }
            OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
        }

        void sendToBack_Click(object sender, EventArgs e)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return;
            }
            //todo localize
            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(453, "Send to Back"), tour));
            foreach (Overlay overlay in GetSortedSelection(true))
            {
                tour.CurrentTourStop.SendToBack(overlay);
            }
            OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
        }

        void bringToFront_Click(object sender, EventArgs e)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return;
            }
            //todo localize
            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(452, "Bring to Front"), tour));
            foreach (Overlay overlay in GetSortedSelection(false))
            {
                tour.CurrentTourStop.BringToFront(overlay);
            }
            OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
        }

        List<Overlay> GetSortedSelection(bool reverse)
        {
            List<Overlay> sorted = new List<Overlay>();

            foreach (Overlay ov in Selection.SelectionSet)
            {
                sorted.Add(ov);
            }


            if (reverse)
            {
                sorted.Sort(delegate (Overlay p1, Overlay p2) { return -Util.Compare(p1.ZOrder, p2.ZOrder); });
            }
            else
            {

                sorted.Sort(delegate (Overlay p1, Overlay p2) { return Util.Compare(p1.ZOrder, p2.ZOrder); });
            }
            return sorted;
        }

        public string clipboardData = "";
        public string clipboardType = "";
        void copyMenu_Click(object sender, EventArgs e)
        {
            //todo impliment copy
            if (tour == null || tour.CurrentTourStop == null)
            {
                return;
            }


            XmlTextWriter writer = new XmlTextWriter();
            
            writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
            writer.WriteStartElement("Overlays");
            foreach (Overlay overlay in Selection.SelectionSet)
            {
                overlay.SaveToXml(writer, true);
            }

            writer.WriteEndElement();

            clipboardData = writer.Body;
            clipboardType = Overlay.ClipboardFormat;
        }

        void cutMenu_Click(object sender, EventArgs e)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return;
            }
            //todo localize
            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(427, "Cut"), tour));
            copyMenu_Click(sender, e);

            foreach (Overlay overlay in Selection.SelectionSet)
            {
                tour.CurrentTourStop.RemoveOverlay(overlay);
            }
            Focus = null;
            ClearSelection();
            OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
        }

        void pasteMenu_Click(object sender, EventArgs e)
        {
            //todo impliment paste

            Undo.Push(new UndoTourSlidelistChange(Language.GetLocalizedText(544, "Paste Object"), tour));

            
            if (clipboardType == Overlay.ClipboardFormat)
            {

                XmlDocumentParser xParser = new XmlDocumentParser();
                XmlDocument doc = xParser.ParseFromString(clipboardData, "text/xml");

               
                ClearSelection();
        
                XmlNode parent = Util.SelectSingleNode(doc, "Overlays");
                foreach (XmlNode child in parent.ChildNodes)
                {
                    if (child.Name == "Overlay")
                    {
                        Overlay copy = Overlay.FromXml(tour.CurrentTourStop, child);
                        //if (copy.AnimationTarget != null)
                        //{
                        //    copy.Id = Guid.NewGuid().ToString();
                        //    copy.AnimationTarget.TargetID = copy.Id;
                        //    tour.CurrentTourStop.AnimationTargets.Add(copy.AnimationTarget);
                        //}
                        bool found = false;
                        double maxX = 0;
                        double maxY = 0;
                        foreach (Overlay item in tour.CurrentTourStop.Overlays)
                        {
                            if (item.Id == copy.Id && item.GetType() == copy.GetType())
                            {
                                found = true;
                                if (maxY < item.Y || maxX < item.X)
                                {
                                    maxX = item.X;
                                    maxY = item.Y;
                                }
                            }
                        }

                        if (found)
                        {
                            copy.X = maxX + 20;
                            copy.Y = maxY + 20;
                        }

                        tour.CurrentTourStop.AddOverlay(copy);
                        Focus = copy;
                        Selection.AddSelection(Focus);
                        OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
                    }
                }
            }
            //else if (UiTools.IsMetaFileAvailable())
            //{
            //    Image img = UiTools.GetMetafileFromClipboard();
            //    if (img != null)
            //    {
            //        BitmapOverlay bmp = new BitmapOverlay(tour.CurrentTourStop, img);
            //        tour.CurrentTourStop.AddOverlay(bmp);
            //        bmp.X = contextPoint.X;
            //        bmp.Y = contextPoint.Y;
            //        Focus = bmp;
            //        selection.SetSelection(Focus);
            //        OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
            //    }
            //}
            //else if (Clipboard.ContainsText() && Clipboard.GetText().Length > 0)
            //{
            //    TextObject temp = TextEditor.DefaultTextobject;
            //    temp.Text = Clipboard.GetText();

            //    TextOverlay text = new TextOverlay(temp);
            //    //text.X = Earth3d.MainWindow.ClientRectangle.Width / 2;
            //    //text.Y = Earth3d.MainWindow.ClientRectangle.Height / 2;
            //    text.X = contextPoint.X;
            //    text.Y = contextPoint.Y;
            //    tour.CurrentTourStop.AddOverlay(text);
            //    Focus = text;
            //    selection.SetSelection(Focus);
            //    OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
            //}
            //else if (Clipboard.ContainsImage())
            //{
            //    Image img = Clipboard.GetImage();
            //    BitmapOverlay bmp = new BitmapOverlay(tour.CurrentTourStop, img);
            //    tour.CurrentTourStop.AddOverlay(bmp);
            //    bmp.X = contextPoint.X;
            //    bmp.Y = contextPoint.Y;
            //    Focus = bmp;
            //    selection.SetSelection(Focus);
            //    OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
            //}

        }

        public bool MouseClick(object sender, ElementEvent e)
        {
            if (CurrentEditor != null)
            {
                if (CurrentEditor.MouseClick(sender, e))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Click(object sender, ElementEvent e)
        {
            if (CurrentEditor != null)
            {
                if (CurrentEditor.Click(sender, e))
                {
                    return true;
                }
            }
            return false;
        }

        public bool MouseDoubleClick(object sender, ElementEvent e)
        {
            if (CurrentEditor != null)
            {
                if (CurrentEditor.MouseDoubleClick(sender, e))
                {
                    return true;
                }
            }

            if (Focus != null)
            {
                if (Focus.GetType() == typeof(TextOverlay))
                {
                    EditText();
                    return true;
                }
            }
            return true;
        }


       
        public TextEditorDelegate editTextCallback = null;

        private void DoneEditing()
        {
           Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(545, "Text Edit"), tour));
           ((TextOverlay)Focus).Width = 0;
           ((TextOverlay)Focus).Height = 0;
           Focus.Color = ((TextOverlay)Focus).TextObject.ForegroundColor;
           Focus.CleanUp();
        }

        private void EditText()
        {
            TextObject textObj = ((TextOverlay)Focus).TextObject;
            editTextCallback(textObj, DoneEditing);

            //todo port Text Editor
            //TextEditor te = new TextEditor();
            //te.TextObject = ((TextOverlay)Focus).TextObject;
            //if (te.ShowDialog() == DialogResult.OK)
            //{
            //    //todo localize
            //    Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(545, "Text Edit"), tour));
            //    ((TextOverlay)Focus).TextObject = te.TextObject;
            //    ((TextOverlay)Focus).Width = 0;
            //    ((TextOverlay)Focus).Height = 0;
            //    Focus.Color = te.TextObject.ForegroundColor;
            //    Focus.CleanUp();
            //}
        }

        public bool KeyDown(object sender, ElementEvent e)
        {
            if (CurrentEditor != null)
            {
                if (CurrentEditor.KeyDown(sender, e))
                {
                    return true;
                }
            }

            int increment = 1;
            if (e.CtrlKey)
            {
                increment = 10;
            }

            switch ((Keys)e.KeyCode)
            {
                case Keys.A:
                    if (e.CtrlKey)
                    {
                        ClearSelection();
                        Selection.AddSelectionRange(tour.CurrentTourStop.Overlays);
                        OverlayList.UpdateOverlayListSelection(Selection);
                        if (tour.CurrentTourStop.Overlays.Count > 0)
                        {
                            Focus = tour.CurrentTourStop.Overlays[0];
                        }
                    }
                    break;
                case Keys.Z:
                    if (e.CtrlKey)
                    {
                        if (Undo.PeekAction())
                        {
                            TourEdit.UndoStep();
                        }
                        else
                        {
                            UiTools.Beep();
                        }
                    }
                    break;
                case Keys.Y:
                    if (e.CtrlKey)
                    {
                        if (Undo.PeekRedoAction())
                        {
                            TourEdit.RedoStep();
                        }
                        else
                        {
                            UiTools.Beep();
                        }
                    }
                    break;
                case Keys.C:
                    if (e.CtrlKey)
                    {
                        this.copyMenu_Click(this, new EventArgs());
                    }
                    break;
                case Keys.V:
                    if (e.CtrlKey)
                    {
                        this.pasteMenu_Click(this, new EventArgs());
                    }
                    break;
                case Keys.X:
                    if (e.CtrlKey)
                    {
                        this.cutMenu_Click(this, new EventArgs());
                    }
                    break;
                case Keys.DeleteKey:
                    this.deleteMenu_Click(null, null);
                    return true;
                case Keys.Tab:
                    if (e.ShiftKey)
                    {
                        SelectLast();
                    }
                    else
                    {
                        SelectNext();
                    }
                    return true;
                case Keys.Left:
                    if (Focus != null)
                    {
                        foreach (Overlay overlay in Selection.SelectionSet)
                        {
                            if (e.ShiftKey)
                            {
                                if (e.AltKey)
                                {
                                    if (overlay.Width > increment)
                                    {
                                        Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(537, "Resize"), tour));
                                        overlay.Width -= increment;
                                    }
                                }
                                else
                                {
                                    double aspect = overlay.Width / overlay.Height;
                                    if (overlay.Width > increment && overlay.Height > (increment * aspect))
                                    {
                                        Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(537, "Resize"), tour));
                                        overlay.Width -= increment;
                                        overlay.Height -= increment * aspect;
                                    }
                                }
                            }
                            else if (e.AltKey)
                            {
                                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(538, "Rotate"), tour));
                                overlay.RotationAngle -= increment;
                            }
                            else
                            {
                                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(540, "Move"), tour));
                                overlay.X -= increment;
                            }
                        }
                        return true;
                    }
                    break;
                case Keys.Right:
                    if (Focus != null)
                    {
                        foreach (Overlay overlay in Selection.SelectionSet)
                        {
                            if (e.ShiftKey)
                            {
                                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(537, "Resize"), tour));
                                if (e.AltKey)
                                {
                                    overlay.Width += increment;

                                }
                                else
                                {
                                    double aspect = overlay.Width / overlay.Height;
                                    overlay.Width += increment;
                                    overlay.Height += increment * aspect;
                                }
                            }
                            else if (e.AltKey)
                            {
                                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(538, "Rotate"), tour));
                                overlay.RotationAngle += increment;
                            }
                            else
                            {
                                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(540, "Move"), tour));
                                overlay.X += increment;
                            }
                        }
                        return true;
                    }
                    break;
                case Keys.Up:
                    if (Focus != null)
                    {
                        foreach (Overlay overlay in Selection.SelectionSet)
                        {
                            if (e.ShiftKey)
                            {
                                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(537, "Resize"), tour));
                                if (e.AltKey)
                                {
                                    overlay.Height += increment;
                                }
                                else
                                {
                                    double aspect = overlay.Width / overlay.Height;
                                    overlay.Width += increment;
                                    overlay.Height += increment * aspect;
                                }
                            }
                            else if (!e.AltKey)
                            {
                                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(540, "Move"), tour));
                                overlay.Y -= increment;
                            }
                        }
                        return true;
                    }
                    break;
                case Keys.Down:
                    if (Focus != null)
                    {
                        foreach (Overlay overlay in Selection.SelectionSet)
                        {
                            if (e.ShiftKey)
                            {
                                if (e.AltKey)
                                {
                                    if (overlay.Height > increment)
                                    {
                                        Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(537, "Resize"), tour));
                                        overlay.Height -= increment;
                                    }
                                }
                                else
                                {
                                    double aspect = overlay.Width / overlay.Height;
                                    if (overlay.Width > increment && overlay.Height > (increment * aspect))
                                    {
                                        Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(537, "Resize"), tour));
                                        overlay.Width -= increment;
                                        overlay.Height -= increment * aspect;
                                    }
                                }
                            }
                            else if (!e.AltKey)
                            {
                                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(540, "Move"), tour));
                                overlay.Y += increment;
                            }
                        }
                        return true;
                    }
                    break;
                case Keys.PageDown:
                    // Next Slide
                    if (e.AltKey)
                    {
                        if (tour.CurrentTourstopIndex < (tour.TourStops.Count - 1))
                        {
                            tour.CurrentTourstopIndex++;
                            TourEdit.SelectCurrent();
                            TourEdit.EnsureSelectedVisible();
                        }
                        return true;
                    }

                    break;
                case Keys.PageUp:
                    // Prev Slide
                    if (e.AltKey)
                    {
                        if (tour.CurrentTourstopIndex > 0)
                        {
                            tour.CurrentTourstopIndex--;
                            TourEdit.SelectCurrent();
                            TourEdit.EnsureSelectedVisible();
                        }
                        return true;
                    }
                    break;
            }
            return false;
        }

        private void SelectNext()
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return;
            }

            Focus = tour.CurrentTourStop.GetNextOverlay(Focus);
            Selection.SetSelection(Focus);
            OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
        }

        private void SelectLast()
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return;
            }

            Focus = tour.CurrentTourStop.GetPerviousOverlay(Focus);
            Selection.SetSelection(Focus);
            OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
        }

        public bool KeyUp(object sender, ElementEvent e)
        {
            if (CurrentEditor != null)
            {
                if (CurrentEditor.KeyUp(sender, e))
                {
                    return true;
                }
            }
            return false;
        }

        public bool AddPicture(System.Html.Data.Files.File file)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return false;
            }

            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(546, "Insert Picture"), tour));
            BitmapOverlay bmp = BitmapOverlay.Create(tour.CurrentTourStop, file);
            bmp.X = 960;
            bmp.Y = 600;
            tour.CurrentTourStop.AddOverlay(bmp);
            OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
            return true;
        }

        public bool AddFlipbook(string filename)
        {
            //if (tour == null || tour.CurrentTourStop == null)
            //{
            //    return false;
            //}

            ////todo localize

            //FlipbookSetup flipSetup = new FlipbookSetup();


            //if (flipSetup.ShowDialog() == DialogResult.OK)
            //{
            //    Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1342, "Insert Flipbook"), tour));
            //    FlipbookOverlay flipbook = new FlipbookOverlay(tour.CurrentTourStop, filename);
            //    flipbook.X = 960;
            //    flipbook.Y = 600;
            //    flipbook.LoopType = flipSetup.LoopType;
            //    flipbook.FramesY = flipSetup.FramesY;
            //    flipbook.FramesX = flipSetup.FramesX;
            //    flipbook.FrameSequence = flipSetup.FrameSequence;
            //    flipbook.StartFrame = flipSetup.StartFrame;
            //    flipbook.Frames = flipSetup.Frames;

            //    tour.CurrentTourStop.AddOverlay(flipbook);
            //    OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
            //    return true;
            //}
            return false;
        }

        public bool AddAudio(System.Html.Data.Files.File file, bool music)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return false;
            }

            AudioOverlay audio = AudioOverlay.Create(tour.CurrentTourStop, file);
            audio.X = 900;
            audio.Y = 600;

            if (music)
            {
                tour.CurrentTourStop.MusicTrack = audio;
            }
            else
            {
                tour.CurrentTourStop.VoiceTrack = audio;
            }
            
            return true;
        }

        public bool AddVideo(string filename)
        {
            // depracted video type
            //if (tour == null || tour.CurrentTourStop == null)
            //{
            //    return false;
            //}

            //VideoClip video = new VideoClip(Earth3d.MainWindow.Device, tour.CurrentTourStop, filename);
            //video.X = 960;
            //video.Y = 600;
            //tour.CurrentTourStop.AddOverlay(video);
            return true;

        }

        public bool AddText(string p, TextObject textObject)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return false;
            }

            TextOverlay text = TextOverlay.Create(textObject);
            text.Color = textObject.ForegroundColor;
            text.X = 960;
            text.Y = 600;
            //todo localize
            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(547, "Insert Text"), tour));
            tour.CurrentTourStop.AddOverlay(text);
            OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
            return true;
        }

        public Overlay AddOverlay(Overlay ol)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return null;
            }
            if (ol.GetType() == typeof(ShapeOverlay))
            {
                ShapeOverlay srcShapeOverlay = (ShapeOverlay)ol;
                if (srcShapeOverlay != null)
                {
                    ShapeOverlay shape = ShapeOverlay.Create(tour.CurrentTourStop, srcShapeOverlay.ShapeType);
                    shape.Width = srcShapeOverlay.Width;
                    shape.Height = srcShapeOverlay.Height;
                    shape.X = contextPoint.X;
                    shape.Y = contextPoint.Y;
                    shape.Color = srcShapeOverlay.Color;
                    shape.RotationAngle = srcShapeOverlay.RotationAngle;
                    //if (ol.AnimationTarget != null)
                    //{
                    //    shape.AnimationTarget = ol.AnimationTarget.Clone(shape);
                    //}
                    tour.CurrentTourStop.AddOverlay(shape);
                    return shape;
                }
            }
            else if (ol.GetType() == typeof(TextOverlay))
            {
                TextOverlay srcTxtOverlay = (TextOverlay)ol;
                if (srcTxtOverlay != null)
                {
                    TextOverlay text = TextOverlay.Create(srcTxtOverlay.TextObject);
                    text.X = contextPoint.X;
                    text.Y = contextPoint.Y;
                    text.Color = srcTxtOverlay.Color;
                    //if (ol.AnimationTarget != null)
                    //{
                    //    text.AnimationTarget = ol.AnimationTarget.Clone(text);
                    //}
                    tour.CurrentTourStop.AddOverlay(text);
                    return text;
                }
            }
            else if (ol.GetType() == typeof(BitmapOverlay))
            {
                BitmapOverlay srcBmpOverlay = (BitmapOverlay)ol;
                if (srcBmpOverlay != null)
                {
                    BitmapOverlay bitmap = srcBmpOverlay.Copy(tour.CurrentTourStop);
                    bitmap.X = contextPoint.X;
                    bitmap.Y = contextPoint.Y;
                    //if (ol.AnimationTarget != null)
                    //{
                    //    bitmap.AnimationTarget = ol.AnimationTarget.Clone(bitmap);
                    //}
                    tour.CurrentTourStop.AddOverlay(bitmap);
                    return bitmap;
                }
            }
            else if (ol.GetType() == typeof(FlipbookOverlay))
            {
                FlipbookOverlay srcFlipbookOverlay = (FlipbookOverlay)ol;
                if (srcFlipbookOverlay != null)
                {
                    FlipbookOverlay bitmap = srcFlipbookOverlay.Copy(tour.CurrentTourStop);
                    bitmap.X = contextPoint.X;
                    bitmap.Y = contextPoint.Y;
                    //if (ol.AnimationTarget != null)
                    //{
                    //    bitmap.AnimationTarget = ol.AnimationTarget.Clone(bitmap);
                    //}
                    tour.CurrentTourStop.AddOverlay(bitmap);
                    return bitmap;
                }
            }
            return null;
        }

        public bool AddShape(string p, ShapeType shapeType)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return false;
            }

            //todo localize
            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(548, "Insert Shape"), tour));

            ShapeOverlay shape = ShapeOverlay.Create(tour.CurrentTourStop, shapeType);
            shape.Width = 200;
            shape.Height = 200;

            if (shapeType == ShapeType.Arrow)
            {
                shape.Height /= 2;
            }
            if (shapeType == ShapeType.Line)
            {
                shape.Height = 12;
            }
            shape.X = 960;
            shape.Y = 600;
            tour.CurrentTourStop.AddOverlay(shape);

            Focus = shape;
            Selection.SetSelection(Focus);
            OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
            return true;
        }
        Color defaultColor = Colors.White;
        public Color GetCurrentColor()
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return defaultColor;
            }
            if (Focus != null)
            {
                return Focus.Color;
            }
            else
            {
                return defaultColor;
            }
        }

        public void SetCurrentColor(Color color)
        {
            defaultColor = color;
            if (tour == null || tour.CurrentTourStop == null)
            {
                return;
            }
            if (Focus != null)
            {
                Focus.Color = color;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (contextMenu != null)
            {
                contextMenu.Dispose();
                contextMenu = null;
            }
        }

        #endregion


        public bool Hover(Vector2d pnt)
        {
            if (CurrentEditor != null)
            {
                if (CurrentEditor.Hover(pnt))
                {
                    return true;
                }
            }
            return true;
        }

    }
    public class OverlayList
    {
        internal static void UpdateOverlayList(TourStop currentTourStop, Selection selection)
        {
        }

        internal static void UpdateOverlayListSelection(Selection selection)
        {
        }
    }
    public class TourEdit
    {
        internal static void EnsureSelectedVisible()
        {
        }

        

        internal static void SelectCurrent()
        {
        }
        internal static void UndoStep()
        {
            if (Undo.PeekAction())
            {
                Undo.StepBack();
                //todo wire this up to web ui
                //tourStopList.Refresh();
                //tourStopList.SelectedItem = tour.CurrentTourstopIndex;
                //ShowSlideStartPosition(tour.CurrentTourStop);
                //this.Refresh();
               // OverlayList.UpdateOverlayList(tour.CurrentTourStop, TourEditorUI.Selection);
            }
        }

        internal static void RedoStep()
        {
            if (Undo.PeekRedoAction())
            {
                Undo.StepForward();

                //tourStopList.Refresh();
                //tourStopList.SelectedItem = tour.CurrentTourstopIndex;
                //ShowSlideStartPosition(tour.CurrentTourStop);
                //this.Refresh();
                //OverlayList.UpdateOverlayList(tour.CurrentTourStop, TourEditorUI.Selection);
            }
        }
    }

    public class SoundEditor
    {
        public TourStop Target = null;
    }
    
    public class TourStopList
    {
        public TourDocument Tour = null;
        public bool ShowAddButton = false;

        public Dictionary<int, TourStop> SelectedItems = null;
        public int SelectedItem = -1;
        public void SelectAll()
        {
            SelectedItems = new Dictionary<int, TourStop>();
            for (int i = 0; i < Tour.TourStops.Count; i++)
            {
                SelectedItems[i] = Tour.TourStops[i];
            }
        }
        public Action refreshCallback = null;

        public void Refresh()
        {
            if (refreshCallback != null)
            {
                refreshCallback();
            }
        }

    
        public bool MultipleSelection = false;
        public bool HitType = false;

        public int FindItem(TourStop ts)
        {
            return -1;
        }
        public void EnsureSelectedVisible()
        {

        }
        public void EnsureAddVisible()
        {

        }
    }

    public class TimeLine
    {
        public static void RefreshUi()
        {

        }
    }
    public delegate void TextEditorDelegate(TextObject item, Action done);
    public delegate void SetNextSlideDelegate(SelectLink nextObj, Action done);
    
}
