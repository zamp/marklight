
namespace MarkLight.Views
{
    /// <summary>
    /// Animates views.
    /// </summary>
    /// <d>Animates views.</d>
    [HideInPresenter]
    public class ViewAnimation : View
    {
        #region Fields

        /// <summary>
        /// Animation target view.
        /// </summary>
        /// <d>The animation target view.</d>
        [ChangeHandler("BehaviorChanged")]
        public View Target;

        /// <summary>
        /// Animation started.
        /// </summary>
        /// <d>Triggered when the animation has started.</d>
        public ViewAction AnimationStarted;

        /// <summary>
        /// Animation reversed.
        /// </summary>
        /// <d>Triggered when the animation is reversed.</d>
        public ViewAction AnimationReversed;

        /// <summary>
        /// Animation completed.
        /// </summary>
        /// <d>Triggered when the animation is completed.</d>
        public ViewAction AnimationCompleted;

        /// <summary>
        /// Animation stopped.
        /// </summary>
        /// <d>Triggered when the animation is stopped.</d>
        public ViewAction AnimationStopped;

        /// <summary>
        /// Animation paused.
        /// </summary>
        /// <d>Triggered when the animation has paused.</d>
        public ViewAction AnimationPaused;

        /// <summary>
        /// Animation resumed.
        /// </summary>
        /// <d>Triggered when the animation resumes playing.</d>
        public ViewAction AnimationResumed;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a boolean indicating whether this animation is active.
        /// </summary>
        public virtual bool IsAnimationRunning
        {
            get
            {
                var isActive = false;

                this.ForEachChild<ViewAnimation>(
                    x => isActive = isActive || x.IsAnimationRunning,
                    ViewSearchArgs.NonRecursive);

                return isActive;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether this animation is reversing.
        /// </summary>
        public virtual bool IsAnimationReversing
        {
            get
            {
                var isReversing = false;

                this.ForEachChild<ViewAnimation>(
                    x => isReversing = isReversing || x.IsAnimationReversing,
                    ViewSearchArgs.NonRecursive);

                return isReversing;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether this animation is completed.
        /// </summary>
        public virtual bool IsAnimationCompleted
        {
            get
            {
                var isCompleted = true;

                this.ForEachChild<ViewAnimation>(
                    x => isCompleted = isCompleted && x.IsAnimationCompleted,
                    ViewSearchArgs.NonRecursive);

                return isCompleted;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether this animation is paused.
        /// </summary>
        public virtual bool IsAnimationPaused
        {
            get
            {
                var isPaused = true;

                this.ForEachChild<ViewAnimation>(
                    x => isPaused = isPaused && x.IsAnimationPaused,
                    ViewSearchArgs.NonRecursive);

                return isPaused;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets default values of the view.
        /// </summary>
        public override void SetDefaultValues()
        {
            base.SetDefaultValues();
            GameObject.hideFlags = UnityEngine.HideFlags.HideInHierarchy;
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        public virtual void StartAnimation()
        {
            this.ForEachChild<ViewAnimation>(x => x.StartAnimation(), ViewSearchArgs.NonRecursive);
            AnimationStarted.Trigger();
        }

        /// <summary>
        /// Stops the animation.
        /// </summary>
        public virtual void StopAnimation()
        {
            this.ForEachChild<ViewAnimation>(x => x.StopAnimation(), ViewSearchArgs.NonRecursive);
            AnimationStopped.Trigger();
        }

        /// <summary>
        /// Resets the animation.
        /// </summary>
        public virtual void ResetAnimation()
        {
            this.ForEachChild<ViewAnimation>(x => x.ResetAnimation(), ViewSearchArgs.NonRecursive);
        }

        /// <summary>
        /// Resets and stops the animation.
        /// </summary>
        public virtual void ResetAndStopAnimation()
        {
            this.ForEachChild<ViewAnimation>(x => x.ResetAndStopAnimation(), ViewSearchArgs.NonRecursive);
            AnimationStopped.Trigger();
        }

        /// <summary>
        /// Reverses the animation.
        /// </summary>
        public virtual void ReverseAnimation()
        {
            this.ForEachChild<ViewAnimation>(x => x.ReverseAnimation(), ViewSearchArgs.NonRecursive);
            AnimationReversed.Trigger();
        }

        /// <summary>
        /// Pauses the animation.
        /// </summary>
        public virtual void PauseAnimation()
        {
            this.ForEachChild<ViewAnimation>(x => x.PauseAnimation(), ViewSearchArgs.NonRecursive);
            AnimationPaused.Trigger();
        }

        /// <summary>
        /// Resumes the animation.
        /// </summary>
        public virtual void ResumeAnimation()
        {
            this.ForEachChild<ViewAnimation>(x => x.ResumeAnimation(), ViewSearchArgs.NonRecursive);
            AnimationResumed.Trigger();
        }

        /// <summary>
        /// Sets animation target.
        /// </summary>
        public virtual void SetAnimationTarget(View view)
        {
            Target = view;
            this.ForEachChild<ViewAnimation>(x => x.SetAnimationTarget(view), ViewSearchArgs.NonRecursive);
        }

        /// <summary>
        /// Called if a child animation has been completed. 
        /// </summary>
        public virtual void ChildAnimationCompleted()
        {            
            if (IsAnimationCompleted)
            {
                AnimationCompleted.Trigger();
            }
        }

        #endregion

    }
}
