using MarkLight.ValueConverters;

namespace MarkLight.Views.UI
{
    /// <summary>
    /// HyperLink view. 
    /// </summary>
    /// <d>Displays text that can be pressed. Has the states: Default, Highlighted, Pressed and Disabled.</d>
    [HideInPresenter]
    public class HyperLink : Label
    {
        #region Fields

        /// <summary>
        /// Indicates if the hyperlink is disabled.
        /// </summary>
        /// <d>If set to true the hyperlink enters the Disabled state and can't be interacted with.</d>
        [ChangeHandler("IsDisabledChanged")]
        public _bool IsDisabled;

        /// <summary>
        /// Boolean indicating if the hyperlink is being pressed.
        /// </summary>
        [NotSetFromXuml]
        public bool IsPressed;

        /// <summary>
        /// Boolean indicating if mouse is over the hyperlink.
        /// </summary>
        [NotSetFromXuml]
        public bool IsMouseOver;
                
        #endregion

        #region Methods

        /// <summary>
        /// Sets default values of the view.
        /// </summary>
        public override void SetDefaultValues()
        {
            base.SetDefaultValues();

            Width.DirectValue = ElementSize.FromPixels(120);
            Height.DirectValue = ElementSize.FromPixels(40);
            FontColor.Value = ColorValueConverter.ColorCodes["lightblue"]; 
        }

        /// <summary>
        /// Called when IsDisabled field changes.
        /// </summary>
        public virtual void IsDisabledChanged()
        {
            if (IsDisabled)
            {
                SetState("Disbled");

                // disable hyperlink actions
                Click.IsDisabled = true;
                MouseEnter.IsDisabled = true;
                MouseExit.IsDisabled = true;
                MouseDown.IsDisabled = true;
                MouseUp.IsDisabled = true;
            }
            else
            {
                SetState(DefaultStateName);

                // enable hyperlink actions
                Click.IsDisabled = false;
                MouseEnter.IsDisabled = false;
                MouseExit.IsDisabled = false;
                MouseDown.IsDisabled = false;
                MouseUp.IsDisabled = false;
            }
        }

        /// <summary>
        /// Called when mouse enters.
        /// </summary>
        public void HyperLinkMouseEnter()
        {
            if (State == "Disabled")
                return;

            IsMouseOver = true;
            SetState(IsPressed ? "Pressed" : "Highlighted");
        }

        /// <summary>
        /// Called when mouse exits.
        /// </summary>
        public void HyperLinkMouseExit()
        {
            if (State == "Disabled")
                return;

            IsMouseOver = false;
            SetState(DefaultStateName);
        }

        /// <summary>
        /// Called when mouse down.
        /// </summary>
        public void HyperLinkMouseDown()
        {
            if (State == "Disabled")
                return;

            IsPressed = true;
            SetState("Pressed");
        }

        /// <summary>
        /// Called when mouse up.
        /// </summary>
        public void HyperLinkMouseUp()
        {
            if (State == "Disabled")
                return;

            IsPressed = false;
            SetState(IsMouseOver ? "Highlighted" : DefaultStateName);
        }

        #endregion
    }
}
