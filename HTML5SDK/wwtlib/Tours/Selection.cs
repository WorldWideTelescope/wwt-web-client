using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Html.Media.Graphics;

namespace wwtlib
{
    public class Selection
    {
        Texture SingleSelectHandles = null;
        Texture MultiSelectHandles = null;
        Texture FocusHandles = null;
        public Selection()
        {
        }

        public List<Overlay> SelectionSet = new List<Overlay>();

        //public Overlay[] SelectionSet
        //{
        //    get { return selectionSet.ToArray(); }
        //}

        public void ClearSelection()
        {
            SelectionSet.Clear();
        }

        public void AddSelection(Overlay overlay)
        {
            if (overlay != null)
            {
                if(!SelectionSet.Contains(overlay))
                {
                    SelectionSet.Add(overlay);
                }
               
            }
        }

        public void AddSelectionRange(Overlay[] overlays)
        {
            foreach (Overlay ov in overlays)
            {
                SelectionSet.Add(ov);
            }
        }

        public bool IsOverlaySelected(Overlay overlay)
        {
            return SelectionSet.Contains(overlay);
        }
   
        public void SetSelection(Overlay overlay)
        {
            SelectionSet.Clear();
            if (overlay != null)
            {
                SelectionSet.Add(overlay);
            }
        }

        public bool MultiSelect
        {
            get
            {
                return SelectionSet.Count > 1;
            }
        }

        public void SetSelectionRange(Overlay[] overlays)
        {
            SelectionSet.Clear();
            foreach (Overlay ov in overlays)
            {
                SelectionSet.Add(ov);
            }
        }

        Overlay focus = null;

        public Overlay Focus
        {
            get { return focus; }
            set { focus = value; }
        }


        double ratio = 1.0f;
        public void Draw3D(RenderContext renderContext, double transparancy)
        {
            ratio = 1116f / renderContext.Height;

            if (SingleSelectHandles == null)
            {
                SingleSelectHandles = Texture.FromUrl("images/Selhand.bmp");
            }

            if (MultiSelectHandles == null)
            {
                MultiSelectHandles = Texture.FromUrl("images/multiSelhand.bmp");
            }

            if (FocusHandles == null)
            {
                FocusHandles = Texture.FromUrl("images/FocusHandles.png");
            }

            if (SelectionSet.Count > 1)
            {
                foreach (Overlay overlay in SelectionSet)
                {
                    if (overlay == focus)
                    {

                        DrawSelectionHandles(renderContext, overlay, FocusHandles);
                    }
                    else
                    {

                        DrawSelectionHandles(renderContext, overlay, MultiSelectHandles);
                    }
                }
            }
            else
            {
                foreach (Overlay overlay in SelectionSet)
                {
                    DrawSelectionHandles(renderContext, overlay, SingleSelectHandles);
                }
            }
        }

        private static PositionColoredTextured[] points = new PositionColoredTextured[9 * 3 * 2];

        private Sprite2d sprite = new Sprite2d();

        private void DrawSelectionHandles(RenderContext renderContext, Overlay overlay, Texture handleTexture)
        {
            Rectangle[] handles = MakeHandles(overlay);
            double angle = overlay.RotationAngle;
            
            int i = 0;
            int j = 0;
            foreach (Rectangle handle in handles)
            {
                points[i + 0] = new PositionColoredTextured();
                points[i + 0].Position = overlay.MakePosition(centerX, centerY, handle.Left - centerX, handle.Top - centerY, angle);
                points[i + 0].Tu = j * (1f / 9f);
                points[i + 0].Tv = 0;
                points[i + 0].Color = Colors.White;

                points[i + 1] = new PositionColoredTextured();
                points[i + 1].Position = overlay.MakePosition(centerX, centerY, handle.Right - centerX, handle.Top - centerY, angle);
                points[i + 1].Tu = (j + 1) * (1f / 9f);
                points[i + 1].Tv = 0;
                points[i + 1].Color = Colors.White;

                points[i + 2] = new PositionColoredTextured();
                points[i + 2].Position = overlay.MakePosition(centerX, centerY, handle.Left - centerX, handle.Bottom - centerY, angle);
                points[i + 2].Tu = j * (1f / 9f);
                points[i + 2].Tv = 1;
                points[i + 2].Color = Colors.White;

                points[i + 3] = new PositionColoredTextured();
                points[i + 3].Position = overlay.MakePosition(centerX, centerY, handle.Right - centerX, handle.Top - centerY, angle);
                points[i + 3].Tu = (j + 1) * (1f / 9f);
                points[i + 3].Tv = 0;
                points[i + 3].Color = Colors.White;

                points[i + 4] = new PositionColoredTextured();
                points[i + 4].Position = overlay.MakePosition(centerX, centerY, handle.Right - centerX, handle.Bottom - centerY, angle);
                points[i + 4].Tu = (j + 1) * (1f / 9f);
                points[i + 4].Tv = 1;
                points[i + 4].Color = Colors.White;

                points[i + 5] = new PositionColoredTextured();
                points[i + 5].Position = overlay.MakePosition(centerX, centerY, handle.Left - centerX, handle.Bottom - centerY, angle);
                points[i + 5].Tu = j * (1f / 9f);
                points[i + 5].Tv = 1;
                points[i + 5].Color = Colors.White;

                i += 6;
                j++;
            }

            if (MultiSelect)
            {
                sprite.Draw(renderContext, points, points.Length - 6, handleTexture, false, 1);
            }
            else
            {
                sprite.Draw(renderContext, points, points.Length, handleTexture, false, 1);
            }
        }

