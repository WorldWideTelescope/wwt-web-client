using System;


namespace wwtlib
{
    public class Dialog
    {
        public Dialog() { }

        public event EventHandler<EventArgs> ShowDialogHook;

        public void Show(object dialogArgs, EventArgs e)
        {
            
            if (ShowDialogHook != null)
            {
                ShowDialogHook.Invoke(dialogArgs, e);
            }
        }
    }

    public class FrameWizard : Dialog
    {
        public FrameWizard()
        {

        }
        public void OK(ReferenceFrame frame)
        {
            LayerManager.referenceFrameWizardFinished(frame);
        }
    }

    public class ReferenceFrameProps : Dialog
    {
        public ReferenceFrameProps()
        {

        }
        public void OK(ReferenceFrame frame)
        {
            LayerManager.LoadTree();
        }
    }

    public class GreatCircleDialog : Dialog
    {
        public GreatCircleDialog()
        {

        }
        public void OK(ReferenceFrame frame)
        {
            
        }
    }
}
