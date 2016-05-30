using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Html.Media.Graphics;

namespace wwtlib
{
    public class Undo
    {
        static Stack<IUndoStep> undoStack = new Stack<IUndoStep>();
        static Stack<IUndoStep> redoStack = new Stack<IUndoStep>();
        
        public static void Clear()
        {
            undoStack = new Stack<IUndoStep>();
            redoStack = new Stack<IUndoStep>();
        }
        
        public static void Push(IUndoStep step)
        {
            undoStack.Push(step);
            redoStack = new Stack<IUndoStep>();
        }

        public static string PeekActionString()
        {
            if (undoStack.Count > 0)
            {
                return undoStack.Peek().ToString();
            }
            else
            {
                //todo localize
                return Language.GetLocalizedText(551, "Nothing to Undo");
            }
        }

        public static string PeekRedoActionString()
        {
            if (redoStack.Count > 0)
            {
                return redoStack.Peek().ToString();
            }
            else
            {
                return "";
            }
        }

        public static bool PeekAction()
        {
            return (undoStack.Count > 0);
        }

        public static bool PeekRedoAction()
        {
            return (redoStack.Count > 0);
        }

        public static void StepBack()
        {
            IUndoStep step = undoStack.Pop();

            step.Undo();

            redoStack.Push(step);
        }
       
        public static void StepForward()
        {
            IUndoStep step = redoStack.Pop();

            step.Redo();

            undoStack.Push(step);
        }
    }

    public class UndoStep : IUndoStep
    {
        public UndoStep()
        {
        }
        public void Undo()
        {
        }
        public void Redo()
        {
        }
        override public string ToString()
        {
            //todo localize
            return Language.GetLocalizedText(551, "Nothing to Undo");
        }
    }

    public class UndoTourSlidelistChange : IUndoStep
    {
        List<TourStop> undoList;
        List<TourStop> redoList;

        int currentIndex = 0;
        string actionText = "";

        public string ActionText
        {
            get { return actionText; }
            set { actionText = value; }
        }
        TourDocument targetTour = null;
        public UndoTourSlidelistChange(string text, TourDocument tour)
        {
            undoList = new List<TourStop>();

            for (int i = 0; i < tour.TourStops.Count; i++)
            {
                undoList.Add(tour.TourStops[i]);
            }

            currentIndex = tour.CurrentTourstopIndex;
            actionText = text;
            targetTour = tour;
            targetTour.TourDirty = true;
        }

        public void Undo()
        {

            redoList = targetTour.TourStops;
            targetTour.TourStops = undoList;
            targetTour.CurrentTourstopIndex = currentIndex;
            targetTour.TourDirty = true;

        }

        public void Redo()
        {
            undoList = targetTour.TourStops;
            targetTour.TourStops = redoList;
            targetTour.CurrentTourstopIndex = currentIndex;
            targetTour.TourDirty = true;
        }

        override public string ToString()
        {
            return actionText;
        }
    }

    public class UndoTourPropertiesChange : IUndoStep
    {

        string actionText = "";

        public string ActionText
        {
            get { return actionText; }
            set { actionText = value; }
        }
        TourDocument targetTour = null;
        string undoTitle;
        string undoAuthor;
        string undoAuthorEmail;
        string undoDescription;
        ImageElement undoAuthorImage;
        string undoOrganizationUrl;
        string undoOrgName;
        string undoKeywords;
        string undoTaxonomy;
        bool undoDomeMode;
        UserLevel undoLevel;
        string redoTitle;
        string redoAuthor;
        string redoAuthorEmail;
        string redoDescription;
        ImageElement redoAuthorImage;
        string redoOrganizationUrl;
        string redoOrgName;
        string redoKeywords;
        string redoTaxonomy;
        bool redoDomeMode;
        UserLevel redoLevel;
        public UndoTourPropertiesChange(string text, TourDocument tour)
        {
            undoTitle = tour.Title;
            undoAuthor = tour.Author;
            undoAuthorEmail = tour.AuthorEmail;
            undoDescription = tour.Description;
            undoAuthorImage = tour.AuthorImage;
            undoOrganizationUrl = tour.OrganizationUrl;
            undoOrgName = tour.OrgName;
            undoKeywords = tour.Keywords;
            undoTaxonomy = tour.Taxonomy;
            undoLevel = tour.Level;
    //        undoDomeMode = tour.DomeMode;

            actionText = text;
            targetTour = tour;
            targetTour.TourDirty = true;
        }

        public void Undo()
        {
            redoTitle = targetTour.Title;
            redoAuthor = targetTour.Author;
            redoAuthorEmail = targetTour.AuthorEmail;
            redoDescription = targetTour.Description;
            redoAuthorImage = targetTour.AuthorImage;
            redoOrganizationUrl = targetTour.OrganizationUrl;
            redoOrgName = targetTour.OrgName;
            redoKeywords = targetTour.Keywords;
            redoTaxonomy = targetTour.Taxonomy;
            redoLevel = targetTour.Level;
    //        redoDomeMode = targetTour.DomeMode;

            targetTour.Title = undoTitle;
            targetTour.Author = undoAuthor;
            targetTour.AuthorEmail = undoAuthorEmail;
            targetTour.Description = undoDescription;
            targetTour.AuthorImage = undoAuthorImage;
            targetTour.OrganizationUrl = undoOrganizationUrl;
            targetTour.OrgName = undoOrgName;
            targetTour.Keywords = undoKeywords;
            targetTour.Taxonomy = undoTaxonomy;
            targetTour.Level = undoLevel;
   //         targetTour.DomeMode = undoDomeMode;
            targetTour.TourDirty = true;


        }

        public void Redo()
        {
            targetTour.Title = redoTitle;
            targetTour.Author = redoAuthor;
            targetTour.AuthorEmail = redoAuthorEmail;
            targetTour.Description = redoDescription;
            targetTour.AuthorImage = redoAuthorImage;
            targetTour.OrganizationUrl = redoOrganizationUrl;
            targetTour.OrgName = redoOrgName;
            targetTour.Keywords = redoKeywords;
            targetTour.Taxonomy = redoTaxonomy;
            targetTour.Level = redoLevel;
            targetTour.TourDirty = true;
     //       targetTour.DomeMode = redoDomeMode;


        }

        override public string ToString()
        {
            return actionText;
        }
    }


}