        public Vector2d PointToSelectionSpace(Vector2d pntIn)
        {
            Vector2d[] tempPoints = new Vector2d[1];
            tempPoints[0] = Vector2d.Create(pntIn.X, pntIn.Y);

            Matrix2d mat = Matrix2d.RotateAt(-SelectionSet[0].RotationAngle / 180 * Math.PI, Vector2d.Create((SelectionSet[0].X), (SelectionSet[0].Y)));
            mat.TransformPoints(tempPoints);
            return tempPoints[0];
        }

        public Vector2d PointToScreenSpace(Vector2d pntIn)
        {
            Vector2d[] tempPoints = new Vector2d[1];
            tempPoints[0] = Vector2d.Create(pntIn.X, pntIn.Y);

            Matrix2d mat = Matrix2d.RotateAt(SelectionSet[0].RotationAngle / 180 * Math.PI, Vector2d.Create((SelectionSet[0].X), (SelectionSet[0].Y)));
            mat.TransformPoints(tempPoints);
            return tempPoints[0];
        }    
        
        public SelectionAnchor HitTest(Vector2d position)
        {
            if (SelectionSet.Count == 1)
            {
                foreach (Overlay overlay in SelectionSet)
                {
                    Rectangle[] handles = MakeHandles(overlay);
                    int index = 0;

                    Vector2d testPoint = PointToSelectionSpace(position);
                    foreach (Rectangle rectf in handles)
                    {
                        if (rectf.Contains(testPoint))
                        {
                            return (SelectionAnchor)index;
                        }
                        index++;
                    }
                }
            }

            return SelectionAnchor.None;

        }

        double centerX = 0;
        double centerY = 0;
        public Rectangle[] MakeHandles(Overlay overlay)
        {
            double x = ((int)(overlay.X-(overlay.Width/2)))+.5f;
            double y = ((int)overlay.Y-(overlay.Height/2))+.5f;

            centerX = overlay.X;
            centerY = overlay.Y;

            double width = overlay.Width;
            double height = overlay.Height;
            double handleSize = 12*ratio;
            Rectangle[] handles = new Rectangle[9];

            handles[0] = Rectangle.Create(x - handleSize, y - handleSize, handleSize, handleSize);
            handles[1] = Rectangle.Create((x + (width / 2)) - (handleSize/2), y - handleSize, handleSize, handleSize);
            handles[2] = Rectangle.Create(x + width, y - handleSize, handleSize, handleSize);
            handles[3] = Rectangle.Create(x + width, (y + (height / 2)) - (handleSize / 2), handleSize, handleSize);
            handles[4] = Rectangle.Create(x + width, (y + height), handleSize, handleSize);
            handles[5] = Rectangle.Create((x + (width / 2)) - (handleSize / 2), (y + height), handleSize, handleSize);
            handles[6] = Rectangle.Create(x - handleSize, (y + height), handleSize, handleSize);
            handles[7] = Rectangle.Create(x - handleSize, (y + (height / 2)) - (handleSize / 2), handleSize, handleSize);
            handles[8] = Rectangle.Create((x + (width / 2)) - (handleSize / 2), y - 30f*ratio, handleSize, handleSize);
            return handles;
        }

    }
    public enum SelectionAnchor { TopLeft=0, Top=1, TopRight=2, Right=3, BottomRight=4, Bottom=5, BottomLeft=6, Left=7, Rotate=8, Move=9, Center=10, None=11 };
 
}
